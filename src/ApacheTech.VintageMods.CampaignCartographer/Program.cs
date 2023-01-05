using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Services.Repositories;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.Extensions.GameContent;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.DependencyInjection;
using Gantry.Services.FileSystem.Enums;
using Gantry.Services.HarmonyPatches.DependencyInjection;
using Gantry.Services.Network.DependencyInjection;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer
{
    /// <summary>
    ///     Entry-point for the mod. This class will configure and build the IOC Container, and Service list for the rest of the mod.
    ///     
    ///     Registrations performed within this class should be global scope; by convention, features should aim to be as stand-alone as they can be.
    /// </summary>
    /// <remarks>
    ///     Only one derived instance of this class should be added to any single mod within
    ///     the VintageMods domain. This class will enable Dependency Injection, and add all
    ///     of the domain services. Derived instances should only have minimal functionality, 
    ///     instantiating, and adding Application specific services to the IOC Container.
    /// </remarks>
    /// <seealso cref="ModHost" />
    [UsedImplicitly]
    internal sealed class Program : ModHost
    {
        protected override void ConfigureUniversalModServices(IServiceCollection services)
        {
            services.AddFileSystemService(o => o.RegisterSettingsFiles = true);
            services.AddHarmonyPatchingService(o => o.AutoPatchModAssembly = true);
            services.AddNetworkService();
#if DEBUG
            HarmonyLib.Harmony.DEBUG = true;
#endif
        }

        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected override void ConfigureClientModServices(IServiceCollection services)
        {
            // TODO: ROADMAP: This may be better to push to a separate mod system.
            services.AddProprietaryModSystem<WorldMapManager>();
            services.AddProprietaryModSystem<IWorldMapManager, WorldMapManager>();

            services.AddSingleton(ioc => ioc.Resolve<WorldMapManager>().WaypointMapLayer());
            
            services.AddSingleton<WaypointTemplateService>();
            services.AddSingleton<WaypointCommandsRepository>();
            services.AddSingleton<WaypointQueriesRepository>();
        }

        /// <summary>
        ///     Called during initial mod loading, called before any mod receives the call to Start().
        /// </summary>
        /// <param name="api">
        ///     Common API Components that are available on the server and the client.
        ///     Cast to ICoreServerAPI or ICoreClientAPI to access side specific features.
        /// </param>
        public override void StartPreUniversalSide(ICoreAPI api)
        {
            IOC.Services.Resolve<IFileSystemService>()
                .RegisterFile("version.data", FileScope.Global);
        }

        /// <summary>
        ///     If this mod allows runtime reloading, you must implement this method to unregister any listeners / handlers
        /// </summary>
        public override void Dispose()
        {
            FluentChat.ClearCommands(ApiEx.Current);
        }
    }
}