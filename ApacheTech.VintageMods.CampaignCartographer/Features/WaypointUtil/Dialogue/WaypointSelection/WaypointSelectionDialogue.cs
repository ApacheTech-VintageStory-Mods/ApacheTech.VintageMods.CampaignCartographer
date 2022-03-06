using System;
using System.Collections.Generic;
using System.Linq;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Infrastructure.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

// ReSharper disable UnusedMember.Global

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.WaypointSelection
{
    /// <summary>
    ///     Dialogue Window: Allows the user to export waypoints to JSON files.
    /// </summary>
    /// <seealso cref="GenericDialogue" />
    [HarmonySidedPatch(EnumAppSide.Client)]
    public abstract class WaypointSelectionDialogue : GenericDialogue
    {
        private readonly WaypointService _service;
        private ElementBounds _clippedBounds;
        private ElementBounds _cellListBounds;
        private GuiElementDynamicText _lblSelectedCount;
        private static WaypointSelectionDialogue _instance;

        protected List<WaypointSelectionCellEntry> Waypoints { get; set; } = new();

        protected GuiElementCellList<WaypointSelectionCellEntry> WaypointsList { get; private set; }

        protected string LeftButtonText { private get; init; }

        protected string RightButtonText { private get; init; }

        public override bool DisableMouseGrab => true;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointSelectionDialogue" /> class.
        /// </summary>
        /// <param name="capi">Client API pass-through</param>
        /// <param name="service">IOC Injected Waypoint Service.</param>
        protected WaypointSelectionDialogue(ICoreClientAPI capi, WaypointService service) : base(capi)
        {
            _service = service;
            Title = LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "Title");
            Alignment = EnumDialogArea.CenterMiddle;

            ClientSettings.Inst.AddWatcher<float>("guiScale", _ =>
            {
                Compose();
                RefreshValues();
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WaypointMapLayer), "OnDataFromServer")]
        public static void Patch_WaypointMapLayer_OnDataFromServer_PostFix()
        {
            _instance?.PopulateCellList();
        }

        #region Form Composition

        public override bool TryOpen()
        {
            var success = base.TryOpen();
            if (success) _instance = this;
            return success;
        }

        public override bool TryClose()
        {
            var success = base.TryClose();
            if (success) _instance = null;
            return success;
        }

        protected override void Compose()
        {
            base.Compose();
            PopulateCellList();
        }

        protected virtual void PopulateCellList()
        {
            WaypointsList.ReloadCells(Waypoints);
        }

        protected override void RefreshValues()
        {
            if (SingleComposer is null) return;

            var cellList = WaypointsList.elementCells.Cast<WaypointSelectionGuiCell>();
            var count = cellList.Count(p => p.On);
            var selectedCountText = LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "SelectedWaypoints", count.FormatLargeNumber());
            _lblSelectedCount.SetNewText(selectedCountText, true, true);

            _cellListBounds.CalcWorldBounds();
            _clippedBounds.CalcWorldBounds();
            SingleComposer.GetScrollbar("scrollbar").SetHeights((float)_clippedBounds.fixedHeight, (float)_cellListBounds.fixedHeight);
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            var platform = capi.World.GetField<ClientPlatformAbstract>("Platform");
            var scaledWidth = Math.Max(600, platform.WindowSize.Width * 0.5) / ClientSettings.GUIScale;
            var scaledHeight = Math.Max(600, (platform.WindowSize.Height - 65) * 0.85) / ClientSettings.GUIScale;

            var buttonRowBoundsRightFixed = ElementBounds
                .FixedSize(60, 30)
                .WithFixedPadding(10, 2)
                .WithAlignment(EnumDialogArea.RightFixed);

            var buttonRowBounds = ElementBounds
                .FixedSize(60, 30)
                .WithFixedPadding(10, 2);

            var textBounds = ElementBounds
                .FixedSize(EnumDialogArea.CenterFixed, 420, 0)
                .WithFixedPadding(10, 5);

            var outerBounds = ElementBounds
                .Fixed(EnumDialogArea.LeftTop, 0, 0, scaledWidth, 35);

            var insetBounds = outerBounds
                .BelowCopy(0, 3)
                .WithFixedSize(scaledWidth, scaledHeight);

            _clippedBounds = insetBounds
                .ForkContainingChild(3, 3, 3, 3);

            _cellListBounds = _clippedBounds
                .ForkContainingChild(0.0, 0.0, 0.0, -3.0)
                .WithFixedPadding(10.0);

            WaypointsList = new GuiElementCellList<WaypointSelectionCellEntry>(capi, _cellListBounds, CellCreator, Waypoints);


            composer
                .AddInset(insetBounds)
                .AddVerticalScrollbar(OnScroll, ElementStdBounds.VerticalScrollbar(insetBounds), "scrollbar")
                .BeginClip(_clippedBounds)
                .AddInteractiveElement(WaypointsList, "waypointsList")
                .EndClip();

            composer.AddSmallButton(LeftButtonText,
                OnLeftButtonPressed, buttonRowBounds.FlatCopy().FixedUnder(insetBounds, 10.0), EnumButtonStyle.Normal,
                EnumTextOrientation.Center, "btnOpenExportsFolder");

            composer.AddInteractiveElement(
                _lblSelectedCount =
                    new GuiElementDynamicText(capi, "",
                        CairoFont.WhiteDetailText().WithOrientation(EnumTextOrientation.Center),
                        textBounds.FlatCopy().FixedUnder(insetBounds, 10.0)), "lblSelectedCount");

            composer.AddSmallButton(RightButtonText, OnRightButtonPressed, buttonRowBoundsRightFixed.FlatCopy().FixedUnder(insetBounds, 10.0));
        }

        #endregion

        #region Cell List Management

        private IGuiElementCell CellCreator(WaypointSelectionCellEntry cell, ElementBounds bounds)
        {
            return new WaypointSelectionGuiCell(ApiEx.Client, cell, bounds)
            {
                On = cell.Waypoint.Enabled,
                OnMouseDownOnCellLeft = OnCellClickLeftSide,
                OnMouseDownOnCellRight = OnCellClickRightSide
            };
        }

        #endregion

        #region Control Event Handlers

        private void OnScroll(float dy)
        {
            var bounds = SingleComposer.GetElement("waypointsList").Bounds;
            bounds.fixedY = 0f - dy;
            bounds.CalcWorldBounds();
        }

        protected virtual void OnCellClickLeftSide(MouseEvent args, int elementIndex)
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

        protected virtual void DeleteWaypoint(Waypoint waypoint, int index)
        {
            PopulateCellList();
            RefreshValues();
        }

        protected virtual void EditWaypoint(Waypoint waypoint, int index)
        {
            PopulateCellList();
            RefreshValues();
        }

        protected void OnCellClickRightSide(MouseEvent args, int elementIndex)
        {
            var cell = WaypointsList.elementCells.Cast<WaypointSelectionGuiCell>().ToList()[elementIndex];
            cell.On = !cell.On;
            cell.Enabled = cell.On;
            cell.Waypoint.Enabled = cell.On;
            RefreshValues();
        }

        protected virtual bool OnLeftButtonPressed()
        {
            return true;
        }

        protected virtual bool OnRightButtonPressed()
        {
            return true;
        }

        #endregion
    }
}