using System;
using System.Threading;
using System.Threading.Tasks;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Exports;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Imports;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.Core.Abstractions;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedVariable
// ReSharper disable RedundantUsingDirective
// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedType.Global

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
    public sealed class WaypointUtil : ClientModSystem
    {
        private WaypointService _service;

        private Action _cachedAction;
        private ICoreClientAPI _capi;

        private static string Confirm => LangEx.FeatureString("WaypointUtil", "Confirm");
        private static string ConfirmationMessage => LangEx.FeatureString("WaypointUtil", "ConfirmationMessage", Confirm);

        public override void StartClientSide(ICoreClientAPI capi)
        {
            _capi = capi;
            _service = ModServices.IOC.Resolve<WaypointService>();

            var command = FluentChat.ClientCommand("wputil")
                .RegisterWith(capi)
                .HasDescription(LangEx.FeatureString("WaypointUtil", "SettingsCommandDescription"))
                .HasSubCommand("export").WithHandler(OnExport)
                .HasSubCommand("import").WithHandler(OnImport)
                .HasSubCommand("purge-all").WithHandler(OnPurgeAll)
                .HasSubCommand("purge-nearby").WithHandler(OnPurgeNearby)
                .HasSubCommand("purge-icon").WithHandler(OnPurgeByIcon)
                .HasSubCommand("purge-colour").WithHandler(OnPurgeByColour)
                .HasSubCommand("purge-title").WithHandler(OnPurgeByTitle)
                .HasSubCommand(Confirm).WithHandler(OnConfirmation)
                .HasSubCommand("cancel").WithHandler(OnCancel);
#if DEBUG
            command.HasSubCommand("stress-test").WithHandler(OnStressTest);
#else
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
#endif
        }

        private static void OnStressTest(string subCommandName, int groupId, CmdArgs args)
        {
            Task.Factory.StartNew(() =>
            {
                using var _ = TimedOperation.Profile("Stress Test");
                var random = new Random();
                for (var i = 0; i < 10000; i++)
                {
                    var blockPos = new BlockPos(
                        random.Next(-1000, 1000) + 500000, 
                        random.Next(1,256),
                        random.Next(-1000, 1000) + 500000);
                    blockPos.AddWaypointAtPos("star1", "red", $"Waypoint Stress Test: {i}", false, true);
                    Thread.Sleep(50);
                }
            });
        }

        /// <summary>
        ///      • Export all waypoints.
        /// </summary>
        private static void OnExport(string subCommandName, int groupId, CmdArgs args)
        {
            var dialogue = ModServices.IOC.Resolve<WaypointExportDialogue>();
            while (dialogue.IsOpened(dialogue.ToggleKeyCombinationCode))
                dialogue.TryClose();
            dialogue.TryOpen();
        }

        /// <summary>
        ///      • Import waypoints from file.
        /// </summary>
        private static void OnImport(string subCommandName, int groupId, CmdArgs args)
        {
            var dialogue = ModServices.IOC.Resolve<WaypointImportDialogue>();
            while (dialogue.IsOpened(dialogue.ToggleKeyCombinationCode))
                dialogue.TryClose();
            dialogue.TryOpen();
        }

        /// <summary>
        ///      • Remove all waypoints.
        /// </summary>
        private void OnPurgeAll(string subCommandName, int groupId, CmdArgs args)
        {
            _cachedAction = () => _service.Purge.All();
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints within a given radius of the player.
        /// </summary>
        private void OnPurgeNearby(string subCommandName, int groupId, CmdArgs args)
        {
            var radius = args.PopFloat().GetValueOrDefault(10f);
            _cachedAction = () => _service.Purge.NearPlayer(radius);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints with a specified icon.
        /// </summary>
        private void OnPurgeByIcon(string subCommandName, int groupId, CmdArgs args)
        {
            var icon = args.PopWord();
            _cachedAction = () => _service.Purge.ByIcon(icon);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints with a specified colour.
        /// </summary>
        private void OnPurgeByColour(string subCommandName, int groupId, CmdArgs args)
        {
            var colour = args.PopWord();
            _cachedAction = () => _service.Purge.ByColour(colour);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints where the title starts with a specified string.
        /// </summary>
        private void OnPurgeByTitle(string subCommandName, int groupId, CmdArgs args)
        {
            var partialTitle = args.PopWord();
            _cachedAction = () => _service.Purge.ByTitle(partialTitle);
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
}
