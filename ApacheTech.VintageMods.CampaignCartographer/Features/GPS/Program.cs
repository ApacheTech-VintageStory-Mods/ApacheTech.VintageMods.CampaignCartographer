using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS
{
    public class Program : ServerFeatureRegistrar
    {
        public override void ConfigureServerModServices(IServiceCollection services)
        {
            services.RegisterSingleton(_ => ModSettings.Global.Feature<GpsSettings>("GPS"));
            services.RegisterSingleton<Dialogue.GpsAdminDialogue>();
        }
    }
}
