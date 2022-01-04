using System;
using System.Collections.Generic;
using System.Linq;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Extensions.System;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedMember.Local

namespace ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints
{
    public class WaypointService
    {
        private readonly ICoreClientAPI _capi;
        private readonly WorldMapManager _mapManager;

        public SortedDictionary<string, WaypointInfoModel> WaypointTypes { get; } = new();

        public WaypointService(ICoreClientAPI capi)
        {
            _capi = capi;
            _mapManager = (_capi = capi).ModLoader.GetModSystem<WorldMapManager>();
            InitialiseComponents();
        }

        private void InitialiseComponents()
        {
            try
            {
                var defaultWaypointsFile = ModServices.FileSystem.GetJsonFile("default-waypoints.json");
                var customWaypointsFile = ModServices.FileSystem.GetJsonFile("custom-waypoints.json");

                if (ModVersion.InstalledVersion("version.data") < ModVersion.ArchiveVersion())
                {
                    _capi.Logger.VerboseDebug("Campaign Cartographer: Updating global default files.");
                    var globalConfigFile = ModServices.FileSystem.GetJsonFile("version.data");
                    var modAssembly = AssemblyEx.GetModAssembly();
                    defaultWaypointsFile.DisembedFrom(modAssembly);
                    globalConfigFile.DisembedFrom(GetType().Assembly);
                }

                WaypointTypes.AddOrUpdateRange(defaultWaypointsFile.ParseAsMany<WaypointInfoModel>(), p => p.Syntax);
                WaypointTypes.AddOrUpdateRange(customWaypointsFile.ParseAsMany<WaypointInfoModel>(), p => p.Syntax);

                _capi.Logger.Event($"{WaypointTypes.Count} waypoint extensions loaded.");
            }
            catch (Exception e)
            {
                _capi.Logger.Error($"Waypoint Extensions: Error loading syntax for .wp command; {e.Message}");
                _capi.Logger.Error(e.StackTrace);
            }
        }

        public WaypointInfoModel GetWaypointModel(string syntax)
        {
            return WaypointTypes.ContainsKey(syntax) 
                ? WaypointTypes[syntax].DeepClone() 
                : null;
        }

        public void PurgeWaypointsNearby(float radius)
        {
            var playerPos = _capi.World.Player.Entity.Pos.AsBlockPos;
            PurgeWaypoints(p => p.IsInHorizontalRangeOf(playerPos, radius));
        }

        public void PurgeWaypointsByIcon(string icon)
        {
            PurgeWaypoints(p => p.Title.Equals(icon, StringComparison.InvariantCultureIgnoreCase));
        }

        public void PurgeAll()
        {
            PurgeWaypoints(null);
        }

        public void PurgeWaypointsByColour(string colour)
        {
            PurgeWaypoints(p => p.Color == colour.ColourValue());
        }

        public void PurgeWaypointsByTitle(string partialTitle)
        {
            PurgeWaypoints(p => p.Title.StartsWith(partialTitle, StringComparison.InvariantCultureIgnoreCase));
        }
        
        public void PurgeWaypointsAtPos(BlockPos position)
        {
            PurgeWaypoints(p => p.Position.AsBlockPos.Equals(position));
        }

        /// <summary>
        ///     Returns a list of all waypoints at a given position.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns><c>true</c> if a waypoint already exists at the specified position, <c>false</c> otherwise.</returns>
        public SortedDictionary<int, Waypoint> GetWaypointsAtPos(BlockPos position)
        {
            return GetWaypoints(p => p.Position.AsBlockPos.Equals(position));
        }

        public SortedDictionary<int, Waypoint> GetWaypoints(Func<Waypoint, bool> predicate = null)
        {
            return SortWaypoints(_mapManager.WaypointMapLayer().ownWaypoints);
        }

        public List<Waypoint> GetWaypointsFor(IServerPlayer toPlayer)
        {
            var waypointLayer = _mapManager.WaypointMapLayer();
            var playerGroupMemberships = toPlayer.ServerData.PlayerGroupMemberships;
            return waypointLayer.Waypoints
                .Where(w =>
                    toPlayer.PlayerUID == w.OwningPlayerUid ||
                    playerGroupMemberships.ContainsKey(w.OwningPlayerGroupId))
                .ToList();
        }

        public SortedDictionary<int, Waypoint> SortWaypoints(IList<Waypoint> waypoints, Func<Waypoint, bool> predicate = null)
        {
            var comparer = Comparer<int>.Create((a, b) => b.CompareTo(a));
            var list = new SortedDictionary<int, Waypoint>(comparer);
            for (var i = 0; i < waypoints.Count; i++)
            {
                if (predicate is null || predicate(waypoints[i]))
                {
                    list.Add(i, waypoints[i]);
                }
            }
            return list;
        }

        private void PurgeWaypoints(Func<Waypoint, bool> predicate)
        {
            var waypoints = GetWaypoints(predicate);
            foreach (var waypoint in waypoints)
            {
                _capi.Event.EnqueueMainThreadTask(() => _capi.SendChatMessage($"/waypoint remove {waypoint.Key}"), "");
            }
            _capi.Event.EnqueueMainThreadTask(() =>
                _capi.Event.RegisterCallback(_ =>
                    _mapManager.GetField<IClientNetworkChannel>("clientChannel")
                        .SendPacket(new OnViewChangedPacket()), 500), "");
        }
    }
}
