using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;
using Vintagestory.API.Client;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Feature: Auto Waypoints
    /// </summary>
    /// <seealso cref="ClientFeatureRegistrar" />
    public class AutoWaypointsProgram : ClientFeatureRegistrar
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterSingleton(_ => ModSettings.World.Feature<AutoWaypointsSettings>("AutoWaypoints"));
            services.RegisterSingleton<Dialogue.AutoWaypointsDialogue>();
            services.RegisterSingleton<AutoWaypointPatchHandler>();
        }

        /// <summary>
        ///     Called on the client, during initial mod loading, called before any mod receives the call to Start().
        /// </summary>
        /// <param name="capi">
        ///     The core API implemented by the client.
        ///     The main interface for accessing the client.
        ///     Contains all sub-components, and some miscellaneous methods.
        /// </param>
        public override void StartPreClientSide(ICoreClientAPI capi)
        {
            Patches.AutoWaypointsPatches.Initialise();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Patches.AutoWaypointsPatches.Dispose();
            base.Dispose();
        }
    }
}
