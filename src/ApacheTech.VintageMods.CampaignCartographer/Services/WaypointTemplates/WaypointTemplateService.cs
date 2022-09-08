using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using System.Collections.Generic;
using System.Linq;
using Gantry.Core;
using ProperVersion;
using System;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WaypointTemplateService
    {
        private readonly ICoreClientAPI _capi;
        private readonly IFileSystemService _fileSystemService;
        private List<PredefinedWaypointTemplate> _defaultWaypoints;

        private SortedDictionary<string, PredefinedWaypointTemplate> WaypointTemplates { get; } = new();


        public WaypointTemplateService(ICoreClientAPI capi, IFileSystemService fileSystemService)
        {
            _capi = capi;
            _fileSystemService = fileSystemService;
            LoadWaypointTemplates();
        }

        public string GetSyntaxListText()
        {
            return string.Join(" | ", WaypointTemplates.Keys);
        }

        public void LoadWaypointTemplates()
        {
            try
            {
                WaypointTemplates.Clear();

                _defaultWaypoints = _fileSystemService
                    .GetJsonFile("default-waypoints.json")
                    .ParseAsMany<PredefinedWaypointTemplate>()
                    .ToList();

                var installedVersion = SemVer.Parse(_fileSystemService
                    .GetJsonFile("version.data")
                    .ParseAsJsonObject()["Version"]
                    .AsString());

                if (installedVersion < SemVer.Parse(ModEx.ModInfo.Version))
                {
                    var defaultWaypointsFile = _fileSystemService.GetJsonFile("default-waypoints.json");
                    _capi.Logger.VerboseDebug("Campaign Cartographer: Updating global default files.");
                    var globalConfigFile = _fileSystemService.GetJsonFile("version.data");
                    defaultWaypointsFile.DisembedFrom(ModEx.ModAssembly);
                    globalConfigFile.DisembedFrom(GetType().Assembly);
                }

                var waypointsFile = _fileSystemService.GetJsonFile("waypoint-types.json");
                var waypoints = waypointsFile.ParseAsMany<PredefinedWaypointTemplate>();
                WaypointTemplates.AddOrUpdateRange(waypoints.Where(p => p.Enabled), p => p.Key);

                _capi.Logger.Event($"{WaypointTemplates.Count} waypoint extensions loaded.");
            }
            catch (Exception e)
            {
                _capi.Logger.Error($"Waypoint Extensions: Error loading syntax for .wp command; {e.Message}");
                _capi.Logger.Error(e.StackTrace);
            }
        }

        public PredefinedWaypointTemplate GetTemplateByKey(string key)
        {
            if (WaypointTemplates.ContainsKey(key))
            {
                return WaypointTemplates[key].Clone() as PredefinedWaypointTemplate;
            }

            if (_defaultWaypoints is null || _defaultWaypoints.Count == 0) return null;

            return _defaultWaypoints
                .Where(p => p.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                .Select(p => p.DeepClone())
                .FirstOrNull();
        }
    }
}