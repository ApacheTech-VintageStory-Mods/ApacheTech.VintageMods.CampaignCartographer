using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil
{
    public class Program : ClientFeatureRegistrar
    {
        public override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterSingleton<OpenFolderDialogue>();
            services.RegisterSingleton<WaypointExportDialogue>();
            services.RegisterSingleton<WaypointImportDialogue>();
        }
    }
}
