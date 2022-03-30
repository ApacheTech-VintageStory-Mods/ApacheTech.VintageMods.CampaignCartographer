using System.Collections.Generic;
using System.Linq;
using ApacheTech.Common.DependencyInjection.Annotation;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.WaypointSelection;
using ApacheTech.VintageMods.CampaignCartographer.Infrastructure.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.GameContent.GUI;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Imports
{
    /// <summary>
    ///     Dialogue Window: Allows the user to export waypoints to JSON files.
    /// </summary>
    /// <seealso cref="GenericDialogue" />
    public class WaypointImportConfirmationDialogue : WaypointSelectionDialogue
    {
        private readonly WaypointService _service;
        private readonly List<WaypointDto> _waypoints;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointImportConfirmationDialogue" /> class.
        /// </summary>
        /// <param name="capi">Client API pass-through</param>
        /// <param name="service">IOC Injected Waypoint Service.</param>
        /// <param name="waypoints"></param>
        [ServiceProviderConstructor]
        public WaypointImportConfirmationDialogue(ICoreClientAPI capi, WaypointService service, List<WaypointDto> waypoints) : base(capi, service)
        {
            _service = service;
            _waypoints = waypoints;
            Title = LangEx.FeatureString("WaypointUtil.Dialogue.ImportConfirmation", "Title");
            Alignment = EnumDialogArea.CenterMiddle;
            Modal = true;
            ModalTransparency = 0f;
            LeftButtonText = LangEx.FeatureString("WaypointUtil.Dialogue.ImportConfirmation", "LeftButtonText");
            RightButtonText = LangEx.FeatureString("WaypointUtil.Dialogue.ImportConfirmation", "RightButtonText");
            ShowTopRightButton = false;

            ClientSettings.Inst.AddWatcher<float>("guiScale", _ =>
            {
                Compose();
                RefreshValues();
            });
        }

        public static WaypointImportConfirmationDialogue Create(List<WaypointDto> waypoints)
        {
            return ModServices.IOC.CreateInstance<WaypointImportConfirmationDialogue>(waypoints);
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
            return _waypoints.Select(dto => new WaypointSelectionCellEntry
            {
                Title = dto.Title,
                DetailText = dto.DetailText,
                RightTopText = dto.Position.RelativeToSpawn().ToString(),
                RightTopOffY = 3f,
                DetailTextFont = CairoFont.WhiteDetailText().WithFontSize((float)GuiStyle.SmallFontSize),
                Waypoint = dto
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
            var title = LangEx.FeatureString("WaypointUtil.Dialogue.ImportConfirmation", "Delete.Title");
            var message = LangEx.FeatureString("WaypointUtil.Dialogue.ImportConfirmation", "Delete.Message");
            MessageBox.Show(title, message, ButtonLayout.OkCancel,
                () =>
                {
                    _waypoints.RemoveAt(index);
                    PopulateCellList();
                    RefreshValues();
                });
        }

        protected override void EditWaypoint(Waypoint waypoint, int index)
        {
            _waypoints[index] = WaypointDto.FromWaypoint(waypoint);
            PopulateCellList();
            RefreshValues();
        }

        protected override bool OnLeftButtonPressed()
        {
            var dialogue = ModServices.IOC.Resolve<WaypointImportDialogue>();
            while (dialogue.IsOpened(dialogue.ToggleKeyCombinationCode))
                dialogue.TryClose();
            dialogue.TryOpen();
            return TryClose();
        }

        protected override bool OnRightButtonPressed()
        {
            var waypoints = WaypointsList.elementCells      
                .Cast<WaypointSelectionGuiCell>()
                .Where(p => p.On)
                .Select(w => w.Waypoint)
                .ToList();

            var code = LangEx.FeatureCode("WaypointUtil.Dialogue.Imports", "File");
            var pluralisedFile = LangEx.Pluralise(code, waypoints.Count);
            var totalCount = waypoints.Count;

            var title = LangEx.FeatureString("WaypointUtil.Dialogue.ImportConfirmation", "ConfirmationTitle");
            var message = LangEx.FeatureString("WaypointUtil.Dialogue.ImportConfirmation", "ConfirmationMessage",
                totalCount.FormatLargeNumber(), pluralisedFile);

            MessageBox.Show(title, message, ButtonLayout.OkCancel, () =>
            {
                _service.AddWaypoints(waypoints);
                TryClose();
            });
            return true;
        }

        #endregion
    }
}