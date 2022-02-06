using System;
using System.Linq;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.VintageMods.CampaignCartographer.Features.CentreMap.Packets;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.CentreMap
{
    /// <summary>
    ///     Feature: Centre Map
    ///      • Centre map on self.
    ///      • Centre map on an online player.
    ///      • Centre map on a specific X,Z location within the world.
    ///      • Centre map on a specific waypoint.
    ///      • Centre map on World Spawn.
    ///      • Centre map on the player's own spawn point.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    public sealed class CentreMapClient : ClientModSystem
    {
        private ICoreClientAPI _capi;
        private WorldMapManager _worldMap;
        private IClientNetworkChannel _clientChannel;

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The core API implemented by the client. The main interface for accessing the client. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            _capi = capi;
            _worldMap = capi.ModLoader.GetModSystem<WorldMapManager>();

            _capi.Event.PlayerEntitySpawn += OnPlayerSpawn;
            _capi.Event.PlayerEntityDespawn += OnPlayerDespawn;

            _clientChannel = _capi.Network.RegisterChannel("centreMap")
                .RegisterMessageType<PlayerSpawnPositionDto>()
                .SetMessageHandler<PlayerSpawnPositionDto>(OnClientSpawnPointResponsePacketReceived);

            FluentChat.ClientCommand("cm")
                .RegisterWith(capi)
                .HasDescription(LangEx.FeatureString("CentreMap", "SettingsCommandDescription"))
                .HasDefaultHandler(OnSelfOption)
                .HasSubCommand("self").WithHandler((_, id, args) => OnSelfOption(id, args))
                .HasSubCommand("home").WithHandler(OnHomeOption)
                .HasSubCommand("player").WithHandler(OnPlayerOption)
                .HasSubCommand("plr").WithHandler(OnPlayerOption)
                .HasSubCommand("position").WithHandler(OnPositionOption)
                .HasSubCommand("pos").WithHandler(OnPositionOption)
                .HasSubCommand("spawn").WithHandler(OnSpawnOption)
                .HasSubCommand("waypoint").WithHandler(OnWaypointOption);
        }

        private void OnPlayerSpawn(IClientPlayer byPlayer)
        {
            _capi.Event.EnqueueMainThreadTask(() =>
                _capi.Event.RegisterCallback(_ =>
                    _worldMap.GetField<IClientNetworkChannel>("clientChannel")
                        .SendPacket(new OnViewChangedPacket()), 500), "");
        }

        private void OnPlayerDespawn(IClientPlayer byPlayer)
        {
            _capi.Event.EnqueueMainThreadTask(() =>
                _capi.Event.RegisterCallback(_ =>
                    _worldMap.GetField<IClientNetworkChannel>("clientChannel")
                        .SendPacket(new OnViewChangedPacket()), 500), "");
        }

        #region Command Handlers

        /// <summary>
        ///      • Centre map on self.
        /// </summary>
        private void OnSelfOption(int groupId, CmdArgs args)
        {
            var player = _capi.World.Player;
            var displayPos = player.Entity.Pos.AsBlockPos.RelativeToSpawn();
            var message = LangEx.FeatureString("CentreMap", "CentreMapOnPlayer", player.PlayerName, displayPos.X, displayPos.Y, displayPos.Z);
            RecentreAndProvideFeedback(player.Entity.Pos.XYZ, message);
        }

        /// <summary>
        ///      • Centre map on the player's own spawn point.
        /// </summary>
        private void OnHomeOption(string subCommandName, int groupId, CmdArgs args)
        {   
            if (!_clientChannel.Connected)
            {
                _capi.ShowChatMessage(LangEx.Get("error-messages.mod-not-installed-on-server"));
                return;
            }
            _clientChannel.SendPacket<PlayerSpawnPositionDto>();
        }

        /// <summary>
        ///      • Centre map on an online player.
        /// </summary>
        private void OnPlayerOption(string subCommandName, int groupId, CmdArgs args)
        {
            var player = _capi.World.Player;
            var targetName = args.PopWord(player.PlayerName);

            var allPlayers = _capi.World.AllPlayers;
            var playerList = allPlayers
                .Where(p => p.PlayerName
                    .StartsWith(targetName, StringComparison.InvariantCultureIgnoreCase))
                .ToList(); 

            if (!playerList.Any()) return;
            var target = (IClientPlayer)playerList.FirstOrDefault() ?? player;

            if (target.Entity is null)
            {
                _capi.EnqueueShowChatMessage(LangEx.FeatureString("CentreMap", "CannotCentreMapOnPlayer", target.PlayerName));
                return;
            }

            var displayPos = target.Entity.Pos.AsBlockPos.RelativeToSpawn();
            var message = LangEx.FeatureString("CentreMap", "CentreMapOnPlayer", target.PlayerName, displayPos.X, displayPos.Y, displayPos.Z);
            RecentreAndProvideFeedback(target.Entity.Pos.XYZ, message);

        }

        /// <summary>
        ///      • Centre map on a specific X,Z location within the world.
        /// </summary>
        private void OnPositionOption(string subCommandName, int groupId, CmdArgs args)
        {
            var playerPos = _capi.World.Player.Entity.Pos.AsBlockPos;
            var x = args.PopInt().GetValueOrDefault(playerPos.X);
            var z = args.PopInt().GetValueOrDefault(playerPos.Z);

            var displayPos = new BlockPos(x, 1, z);
            var pos = displayPos.Add(_capi.World.DefaultSpawnPosition.AsBlockPos);
            var message = LangEx.FeatureString("CentreMap", "CentreMapOnPosition", displayPos.X, displayPos.Z);
            RecentreAndProvideFeedback(pos.ToVec3d(), message);
        }

        /// <summary>
        ///      • Centre map on World Spawn.
        /// </summary>
        private void OnSpawnOption(string subCommandName, int groupId, CmdArgs args)
        {
            var pos = _capi.World.DefaultSpawnPosition.AsBlockPos;
            var displayPos = pos.RelativeToSpawn() ?? pos;
            var message = LangEx.FeatureString("CentreMap", "CentreMapOnWorldSpawn", displayPos.X, displayPos.Z);
            RecentreAndProvideFeedback(pos.ToVec3d(), message);
        }

        /// <summary>
        ///      • Centre map on a specific waypoint.
        /// </summary>
        private void OnWaypointOption(string subCommandName, int groupId, CmdArgs args)
        {
            var waypointId = args.PopInt().GetValueOrDefault(0);
            var target = _worldMap.WaypointMapLayer().ownWaypoints[waypointId];

            var pos = target.Position.AsBlockPos;
            var displayPos = pos.RelativeToSpawn() ?? pos;
            var message = LangEx.FeatureString("CentreMap", "CentreMapOnWaypoint", waypointId, target.Title, displayPos.X, displayPos.Z);
            RecentreAndProvideFeedback(pos.ToVec3d(), message);
        }

        #endregion

        /// <summary>
        ///     Called when on the client, when the server sends a <see cref="PlayerSpawnPositionDto"/> packet.
        /// </summary>
        /// <param name="packet">The packet that was sent.</param>
        private void OnClientSpawnPointResponsePacketReceived(PlayerSpawnPositionDto packet)
        {
            var displayPos = packet.SpawnPosition.RelativeToSpawn();
            var message = LangEx.FeatureString("CentreMap", "CentreMapOnPlayerSpawn", displayPos.X, displayPos.Z);
            RecentreAndProvideFeedback(packet.SpawnPosition.ToVec3d(), message);
        }

        private void RecentreAndProvideFeedback(Vec3d position, string message)
        {
            _worldMap.RecentreMap(position);
            _capi.ShowChatMessage(message);
        }
    }

}
