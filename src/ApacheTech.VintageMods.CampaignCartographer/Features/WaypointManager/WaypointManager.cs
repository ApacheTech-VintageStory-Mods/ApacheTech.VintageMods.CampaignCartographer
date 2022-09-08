using System.Linq;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.Exports;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.Imports;
using ApacheTech.VintageMods.CampaignCartographer.Services.ModMenu.Extensions;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Registration;
using Gantry.Core.GameContent.AssetEnum;
using Gantry.Core.ModSystems;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class WaypointManager : ClientModSystem, IClientServiceRegistrar
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureClientModServices(IServiceCollection services)
        {
            services.AddTransient<WaypointExportDialogue>();
            services.AddTransient(ioc => new ShowConfirmExportDialogue(p =>
                ioc.CreateInstance<WaypointExportConfirmationDialogue>(p)));
            services.AddTransient<WaypointImportDialogue>();
        }

        protected override void StartPreClientSide(ICoreClientAPI capi)
        {
            capi.Event.BlockTexturesLoaded += () =>
            {
                capi.AddModMenuDialogue<WaypointExportDialogue>("WaypointManager");
            };

            capi.Event.LevelFinalize += () =>
            {
                IOC.Services.Resolve<WaypointMapLayer>().WaypointColors =
                    NamedColour
                        .ValuesList()
                        .Select(x => x.ToColour().ToArgb())
                        .ToList();
            };
        }
    }
}