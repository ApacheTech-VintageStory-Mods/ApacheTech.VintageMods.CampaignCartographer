using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins
{
    /// <summary>
    ///     Feature: Player Pins
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
            services.RegisterSingleton(_ => ModSettings.World.Feature<PlayerPinsSettings>("PlayerPins"));
            services.RegisterSingleton<Dialogue.PlayerPinsDialogue>();
        }
    }
}
