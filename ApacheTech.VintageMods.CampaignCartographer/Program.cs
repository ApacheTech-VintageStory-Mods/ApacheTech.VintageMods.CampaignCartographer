using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints;
using ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Broker;
using ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Handlers;
using ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Packets;
using ApacheTech.VintageMods.CampaignCartographer.Services.GUI;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems.Composite;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Hosting.Configuration.Extensions;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.Core.Services.FileSystem.Enums;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.CampaignCartographer
{
    /// <summary>
    ///     Entry-point for the mod. This class will configure and build the IOC Container, and Service list for the rest of the mod.
    ///     
    ///     Registrations performed within this class should be global scope; by convention, features should aim to be as stand-alone as they can be.
    /// </summary>
    /// <seealso cref="ModHost" />
    internal sealed class Program : ModHost
    {
        /// <summary>
        ///     Allows a mod to include Singleton, Scoped, or Transient services to the DI Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected override void ConfigureServerModServices(IServiceCollection services)
        {
            services.RegisterSingleton(_ => ModSettings.Global.Feature<GpsSettings>("GPS"));
            services.RegisterSingleton(_ => ModSettings.Global.Feature<AutoWaypointsSettings>("AutoWaypoints"));
            services.RegisterSingleton<GPSChatCommandBroker>();
            services.RegisterSingleton<WhisperCommandHandler>();
        }

        /// <summary>
        ///     Allows a mod to include Singleton, Scoped, or Transient services to the DI Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterSingleton(_ => ModSettings.World.Feature<AutoWaypointsSettings>("AutoWaypoints"));
            services.RegisterSingleton<TestWindow>();
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
                .RegisterFile("version-installed.json", FileScope.Global)
                .RegisterSettingsFile("settings-global-client.json", FileScope.Global)
                .RegisterSettingsFile("settings-world-client.json", FileScope.World);
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
                .RegisterFile("version-installed.json", FileScope.Global)
                .RegisterSettingsFile("settings-global-server.json", FileScope.Global);

            FluentChat
                .ServerCommand("wpconfig")
                .HasDescription(Lang.Get("wpex:features.wpconfig.server.description"))
                .RequiresPrivilege(Privilege.controlserver)
                .RegisterWith(sapi);
        }

        /// <summary>
        ///     Side agnostic Start method, called after all mods received a call to StartPre().
        /// </summary>
        /// <param name="api">The API.</param>
        public override void Start(ICoreAPI api)
        {
            ModServices.Harmony.UseHarmony();
        }
    }
}