using System;
using System.Linq;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.Extensions.Game;
using ApacheTech.VintageMods.Core.Hosting;
using ApacheTech.VintageMods.Core.Services.Configuration.Contracts;
using ApacheTech.VintageMods.Core.Services.Configuration.Extensions;
using ApacheTech.VintageMods.FluentChatCommands;
using ApacheTech.VintageMods.WaypointExtensions.Features.GPS.Packets;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.WaypointExtensions.Features.GPS
{
    /// <summary>
    ///     Feature: Global Positioning System.
    /// 
    ///      - Display your current XYZ coordinates.
    ///      - Copy your current XYZ coordinates to clipboard.
    ///      - Send your current XYZ coordinates as a chat message to the current chat group.
    ///      - Whisper your current XYZ coordinates to a single player (server settings permitting).
    ///      - Server: Enable/Disable permissions to whisper to other members of the server.
    /// </summary>
    /// <seealso cref="UniversalModSystem" />
    public sealed class GPS : UniversalModSystem
    {
        private IGlobalConfiguration _globalSettings;
        private GPSOptionsPacket _packet = new();

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The core API implemented by the client. The main interface for accessing the client. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            ModServices.Network.DefaultClientChannel
                .RegisterMessageType<WhisperPacket>()
                .RegisterMessageType<GPSOptionsPacket>()
                .SetMessageHandler<GPSOptionsPacket>(OnClientSetPropertyPacketReceived);

            FluentChat.ClientCommand("gps")
                .RegisterWith(capi)
                .HasDescription(Lang.Get("wpex:features.gps.client.description"))
                .HasDefaultHandler(OnClientDefaultHandler)
                .HasSubCommand("chat").WithHandler(OnClientSubCommandBroadcast)
                .HasSubCommand("copy").WithHandler(OnClientSubCommandClipboard)
                .HasSubCommand("to").WithHandler(OnClientSubCommandWhisper);
        }

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreServerAPI in Start()
        /// </summary>
        /// <param name="sapi">The core API implemented by the server. The main interface for accessing the server. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            ModServices.Network.DefaultServerChannel
                .RegisterMessageType<WhisperPacket>()
                .RegisterMessageType<GPSOptionsPacket>()
                .SetMessageHandler<WhisperPacket>(OnServerWhisperPacketReceived);

            FluentChat
                .ServerCommand("wpconfig")
                .HasSubCommand("gps")
                .WithHandler(OnServerConfigCommand);

            _globalSettings = ModServices.IOC.Resolve<IGlobalConfiguration>();
            _packet = _globalSettings.FeatureAs<GPSOptionsPacket>("GPS");
            sapi.Event.PlayerNowPlaying += OnServerPlayerJoin;
        }

        #region Client Chat Commands

        /// <summary>
        ///     Client Default Command Handler: .gps
        /// </summary>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientDefaultHandler(int groupId, CmdArgs args)
        {
            var player = Capi.World.Player;
            var pos = PlayerLocationMessage(player);
            Capi.ShowChatMessage(pos);
        }

        /// <summary>
        ///     Client Sub-Command Handler: .gps chat
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientSubCommandBroadcast(string subCommandName, int groupId, CmdArgs args)
        {
            var player = Capi.World.Player;
            var pos = PlayerLocationMessage(player);
            Capi.SendChatMessage(pos);
        }

        /// <summary>
        ///     Client Sub-Command Handler: .gps copy
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientSubCommandClipboard(string subCommandName, int groupId, CmdArgs args)
        {
            var player = Capi.World.Player;
            var pos = PlayerLocationMessage(player);
            Capi.Forms.SetClipboardText($"{player.PlayerName}: {pos}");
            Capi.ShowChatMessage(Lang.Get("wpex:features.gps.client.location-copied-to-clipboard"));
        }

        /// <summary>
        ///     Client Sub-Command Handler: .gps to [playerName]
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientSubCommandWhisper(string subCommandName, int groupId, CmdArgs args)
        {
            if (!ModServices.Network.DefaultClientChannel.Connected)
            {
                Capi.ShowChatMessage(Lang.Get("wpex:error-messages.mod-not-installed-on-server"));
                return;
            }
            if (!_packet.WhispersAllowed)
            {
                Capi.ShowChatMessage(Lang.Get("wpex:error-messages.feature-disabled"));
                return;
            }
            var message = PlayerLocationMessage(Capi.World.Player);
            var recipient = args.PopWord();

            ModServices.Network.DefaultClientChannel.SendPacket(new WhisperPacket
            {
                RecipientName = recipient,
                GroupId = GlobalConstants.AllChatGroups,
                Message = message
            });
        }

        #endregion

        /// <summary>
        ///     Sets a property within the class, from the other app-side.
        /// </summary>
        /// <param name="packet">The packet, containing the property name, and value to set.</param>
        private void OnClientSetPropertyPacketReceived(GPSOptionsPacket packet)
        {
            _packet = packet;
        }

        /// <summary>
        ///     Called when a Server Admin changes the settings for the GPS feature.
        /// </summary>
        /// <param name="subCommandName">Name of the sub command.</param>
        /// <param name="player">The player.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="args">The arguments.</param>
        private void OnServerConfigCommand(string subCommandName, IServerPlayer player, int groupId, CmdArgs args)
        {
            // TODO: Breaks Open/Closed, DRY, SRP... Refactor to fix SOLID concerns.
            switch (args.PopWord())
            {
                case "whisper":
                    switch (args.PopWord())
                    {
                        case "enable":
                            SetWhispersAllowed(true);
                            player.SendMessage(groupId, Lang.Get("wpex:features.gps.server.gps-whisper-enabled"), EnumChatType.CommandSuccess);
                            break;

                        case "disable":
                            SetWhispersAllowed(false);
                            player.SendMessage(groupId, Lang.Get("wpex:features.gps.server.gps-whisper-disabled"), EnumChatType.CommandSuccess);
                            break;

                        default:
                            player.SendMessage(groupId, Lang.Get("wpex:features.gps.server.invalid-command-syntax"), EnumChatType.CommandError);
                            break;
                    }
                    break;
                default:
                    player.SendMessage(groupId, Lang.Get("wpex:features.gps.server.invalid-command-syntax"), EnumChatType.CommandError);
                    break;
            }
        }

        private void SetWhispersAllowed(bool value)
        {
            _packet.WhispersAllowed = value;
            _globalSettings.SaveFeature("GPS", _packet);
            ModServices.Network.DefaultServerChannel.BroadcastPacket(_packet);
        }

        /// <summary>
        ///     Called when any player joins the Server.
        /// </summary>
        /// <param name="joiningPlayer">The player that is joining the server.</param>
        private void OnServerPlayerJoin(IServerPlayer joiningPlayer)
        {
            ModServices.Network.DefaultServerChannel.SendPacket(_packet, joiningPlayer);
        }

        /// <summary>
        ///     Called when a <see cref="WhisperPacket"/> packet is received on the Server.
        /// </summary>
        /// <param name="fromPlayer">The player who's Client sent the packet..</param>
        /// <param name="packet">The packet set from the Client.</param>
        private void OnServerWhisperPacketReceived(IServerPlayer fromPlayer, WhisperPacket packet)
        {
            var toPlayer = Sapi.World.AllOnlinePlayers
                .FirstOrDefault(p => p.PlayerName.StartsWith(packet.RecipientName, StringComparison.InvariantCultureIgnoreCase));

            if (toPlayer is null)
            {
                Sapi.SendMessage(fromPlayer, packet.GroupId, Lang.Get("wpex:features.gps.client.player-not-found", packet.RecipientName), EnumChatType.OwnMessage);
                return;
            }

            var receivedMessage = Lang.Get("wpex:features.gps.server.whisper-received", fromPlayer.PlayerName, packet.Message);
            Sapi.SendMessage(toPlayer as IServerPlayer, packet.GroupId, receivedMessage, EnumChatType.OwnMessage);

            var sentMessage = Lang.Get("wpex:features.gps.server.whisper-sent", toPlayer.PlayerName, packet.Message);
            Sapi.SendMessage(fromPlayer, packet.GroupId, sentMessage, EnumChatType.OwnMessage);
        }

        /// <summary>
        ///     Retrieves the player's current location. 
        /// </summary>
        /// <param name="player">The player to find the location of.</param>
        /// <returns>A string representation of the XYZ coordinates of the player.</returns>
        private string PlayerLocationMessage(IPlayer player)
        {
            var pos = player.Entity.Pos.AsBlockPos.RelativeToSpawn();
            return $"X = {pos.X}, Y = {pos.Y}, Z = {pos.Z}.";
        }
    }
}
