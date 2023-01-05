using System;
using System.Collections.Generic;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Services.ModMenu.Dialogue;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Registration;
using Gantry.Core.Extensions.GameContent.Gui;
using Gantry.Core.ModSystems;
using JetBrains.Annotations;
using Vintagestory.API.Client;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Services.ModMenu
{
    /// <summary>
    ///     Provides a main GUI for the mod, as a central location to access each feature; rather than through commands.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    [UsedImplicitly]
    public sealed class ModMenu :  ClientModSystem, IClientServiceRegistrar
    {
        /// <summary>
        ///     If you need mods to be executed in a certain order, adjust this methods return value.
        ///     The server will call each Mods Start() method the ascending order of each mods execute order value.
        ///     And thus, as long as every mod registers it's event handlers in the Start() method, all event handlers
        ///     will be called in the same execution order. Default execute order of some survival mod parts.
        /// 
        ///     World Gen: 
        ///     - GenTerra: 0
        ///     - RockStrata: 0.1
        ///     - Deposits: 0.2
        ///     - Caves: 0.3
        ///     - BlockLayers: 0.4
        /// 
        ///     Asset Loading:
        ///     - Json Overrides loader: 0.05
        ///     - Load hardcoded mantle block: 0.1
        ///     - Block and Item Loader: 0.2
        ///     - Recipes (Smithing, Knapping, ClayForming, Grid recipes, Alloys) Loader: 1
        /// </summary>
        /// <returns>A <see cref="double"/>, representing the position of this system, within the order in which mods are loaded by the game.</returns>
        public override double ExecuteOrder() => 0;

        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureClientModServices(IServiceCollection services)
        {
            services.AddTransient<ModMenuDialogue>();
            services.AddTransient<SupportDialogue>();
        }

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="api">The game's core client API.</param>
        public override void StartClientSide(ICoreClientAPI api)
        {
            api.Input.RegisterTransientGuiDialogueHotKey(
                IOC.Services.Resolve<ModMenuDialogue>, LangEx.ModTitle(), GlKeys.F7);
            
            FluentChat.RegisterCommand("wpsettings", api)!
                .WithDescription(LangEx.FeatureString("ManualWaypoints", "SettingsCommandDescription"))
                .WithHandler((_, _, _) => IOC.Services.Resolve<ModMenuDialogue>().ToggleGui());
        }

        /// <summary>
        ///     The dialogue windows, from features within this mod, that will be displayed within the menu.
        /// </summary>
        public Dictionary<Type, string> FeatureDialogues { get; } = new();
    }
}
