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
                () => IOC.Services.Resolve<ModMenuDialogue>(), LangEx.ModTitle(), GlKeys.F7);
            
            FluentChat.ClientCommand("wpsettings")
                .RegisterWith(api)
                .HasDescription(LangEx.FeatureString("ManualWaypoints", "SettingsCommandDescription"))
                .HasDefaultHandler((_, _) => IOC.Services.Resolve<ModMenuDialogue>().ToggleGui());
        }

        /// <summary>
        ///     The dialogue windows, from features within this mod, that will be displayed within the menu.
        /// </summary>
        public Dictionary<Type, string> FeatureDialogues { get; } = new();
    }
}
