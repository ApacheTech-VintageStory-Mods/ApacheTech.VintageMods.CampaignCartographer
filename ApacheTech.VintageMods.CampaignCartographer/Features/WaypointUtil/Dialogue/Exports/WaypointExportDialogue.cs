using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.WaypointSelection;
using ApacheTech.VintageMods.CampaignCartographer.Infrastructure.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Exports
{
    /// <summary>
    ///     Dialogue Window: Allows the user to export waypoints to JSON files.
    /// </summary>
    /// <seealso cref="GenericDialogue" />
    public class WaypointExportDialogue : WaypointSelectionDialogue
    {
        private readonly WaypointService _service;

        private readonly string _exportsDirectory =
            ModPaths.CreateDirectory(Path.Combine(ModPaths.ModDataWorldPath, "Saves"));

        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointExportDialogue" /> class.
        /// </summary>
        /// <param name="capi">Client API pass-through</param>
        /// <param name="service">IOC Injected Waypoint Service.</param>
        public WaypointExportDialogue(ICoreClientAPI capi, WaypointService service) : base(capi, service)
        {
            _service = service;
            Title = LangEx.FeatureString("ManualWaypoints.Dialogue.MenuScreen", "WaypointManager");
            Alignment = EnumDialogArea.CenterMiddle;
            Modal = true;
            ModalTransparency = 0f;
            LeftButtonText = LangEntry("OpenExportsFolder");
            RightButtonText = LangEntry("ExportSelectedWaypoints");
            ShowTopRightButton = true;

            ClientSettings.Inst.AddWatcher<float>("guiScale", _ =>
            {
                Compose();
                RefreshValues();
            });
        }

        #region Form Composition

        protected override void PopulateCellList()
        {
            Waypoints = GetWaypointExportCellEntries();
            base.PopulateCellList();
        }

        #endregion

        #region Cell List Management

        private List<WaypointSelectionCellEntry> GetWaypointExportCellEntries()
        {
            var playerPos = ApiEx.Client.World.Player.Entity.Pos.AsBlockPos;
            var waypoints = _service.GetSortedWaypoints(SortOrder);
            return waypoints.Select(w =>
            {
                var dto = w.Value;
                var current = Waypoints.FirstOrDefault(p => p.Waypoint.Equals(w.Value));
                dto.Enabled = current?.Waypoint.Enabled ?? true;

                return new WaypointSelectionCellEntry
                {
                    Title = dto.Title,
                    DetailText = dto.DetailText,
                    RightTopText = $"{dto.Position.RelativeToSpawn()} ({dto.Position.HorizontalManhattenDistance(playerPos).FormatLargeNumber()}m)",
                    RightTopOffY = 3f,
                    DetailTextFont = CairoFont.WhiteDetailText().WithFontSize((float)GuiStyle.SmallFontSize),
                    Waypoint = dto
                };
            }).ToList();
        }

        #endregion

        #region Control Event Handlers

        protected override void OnCellClickLeftSide(MouseEvent args, int elementIndex)
        {
            var cell = WaypointsList.elementCells.Cast<WaypointSelectionGuiCell>().ToList()[elementIndex];
            _service.WorldMap.RecentreMap(cell.Waypoint.Position.ToVec3d());
            if (args.Button != EnumMouseButton.Right) return;

            var dialogue = ModServices.IOC.CreateInstance<AddEditWaypointDialogue>(
                cell.Waypoint.ToWaypoint(), WaypointTypeMode.Edit, elementIndex).With(p =>
                {
                    p.OnOkAction = EditWaypoint;
                    p.OnDeleteAction = DeleteWaypoint;
                });
            dialogue.TryOpen();
        }

        protected override void DeleteWaypoint(Waypoint waypoint, int index)
        {
            capi.SendChatMessage($"/waypoint remove {index}");
            _service.WorldMap.ForceSendWaypoints();
            capi.RegisterDelayedCallback(_ =>
            {
                PopulateCellList();
                RefreshValues();
            }, 500);
        }

        protected override void EditWaypoint(Waypoint waypoint, int index)
        {
            var colour = NamedColour.FromArgb(waypoint.Color);
            capi.SendChatMessage($"/waypoint modify {index} {colour} {waypoint.Icon} {waypoint.Pinned} {waypoint.Title}");
            _service.WorldMap.ForceSendWaypoints();
            capi.RegisterDelayedCallback(_ =>
            {
                PopulateCellList();
                RefreshValues();
            }, 500);
        }

        protected override bool OnLeftButtonPressed()
        {
            NetUtil.OpenUrlInBrowser(_exportsDirectory);
            return true;
        }

        protected override bool OnRightButtonPressed()
        {
            var waypoints = WaypointsList.elementCells
                .Cast<WaypointSelectionGuiCell>()
                .Where(p => p.On)
                .Select(w => w.Waypoint)
                .ToList();
            WaypointExportConfirmationDialogue.ShowDialogue(waypoints);
            return true;
        }

        #endregion
    }
}