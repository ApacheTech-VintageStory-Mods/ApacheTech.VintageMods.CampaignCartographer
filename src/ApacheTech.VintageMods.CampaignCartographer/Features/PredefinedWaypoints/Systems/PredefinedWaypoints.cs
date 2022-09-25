using System.Collections.Generic;
using System.IO;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Commands;
using ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Dialogue.PredefinedWaypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.ModMenu.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Registration;
using Gantry.Core.ModSystems;
using Gantry.Services.FileSystem;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.DependencyInjection;
using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.Extensions;
using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Systems
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Contains a GUI that can be used to control the settings for the feature.
    ///      • Add a waypoint at the player's current location, via a chat command.
    /// </summary>
    /// <seealso cref="ClientModSystem" />  
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class PredefinedWaypoints : ClientModSystem, IClientServiceRegistrar
    {

        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureClientModServices(IServiceCollection services)
        {
            services.AddFeatureWorldSettings<PredefinedWaypointsSettings>();
            services.AddSingleton<PredefinedWaypointsChatCommand>();
            services.AddTransient<PredefinedWaypointsDialogue>();
            services.AddTransient<EditBlockSelectionWaypointDialogue>();
        }

        /// <summary>
        ///     Called on the client, during initial mod loading, called before any mod receives the call to Start().
        /// </summary>
        /// <param name="capi">
        ///     The core API implemented by the client.
        ///     The main interface for accessing the client.
        ///     Contains all sub-components, and some miscellaneous methods.
        /// </param>
        protected override void StartPreClientSide(ICoreClientAPI capi)
        {
            IOC.Services.Resolve<IFileSystemService>()
                .RegisterFile("trader-colours.json", FileScope.Global)
                .RegisterFile("waypoint-types.json", FileScope.Global);
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            capi.AddModMenuDialogue<PredefinedWaypointsDialogue>("PredefinedWaypoints");
            capi.AddModMenuDialogue<EditBlockSelectionWaypointDialogue>("BlockSelection");
            capi.RegisterCommand(IOC.Services.Resolve<PredefinedWaypointsChatCommand>());
            UpdateWaypointTypesFromWorldFile();
        }

        private void UpdateWaypointTypesFromWorldFile()
        {
            var oldFile = new FileInfo(Path.Combine(ModPaths.ModDataWorldPath, "waypoint-types.json"));
            if (!oldFile.Exists) return;

            var newFile = IOC.Services.Resolve<IFileSystemService>().GetJsonFile("waypoint-types.json");
            var waypointTypes = new SortedDictionary<string, PredefinedWaypointTemplate>();

            waypointTypes.AddOrUpdateRange(newFile.ParseAsMany<PredefinedWaypointTemplate>(), w => w.Key);
            waypointTypes.AddOrUpdateRange(oldFile.ParseAsMany<PredefinedWaypointTemplate>(), w => w.Key);

            newFile.SaveFrom(waypointTypes.Values);
            oldFile.Delete();
        }
    }
}
