using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS
{
    /// <summary>
    ///     Feature: GPS
    /// </summary>
    /// <seealso cref="ServerFeatureRegistrar" />
    public class Program : ServerFeatureRegistrar
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public override void ConfigureServerModServices(IServiceCollection services)
        {
            services.RegisterSingleton(_ => ModSettings.Global.Feature<GpsSettings>("GPS"));
        }
    }
}
