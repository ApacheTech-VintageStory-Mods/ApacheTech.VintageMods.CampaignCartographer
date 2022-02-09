using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.CampaignCartographer.Infrastructure.Dialogue;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;

namespace ApacheTech.VintageMods.CampaignCartographer.Infrastructure
{
    public class Program : ClientFeatureRegistrar
    {
        public override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterTransient<AddEditWaypointDialogue>();
        }
    }
}
