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
using Gantry.Services.FileSystem.DependencyInjection;
using Gantry.Services.FileSystem.Features;
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
        private WaypointQueriesRepository _queryRepo;
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
            _queryRepo = IOC.Services.Resolve<WaypointQueriesRepository>();
            _commandRepo = IOC.Services.Resolve<WaypointCommandsRepository>();

            FluentChat.ClientCommand("wputil")
                .RegisterWith(capi)
                .HasDescription(LangEx.FeatureString("WaypointUtil", "SettingsCommandDescription"))
                .HasSubCommand("purge-all").WithHandler(OnPurgeAll)
                .HasSubCommand("purge-nearby").WithHandler(OnPurgeNearby)
                .HasSubCommand("purge-icon").WithHandler(OnPurgeByIcon)
                .HasSubCommand("purge-colour").WithHandler(OnPurgeByColour)
                .HasSubCommand("purge-title").WithHandler(OnPurgeByTitle)
                .HasSubCommand(Confirm).WithHandler(OnConfirmation)
                .HasSubCommand("cancel").WithHandler(OnCancel)
                .HasSubCommand("reset").WithHandler(OnFactoryReset)
                .HasSubCommand("silent").WithHandler(OnSilent);
        }

        /// <summary>
        ///      • Silence waypoint user feedback.
        /// </summary>
        private void OnSilent(string subCommandName, int groupId, CmdArgs args)
        {
            IOC.Services.Resolve<WaypointUtilSettings>().SilenceFeedback = args.PopBool().GetValueOrDefault(false);
        }

        /// <summary>
        ///      • Reset mod to factory settings.
        /// </summary>
        private void OnFactoryReset(string subCommandName, int groupId, CmdArgs args)
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
        private void OnPurgeAll(string subCommandName, int groupId, CmdArgs args)
        {
            _cachedAction = () => _commandRepo.RemoveAll();
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints within a given radius of the player.
        /// </summary>
        private void OnPurgeNearby(string subCommandName, int groupId, CmdArgs args)
        {
            var radius = args.PopInt().GetValueOrDefault(10);
            _cachedAction = () => _commandRepo.RemoveNearPlayer(radius);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints with a specified icon.
        /// </summary>
        private void OnPurgeByIcon(string subCommandName, int groupId, CmdArgs args)
        {
            var icon = args.PopWord();
            _cachedAction = () => _commandRepo.RemoveByIcon(icon);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints with a specified colour.
        /// </summary>
        private void OnPurgeByColour(string subCommandName, int groupId, CmdArgs args)
        {
            var colour = args.PopWord();
            _cachedAction = () => _commandRepo.RemoveByColour(colour);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints where the title starts with a specified string.
        /// </summary>
        private void OnPurgeByTitle(string subCommandName, int groupId, CmdArgs args)
        {
            var partialTitle = args.PopWord();
            _cachedAction = () => _commandRepo.RemoveByTitle(partialTitle);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///     Confirm choice, and remove selected waypoints.
        /// </summary>
        private void OnConfirmation(string subCommandName, int groupId, CmdArgs args)
        {
            _cachedAction?.Invoke();
            _cachedAction = null;
        }

        /// <summary>
        ///     Cancels the currently cached action.
        /// </summary>
        private void OnCancel(string subCommandName, int groupId, CmdArgs args)
        {
            _cachedAction = null;
        }
    }

    public class WaypointUtilSettings : FeatureSettings
    {
        public bool SilenceFeedback { get; set; }
    }
}
