using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins
{
    public class Program : ClientFeatureRegistrar
    {
        public override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterSingleton(_ => ModSettings.World.Feature<PlayerPinsSettings>("PlayerPins"));
            services.RegisterSingleton<Dialogue.PlayerPinsDialogue>();
        }
    }
}
