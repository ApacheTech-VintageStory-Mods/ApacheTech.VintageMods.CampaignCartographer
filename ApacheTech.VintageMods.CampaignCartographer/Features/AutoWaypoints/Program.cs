using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    public class Program : ClientFeatureRegistrar
    {
        public override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterSingleton(_ => ModSettings.World.Feature<AutoWaypointsSettings>("AutoWaypoints"));
            services.RegisterSingleton<Dialogue.AutoWaypointsDialogue>();
        }
    }
}
