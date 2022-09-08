using System;
using System.Collections.Generic;
using System.Linq;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Services.FirstRun.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Registration;
using Gantry.Core.ModSystems;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.DependencyInjection;
using Gantry.Services.FileSystem.Enums;
using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.FirstRun
{
    /// <summary>
    ///     Registers types for the FirstRun feature with the IOC container.
    /// </summary>
    /// <seealso cref="IClientServiceRegistrar" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class FirstRun : ClientModSystem, IClientServiceRegistrar
    {
        private FirstRunWorldSettings _worldSettings;
        private FirstRunGlobalSettings _globalSettings;
        private IFileSystemService _fileSystemService;
        private List<PredefinedWaypointTemplate> _defaultWaypoints;

        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureClientModServices(IServiceCollection services)
        {
            services.AddTransient<FirstRunDialogue>();
            services.AddFeatureGlobalSettings<FirstRunGlobalSettings>();
            services.AddFeatureWorldSettings<FirstRunWorldSettings>();
        }

        protected override void StartPreClientSide(ICoreClientAPI capi)
        {
            _fileSystemService = IOC.Services.Resolve<IFileSystemService>()
                .RegisterFile("default-waypoints.json", FileScope.Global);
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            _globalSettings = IOC.Services.Resolve<FirstRunGlobalSettings>();
            _worldSettings = IOC.Services.Resolve<FirstRunWorldSettings>();

            if (_worldSettings.FirstRun) HandleFirstRun();
        }

        private void HandleFirstRun()
        {
            _worldSettings.FirstRun = false;
            if (_globalSettings.NeverLoadDefaultWaypoints) return;

            _defaultWaypoints = _fileSystemService
                .GetJsonFile("default-waypoints.json")
                .ParseAsMany<PredefinedWaypointTemplate>()
                .ToList();

            if (!_globalSettings.AlwaysLoadDefaultWaypoints)
            {
                OpenFirstRunDialogue();
                return;
            }

            SaveDefaultWaypointsToDisk();
        }

        public void ResetToFactorySettings()
        {
            _worldSettings.FirstRun = true;

            _globalSettings.NeverLoadDefaultWaypoints = false;
            _globalSettings.AlwaysLoadDefaultWaypoints = false;

            _defaultWaypoints.Clear();

            _fileSystemService
                .GetJsonFile("waypoint-types.json")
                .SaveFrom(_defaultWaypoints);

            HandleFirstRun();
        }


        private void SaveDefaultWaypointsToDisk()
        {
            _fileSystemService
                .GetJsonFile("waypoint-types.json")
                .SaveFrom(_defaultWaypoints);
        }

        public void OpenFirstRunDialogue()
        {
            IOC.Services.CreateInstance<FirstRunDialogue>(new Action<bool, bool>((loadWaypoints, rememberSettings) =>
            {
                if (!loadWaypoints)
                {
                    _globalSettings.NeverLoadDefaultWaypoints = rememberSettings;
                    return;
                }
                _globalSettings.AlwaysLoadDefaultWaypoints = rememberSettings;

                SaveDefaultWaypointsToDisk();
            })).TryOpen();
        }
    }
}
