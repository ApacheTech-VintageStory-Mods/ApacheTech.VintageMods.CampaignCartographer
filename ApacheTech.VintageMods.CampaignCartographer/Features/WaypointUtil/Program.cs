using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Exports;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Imports;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil
{
    /// <summary>
    ///     Feature: Waypoint Utilities
    /// </summary>
    /// <seealso cref="ClientFeatureRegistrar" />
    public class Program : ClientFeatureRegistrar
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterTransient<WaypointExportDialogue>();
            services.RegisterTransient(sp => new ShowConfirmExportDialogue(p => 
                sp.CreateInstance<WaypointExportConfirmationDialogue>(p)));

            services.RegisterTransient<WaypointImportDialogue>();
        }
    }
}
