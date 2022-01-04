using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.Core.Services.FileSystem.Enums;
using Vintagestory.API.Client;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints
{
    public class Program : ClientFeatureRegistrar
    {
        public override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterSingleton(sp => sp.CreateInstance<Commands.ManualWaypointsChatCommand>());
        }

        public override void StartPreClientSide(ICoreClientAPI capi)
        {
            ModServices.FileSystem
                .RegisterFile("default-waypoints.json", FileScope.Global)
                .RegisterFile("custom-waypoints.json", FileScope.World);
        }
    }
}
