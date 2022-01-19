using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.CampaignCartographer.Features.FirstRun.Dialogue;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;
// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.FirstRun
{
    /// <summary>
    ///     Registers types for the FirstRun feature with the IOC container.
    /// </summary>
    /// <seealso cref="ClientFeatureRegistrar" />
    public sealed class Program : ClientFeatureRegistrar
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterTransient<FirstRunDialogue>();
        }
    }
}
