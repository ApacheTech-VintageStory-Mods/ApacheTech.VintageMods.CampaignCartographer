using System;
using System.Linq;
using ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Broker;
using ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Packets;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS
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
        private GPSChatCommandBroker _broker;
        private GpsSettings _settings;

        private ICoreClientAPI _capi;
        private ICoreServerAPI _sapi;

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The core API implemented by the client. The main interface for accessing the client. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            _capi = capi;
            ModServices.Network.DefaultClientChannel
                .RegisterMessageType<WhisperPacket>()
                .RegisterMessageType<GpsSettings>()
                .SetMessageHandler<GpsSettings>(OnClientSetPropertyPacketReceived);

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
            _sapi = sapi;
            _broker = ModServices.IOC.Resolve<GPSChatCommandBroker>();
            _settings = ModServices.IOC.Resolve<GpsSettings>();

            ModServices.Network.DefaultServerChannel
                .RegisterMessageType<WhisperPacket>()
                .RegisterMessageType<GpsSettings>()
                .SetMessageHandler<WhisperPacket>(OnServerWhisperPacketReceived);

            FluentChat
                .ServerCommand("wpconfig")
                .HasSubCommand("gps")
                .WithHandler(OnServerConfigCommand);

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
            var player = _capi.World.Player;
            var pos = PlayerLocationMessage(player);
            _capi.ShowChatMessage(pos);
        }

        /// <summary>
        ///     Client Sub-Command Handler: .gps chat
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientSubCommandBroadcast(string subCommandName, int groupId, CmdArgs args)
        {
            var player = _capi.World.Player;
            var pos = PlayerLocationMessage(player);
            _capi.SendChatMessage(pos);
        }

        /// <summary>
        ///     Client Sub-Command Handler: .gps copy
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientSubCommandClipboard(string subCommandName, int groupId, CmdArgs args)
        {
            var player = _capi.World.Player;
            var pos = PlayerLocationMessage(player);
            _capi.Forms.SetClipboardText($"{player.PlayerName}: {pos}");
            _capi.ShowChatMessage(Lang.Get("wpex:features.gps.client.location-copied-to-clipboard"));
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
                _capi.ShowChatMessage(Lang.Get("wpex:error-messages.mod-not-installed-on-server"));
                return;
            }
            if (!_settings.WhispersAllowed)
            {
                _capi.ShowChatMessage(Lang.Get("wpex:error-messages.feature-disabled"));
                return;
            }
            var message = PlayerLocationMessage(_capi.World.Player);
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
        private void OnClientSetPropertyPacketReceived(GpsSettings packet)
        {
            _settings = packet;
        }

        /// <summary>
        ///     Called when a Server Admin changes the settings for the GPS feature.
        /// </summary>
        /// <param name="subCommandName">Name of the sub command.</param>
        /// <param name="player">The player that initiated the command.</param>
        /// <param name="groupId">The chat group to pass messages back to.</param>
        /// <param name="args">The arguments passed in, by the user.</param>
        private void OnServerConfigCommand(string subCommandName, IServerPlayer player, int groupId, CmdArgs args)
        {
            _broker.OnServerConfigCommand(player, groupId, args);
        }

        /// <summary>
        ///     Called when any player joins the Server.
        /// </summary>
        /// <param name="joiningPlayer">The player that is joining the server.</param>
        private void OnServerPlayerJoin(IServerPlayer joiningPlayer)
        {
            ModServices.Network.DefaultServerChannel.SendPacket(_settings, joiningPlayer);
        }

        /// <summary>
        ///     Called when a <see cref="WhisperPacket"/> packet is received on the Server.
        /// </summary>
        /// <param name="fromPlayer">The player who's Client sent the packet..</param>
        /// <param name="packet">The packet set from the Client.</param>
        private void OnServerWhisperPacketReceived(IServerPlayer fromPlayer, WhisperPacket packet)
        {
            var toPlayer = _sapi.World.AllOnlinePlayers
                .FirstOrDefault(p =>
                    p.PlayerName.StartsWith(packet.RecipientName, StringComparison.InvariantCultureIgnoreCase));

            if (toPlayer is null)
            {
                _sapi.SendMessage(fromPlayer, packet.GroupId, Lang.Get("wpex:features.gps.client.player-not-found", packet.RecipientName), EnumChatType.OwnMessage);
                return;
            }

            var receivedMessage = Lang.Get("wpex:features.gps.server.whisper-received", fromPlayer.PlayerName, packet.Message);
            _sapi.SendMessage(toPlayer as IServerPlayer, packet.GroupId, receivedMessage, EnumChatType.OwnMessage);

            var sentMessage = Lang.Get("wpex:features.gps.server.whisper-sent", toPlayer.PlayerName, packet.Message);
            _sapi.SendMessage(fromPlayer, packet.GroupId, sentMessage, EnumChatType.OwnMessage);
        }

        /// <summary>
        ///     Retrieves the player's current location. 
        /// </summary>
        /// <param name="player">The player to find the location of.</param>
        /// <returns>A string representation of the XYZ coordinates of the player.</returns>
        private static string PlayerLocationMessage(IPlayer player)
        {
            var pos = player.Entity.Pos.AsBlockPos.RelativeToSpawn();
            return $"X = {pos.X}, Y = {pos.Y}, Z = {pos.Z}.";
        }
    }
}
