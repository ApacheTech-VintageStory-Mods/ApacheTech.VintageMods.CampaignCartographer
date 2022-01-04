using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.FluentChatCommands;
using Newtonsoft.Json;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

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
    /// <seealso cref="ApacheTech.VintageMods.Core.Abstractions.ModSystems.ClientModSystem" />
    public sealed class WaypointUtil : ClientModSystem
    {
        private WaypointService _service;

        private Action _cachedAction;
        private ICoreClientAPI _capi;

        private const string Confirm = "confirm"; // LangEx.FeatureString("WaypointUtil", "Confirm")
        private const string ConfirmationMessage = "Type `.wpUtil {0}` to confirm your choice."; // LangEx.FeatureCode("WaypointUtil", "ConfirmationMessage")

        public override void StartClientSide(ICoreClientAPI capi)
        {
            _capi = capi;
            _service = ModServices.IOC.Resolve<WaypointService>();
            

            capi.Input.RegisterHotKey("ofd", "Open File Dialogue", GlKeys.F7, HotkeyType.GUIOrOtherControls);
            capi.Input.SetHotKeyHandler("ofd", _ => new OpenFolderDialogue(capi).TryOpen());


            FluentChat.ClientCommand("wputil").RegisterWith(capi)
                .HasDescription(LangEx.FeatureString("WaypointUtil", "Description"))
                .HasSubCommand("export").WithHandler(OnExport)
                .HasSubCommand("import").WithHandler(OnImport)
                .HasSubCommand("purge-all").WithHandler(OnPurgeAll)
                .HasSubCommand("purge-nearby").WithHandler(OnPurgeNearby)
                .HasSubCommand("purge-icon").WithHandler(OnPurgeByIcon)
                .HasSubCommand("purge-colour").WithHandler(OnPurgeByColour)
                .HasSubCommand("purge-title").WithHandler(OnPurgeByTitle)
                .HasSubCommand("confirm").WithHandler(OnConfirmation)
                .HasSubCommand("cancel").WithHandler(OnCancel);
        }

        /// <summary>
        ///      • Export all waypoints.
        /// </summary>
        private void OnExport(string subCommandName, int groupId, CmdArgs args)
        {
            var waypoints = _service.GetWaypoints().Values;
            var exportList = waypoints.Select(WaypointDto.FromWaypoint).ToList();
            var json = JsonConvert.SerializeObject(exportList, Formatting.Indented);
            File.WriteAllText(@"C:\waypoints.json", json);
        }

        /// <summary>
        ///      • Import waypoints from file.
        /// </summary>
        private void OnImport(string subCommandName, int groupId, CmdArgs args)
        {
            var option = args.PopWord("");
            var json = File.ReadAllText(@"C:\waypoints.json");
            var waypoints = JsonConvert.DeserializeObject<List<WaypointDto>>(json);

            if (option.Equals("purge", StringComparison.InvariantCultureIgnoreCase))
            {
                _service.PurgeAll();
            }
            var replace = option.Equals("replace", StringComparison.InvariantCultureIgnoreCase);

            foreach (var w in waypoints)
            {
                if (replace) _service.PurgeWaypointsAtPos(w.Position);
                w.Position.AddWaypointAtPos(w.Icon, w.Colour, w.Title, w.Pinned);
            }
        }

        /// <summary>
        ///      • Remove all waypoints.
        /// </summary>
        private void OnPurgeAll(string subCommandName, int groupId, CmdArgs args)
        {
            _cachedAction = () => _service.PurgeAll();
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints within a given radius of the player.
        /// </summary>
        private void OnPurgeNearby(string subCommandName, int groupId, CmdArgs args)
        {
            var radius = args.PopFloat().GetValueOrDefault(10f);
            _cachedAction = () => _service.PurgeWaypointsNearby(radius);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints with a specified icon.
        /// </summary>
        private void OnPurgeByIcon(string subCommandName, int groupId, CmdArgs args)
        {
            var icon = args.PopWord();
            _cachedAction = () => _service.PurgeWaypointsByIcon(icon);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints with a specified colour.
        /// </summary>
        private void OnPurgeByColour(string subCommandName, int groupId, CmdArgs args)
        {
            var colour = args.PopWord();
            _cachedAction = () => _service.PurgeWaypointsByColour(colour);
            _capi.ShowChatMessage(string.Format(ConfirmationMessage, Confirm));
        }

        /// <summary>
        ///      • Remove all waypoints where the title starts with a specified string.
        /// </summary>
        private void OnPurgeByTitle(string subCommandName, int groupId, CmdArgs args)
        {
            var partialTitle = args.PopWord();
            _cachedAction = () => _service.PurgeWaypointsByTitle(partialTitle);
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
