using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Hosting;
using ApacheTech.VintageMods.Core.Hosting.Configuration.Extensions;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.Core.Services.FileSystem.Enums;
using ApacheTech.VintageMods.Core.Services.MefLab;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

// ReSharper disable UnusedType.Global
// ReSharper disable StringLiteralTypo

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
    internal sealed class Program : ModHost
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected override void ConfigureServerModServices(IServiceCollection services)
        {
            services.RegisterSingleton<IMefLabContractMediator>(sp => sp.Resolve<MefLabMediator>());
        }

        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterSingleton<IMefLabContractMediator>(sp => sp.Resolve<MefLabMediator>());
            services.RegisterSingleton<WaypointService>();
        }

        /// <summary>
        ///     Called on the client, during initial mod loading, called before any mod receives the call to Start().
        /// </summary>
        /// <param name="capi">
        ///     The core API implemented by the client.
        ///     The main interface for accessing the client.
        ///     Contains all sub-components, and some miscellaneous methods.
        /// </param>
        protected override void StartPreClientSide(ICoreClientAPI capi)
        {
            ModServices.FileSystem
                .RegisterSettingsFile("settings-global-client.json", FileScope.Global)
                .RegisterSettingsFile("settings-world-client.json", FileScope.World)
                .RegisterFile("version.data", FileScope.Global);
        }

        /// <summary>
        ///     Called on the server, during initial mod loading, called before any mod receives the call to Start().
        /// </summary>
        /// <param name="sapi">
        ///     The core API implemented by the server.
        ///     The main interface for accessing the server.
        ///     Contains all sub-components, and some miscellaneous methods.
        /// </param>
        protected override void StartPreServerSide(ICoreServerAPI sapi)
        {
            ModServices.FileSystem
                .RegisterFile("version.data", FileScope.Global)
                .RegisterSettingsFile("settings-global-server.json", FileScope.Global);
        }

        /// <summary>
        ///     Side agnostic Start method, called after all mods received a call to StartPre().
        /// </summary>
        /// <param name="api">
        ///     Common API Components that are available on the server and the client.
        ///     Cast to ICoreServerAPI or ICoreClientAPI to access side specific features.
        /// </param>
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            ModServices.Harmony.UseHarmony();
        }

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">
        ///     The core API implemented by the client.
        ///     The main interface for accessing the client.
        ///     Contains all sub-components, and some miscellaneous methods.
        /// </param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            //capi.EnableAsyncTasks();
        }

        /// <summary>
        ///     If this mod allows runtime reloading, you must implement this method to unregister any listeners / handlers
        /// </summary>
        public override void Dispose()
        {
            ApiEx.Run(DisposeClient, DisposeServer);
        }

        private static void DisposeClient()
        {
            FluentChat.DisposeClientCommands();
        }

        private static void DisposeServer()
        {
            FluentChat.DisposeServerCommands();
        }
    }
}