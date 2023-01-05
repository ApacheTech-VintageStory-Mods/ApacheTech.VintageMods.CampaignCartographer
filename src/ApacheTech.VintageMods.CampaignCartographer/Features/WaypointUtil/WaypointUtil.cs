using System;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Services.FirstRun;
using ApacheTech.VintageMods.CampaignCartographer.Services.Repositories;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Registration;
using Gantry.Core.ModSystems;
using Gantry.Services.FileSystem.Configuration.Abstractions;
using Gantry.Services.FileSystem.DependencyInjection;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil
{
    /// <summary>
    ///     Feature: Waypoint Utilities (.wpUtil)
    ///      • Contains a GUI that can be used to control the settings for the feature.
    ///      • Export all waypoints.
    ///      • Import waypoints from file.
    ///      • Remove all waypoints.
    ///      • Remove all waypoints within a given radius of the player.
    ///      • Remove all waypoints with a specified colour.
    ///      • Remove all waypoints with a specified icon.
    ///      • Remove all waypoints where the title starts with a specified string.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class WaypointUtil : ClientModSystem, IClientServiceRegistrar
    {
        private WaypointCommandsRepository _commandRepo;

        private Action _cachedAction;
        private ICoreClientAPI _capi;

        private static string Confirm => LangEx.FeatureString("WaypointUtil", "Confirm");
        private static string ConfirmationMessage => LangEx.FeatureString("WaypointUtil", "ConfirmationMessage", Confirm);

        public void ConfigureClientModServices(IServiceCollection services)
        {
            services.AddFeatureWorldSettings<WaypointUtilSettings>();
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            _capi = capi;
            _commandRepo = IOC.Services.Resolve<WaypointCommandsRepository>();

            FluentChat.RegisterCommand("wputil", capi)!
                .WithDescription(LangEx.FeatureString("WaypointUtil", "SettingsCommandDescription"))
                .HasSubCommand("purge-all", s => s.WithHandler(OnPurgeAll).Build())
                .HasSubCommand("purge-nearby", s => s.WithHandler(OnPurgeNearby).Build())
                .HasSubCommand("purge-icon", s => s.WithHandler(OnPurgeByIcon).Build())
                .HasSubCommand("purge-colour", s => s.WithHandler(OnPurgeByColour).Build())
                .HasSubCommand("purge-title", s => s.WithHandler(OnPurgeByTitle).Build())
                .HasSubCommand(Confirm, s => s.WithHandler(OnConfirmation).Build())
                .HasSubCommand("cancel", s => s.WithHandler(OnCancel).Build())
                .HasSubCommand("reset", s => s.WithHandler(OnFactoryReset).Build())
                .HasSubCommand("silent", s => s.WithHandler(OnSilent).Build());
        }

        /// <summary>
        ///      • Silence waypoint user feedback.
        /// </summary>
        private void OnSilent(IPlayer player, int groupId, CmdArgs args)
        {
            IOC.Services.Resolve<WaypointUtilSettings>().SilenceFeedback = args.PopBool().GetValueOrDefault(false);
        }

        /// <summary>
        ///      • Reset mod to factory settings.
        /// </summary>
        private void OnFactoryReset(IPlayer player, int groupId, CmdArgs args)
        {
            _cachedAction = () =>
            {
                IOC.Services.Resolve<FirstRun>().ResetToFactorySettings();
            };
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints.
        /// </summary>
        private void OnPurgeAll(IPlayer player, int groupId, CmdArgs args)
        {
            _cachedAction = () => _commandRepo.RemoveAll();
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints within a given radius of the player.
        /// </summary>
        private void OnPurgeNearby(IPlayer player, int groupId, CmdArgs args)
        {
            var radius = args.PopInt().GetValueOrDefault(10);
            _cachedAction = () => _commandRepo.RemoveNearPlayer(radius);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints with a specified icon.
        /// </summary>
        private void OnPurgeByIcon(IPlayer player, int groupId, CmdArgs args)
        {
            var icon = args.PopWord();
            _cachedAction = () => _commandRepo.RemoveByIcon(icon);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints with a specified colour.
        /// </summary>
        private void OnPurgeByColour(IPlayer player, int groupId, CmdArgs args)
        {
            var colour = args.PopWord();
            _cachedAction = () => _commandRepo.RemoveByColour(colour);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints where the title starts with a specified string.
        /// </summary>
        private void OnPurgeByTitle(IPlayer player, int groupId, CmdArgs args)
        {
            var partialTitle = args.PopWord();
            _cachedAction = () => _commandRepo.RemoveByTitle(partialTitle);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///     Confirm choice, and remove selected waypoints.
        /// </summary>
        private void OnConfirmation(IPlayer player, int groupId, CmdArgs args)
        {
            _cachedAction?.Invoke();
            _cachedAction = null;
        }

        /// <summary>
        ///     Cancels the currently cached action.
        /// </summary>
        private void OnCancel(IPlayer player, int groupId, CmdArgs args)
        {
            _cachedAction = null;
        }
    }

    public class WaypointUtilSettings : FeatureSettings
    {
        public bool SilenceFeedback { get; set; }
    }
}
