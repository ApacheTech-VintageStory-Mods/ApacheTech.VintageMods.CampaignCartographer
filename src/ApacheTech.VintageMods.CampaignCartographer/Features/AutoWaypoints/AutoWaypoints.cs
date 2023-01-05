using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Services.ModMenu.Extensions;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Registration;
using Gantry.Core.Extensions.GameContent.Gui;
using Gantry.Core.ModSystems;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.DependencyInjection;
using Gantry.Services.FileSystem.Enums;
using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Automatic Waypoint Addition (.wpAuto)
    ///      - Contains a GUI that can be used to control the settings for the feature (Shift + F7).
    ///      - Enable / Disable all automatic waypoint placements.
    ///      - Automatically add waypoints for Translocators, as the player travels between them.
    ///      - Automatically add waypoints for Teleporters, as the player travels between them.
    ///      - Automatically add waypoints for Traders, as the player interacts with them.
    ///      - Automatically add waypoints for Meteors, when the player punches a Meteoric Iron Block.
    ///      - Server: Send Teleporter information to clients, when creating Teleporter waypoints.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    [UsedImplicitly]
    public sealed class AutoWaypoints : ClientModSystem, IClientServiceRegistrar
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureClientModServices(IServiceCollection services)
        {
            services.AddFeatureWorldSettings<AutoWaypointsSettings>();
            services.AddSingleton<AutoWaypointsDialogue>();
            services.AddSingleton<AutoWaypointPatchHandler>();
        }

        /// <summary>
        ///     Called during initial mod loading, called before any mod receives the call to Start().
        /// </summary>
        /// <param name="capi"></param>
        protected override void StartPreClientSide(ICoreClientAPI capi)
        {
            IOC.Services
                .Resolve<IFileSystemService>()
                .RegisterFile("crossmap.json", FileScope.Global);
        }

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The client-side API.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            capi.Event.LevelFinalize += () =>
            {
                FluentChat.RegisterCommand("wpAuto", capi)!
                    .WithDescription(LangEx.FeatureString("AutoWaypoints", "SettingsCommandDescription"))
                    .WithHandler((_, _, _) => IOC.Services.Resolve<AutoWaypointsDialogue>().ToggleGui());

                capi.AddModMenuDialogue<AutoWaypointsDialogue>("AutoWaypoints");
            };
        }
    }
}