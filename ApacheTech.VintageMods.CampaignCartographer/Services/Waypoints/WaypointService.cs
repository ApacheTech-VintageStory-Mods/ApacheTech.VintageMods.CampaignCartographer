using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.FirstRun;
using ApacheTech.VintageMods.CampaignCartographer.Features.FirstRun.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Extensions.System;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
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
    /// <summary>
    ///     Provides mechanisms for managing waypoints.
    /// </summary>
    public class WaypointService
    {
        // DEV NOTE:    What started off as a service with noble intentions, has now become a homunculus.

        private readonly ICoreClientAPI _capi;

        private readonly List<ManualWaypointTemplateModel> _defaultWaypoints;
        public WorldMapManager WorldMap { get; }

        public SortedDictionary<string, ManualWaypointTemplateModel> WaypointTypes { get; } = new();

        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointService" /> class.
        /// </summary>
        /// <param name="capi">The capi.</param>
        public WaypointService(ICoreClientAPI capi)
        {
            _defaultWaypoints = ModServices.FileSystem
                .GetJsonFile("default-waypoints.json")
                .ParseAsMany<ManualWaypointTemplateModel>()
                .ToList();

            WorldMap = (_capi = capi).ModLoader.GetModSystem<WorldMapManager>();
            var worldSettings = ModSettings.World.Feature<GeneralClientWorldSettings>("General");
            var globalSettings = ModSettings.Global.Feature<GeneralClientGlobalSettings>("General");

            if (worldSettings.FirstRun)
            {
                HandleFirstRun(globalSettings, worldSettings);
            }
            LoadWaypoints();
        }

        private void HandleFirstRun(GeneralClientGlobalSettings globalSettings, GeneralClientWorldSettings worldSettings)
        {
            if (globalSettings.AlwaysLoadDefaultWaypoints)
            {
                ModServices.FileSystem
                    .GetJsonFile("waypoint-types.json")
                    .SaveFrom(_defaultWaypoints);
            }
            else if (!globalSettings.NeverLoadDefaultWaypoints)
            {
                OpenFirstRunDialogue(globalSettings);
            }

            worldSettings.FirstRun = false;
            ModSettings.World.Save("General", worldSettings);
        }

        public void OpenFirstRunDialogue(GeneralClientGlobalSettings globalSettings)
        {
            ModServices.IOC.CreateInstance<FirstRunDialogue>(new Action<bool, bool>((state, rememberSetting) =>
            {
                if (!state)
                {
                    globalSettings.NeverLoadDefaultWaypoints = rememberSetting;
                    ModSettings.Global.Save("General", globalSettings);
                    return;
                }

                ModServices.FileSystem
                    .GetJsonFile("waypoint-types.json")
                    .SaveFrom(_defaultWaypoints);

                globalSettings.AlwaysLoadDefaultWaypoints = rememberSetting;
                ModSettings.Global.Save("General", globalSettings);
                LoadWaypoints();
            })).TryOpen();
        }

        public void LoadWaypoints()
        {
            try
            {
                WaypointTypes.Clear();
                if (ModVersion.InstalledVersion("version.data") < ModVersion.ArchiveVersion())
                {
                    var defaultWaypointsFile = ModServices.FileSystem.GetJsonFile("default-waypoints.json");
                    _capi.Logger.VerboseDebug("Campaign Cartographer: Updating global default files.");
                    var globalConfigFile = ModServices.FileSystem.GetJsonFile("version.data");
                    var modAssembly = AssemblyEx.GetModAssembly();
                    defaultWaypointsFile.DisembedFrom(modAssembly);
                    globalConfigFile.DisembedFrom(GetType().Assembly);
                }

                var waypointsFile = ModServices.FileSystem.GetJsonFile("waypoint-types.json");
                var waypoints = waypointsFile.ParseAsMany<ManualWaypointTemplateModel>();
                WaypointTypes.AddOrUpdateRange(waypoints.Where(p => p.Enabled), p => p.Syntax);

                _capi.Logger.Event($"{WaypointTypes.Count} waypoint extensions loaded.");
            }
            catch (Exception e)
            {
                _capi.Logger.Error($"Waypoint Extensions: Error loading syntax for .wp command; {e.Message}");
                _capi.Logger.Error(e.StackTrace);
            }
        }

        public ManualWaypointTemplateModel GetWaypointModel(string syntax)
        {
            if (WaypointTypes.ContainsKey(syntax))
            {
                return WaypointTypes[syntax].Clone() as ManualWaypointTemplateModel;
            }

            if (_defaultWaypoints is null || _defaultWaypoints.Count == 0) return null;

            return _defaultWaypoints
                .Where(p => p.Syntax.Equals(syntax, StringComparison.InvariantCultureIgnoreCase))
                .Select(p => p.Clone() as ManualWaypointTemplateModel)
                .FirstOrNull();
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
            return SortWaypoints(WorldMap.WaypointMapLayer().ownWaypoints.Where(predicate ?? (_ => true)).ToList());
        }

        public Task<SortedDictionary<int, Waypoint>> GetWaypointsAsync(Func<Waypoint, bool> predicate = null)
        {
            return Task.FromResult(GetWaypoints(predicate));
        }

        public List<Waypoint> GetWaypointsFor(IServerPlayer toPlayer)
        {
            var waypointLayer = WorldMap.WaypointMapLayer();
            var playerGroupMemberships = toPlayer.ServerData.PlayerGroupMemberships;
            return waypointLayer.Waypoints
                .Where(w =>
                    toPlayer.PlayerUID == w.OwningPlayerUid ||
                    playerGroupMemberships.ContainsKey(w.OwningPlayerGroupId))
                .ToList();
        }

        public static SortedDictionary<int, Waypoint> SortWaypoints(IList<Waypoint> waypoints, Func<Waypoint, bool> predicate = null)
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
            Task.Factory.StartNew(() =>
            {
                var waypoints = GetWaypoints(predicate);
                foreach (var waypoint in waypoints)
                {
                    _capi.SendChatMessage($"/waypoint remove {waypoint.Key}");
                    Thread.Sleep(50);
                }
                _capi.Event.EnqueueMainThreadTask(() =>
                    _capi.Event.RegisterCallback(_ =>
                        WorldMap.GetField<IClientNetworkChannel>("clientChannel")
                            .SendPacket(new OnViewChangedPacket()), 500), "");
            });
        }

        public void AddWaypoints(IEnumerable<WaypointDto> waypoints)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var dto in waypoints)
                {
                    dto.AddToMap();
                    Thread.Sleep(50);
                }
            });
        }
    }
}
