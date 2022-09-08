using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Commands;
using ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Services.ModMenu.Extensions;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Registration;
using Gantry.Core.Extensions.GameContent.Gui;
using Gantry.Core.ModSystems;
using Gantry.Services.FileSystem.DependencyInjection;
using JetBrains.Annotations;
using Vintagestory.API.Client;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins
{
    /// <summary>
    ///     Client-side entry point for the PlayerPins feature.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PlayerPins : ClientModSystem, IClientServiceRegistrar
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureClientModServices(IServiceCollection services)
        {
            services.AddFeatureWorldSettings<PlayerPinsSettings>();
            services.AddSingleton<PlayerPinsDialogue>();
            services.AddSingleton<FriendClientChatCommand>();
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            FluentChat.ClientCommand("playerpins")
                .RegisterWith(capi)
                .HasDescription(LangEx.FeatureString("PlayerPins", "SettingsCommandDescription"))
                .HasDefaultHandler((_, _) => IOC.Services.Resolve<PlayerPinsDialogue>().ToggleGui());

            IOC.Services.Resolve<FriendClientChatCommand>().Register();

            capi.AddModMenuDialogue<PlayerPinsDialogue>("PlayerPins");
        }
    }
}