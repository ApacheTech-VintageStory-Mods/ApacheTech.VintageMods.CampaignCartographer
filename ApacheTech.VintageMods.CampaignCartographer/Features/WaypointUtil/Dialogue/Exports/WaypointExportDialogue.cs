using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
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
    [HarmonySidedPatch(EnumAppSide.Client)]
    public class WaypointExportDialogue : GenericDialogue
    {
        private readonly WaypointService _service;
        private List<WaypointExportCellEntry> _waypoints = new();
        private ElementBounds _clippedBounds;
        private ElementBounds _cellListBounds;
        private GuiElementCellList<WaypointExportCellEntry> _waypointsList;
        private GuiElementDynamicText _lblSelectedCount;
        private static WaypointExportDialogue _instance;

        private readonly string _exportsDirectory = 
            ModPaths.CreateDirectory(Path.Combine(ModPaths.ModDataWorldPath, "Exports"));

        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointExportDialogue" /> class.
        /// </summary>
        /// <param name="capi">Client API pass-through</param>
        /// <param name="service">IOC Injected Waypoint Service.</param>
        public WaypointExportDialogue(ICoreClientAPI capi, WaypointService service) : base(capi)
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

        private void PopulateCellList()
        {
            _waypoints = GetWaypointExportCellEntries();
            var cellList = SingleComposer.GetCellList<WaypointExportCellEntry>("waypointsList");
            cellList.ReloadCells(_waypoints);
        }

        protected override void RefreshValues()
        {
            if (SingleComposer is null) return;

            var cellList = _waypointsList.elementCells.Cast<WaypointExportGuiCell>();
            var count = cellList.AsParallel().Count(p => p.Enabled);

            _cellListBounds.CalcWorldBounds();
            _clippedBounds.CalcWorldBounds();
            _lblSelectedCount.SetNewText(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "SelectedWaypoints", count.FormatLargeNumber()), true);
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
                .FixedSize(EnumDialogArea.CenterFixed, 0, 30)
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

            _waypointsList = new GuiElementCellList<WaypointExportCellEntry>(capi, _cellListBounds, CellCreator, _waypoints);

            _lblSelectedCount =
                new GuiElementDynamicText(capi, "",
                    CairoFont.WhiteDetailText().WithOrientation(EnumTextOrientation.Center),
                    textBounds.FlatCopy().FixedUnder(insetBounds, 10));

            composer
                .AddInset(insetBounds)
                .AddVerticalScrollbar(OnScroll, ElementStdBounds.VerticalScrollbar(insetBounds), "scrollbar")
                .BeginClip(_clippedBounds)
                .AddInteractiveElement(_waypointsList, "waypointsList")
                .EndClip()
                
                .AddSmallButton(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "OpenExportsFolder"),
                    OnOpenExportsFolderButtonPressed, buttonRowBounds.FlatCopy().FixedUnder(insetBounds, 10.0), EnumButtonStyle.Normal,
                    EnumTextOrientation.Center, "btnOpenExportsFolder")

                .AddInteractiveElement(_lblSelectedCount, "lblSelectedCount")
                
                .AddSmallButton(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "ExportSelectedWaypoints"), 
                    OnExportSelectedWaypointButtonPressed, buttonRowBoundsRightFixed.FlatCopy().FixedUnder(insetBounds, 10.0));
        }

        #endregion

        #region Cell List Management

        private IGuiElementCell CellCreator(WaypointExportCellEntry cell, ElementBounds bounds)
        {
            return new WaypointExportGuiCell(ApiEx.Client, cell, bounds)
            {
                OnMouseDownOnCellLeft = OnCellClickLeftSide,
                OnMouseDownOnCellRight = OnCellClickRightSide
            };
        }

        private List<WaypointExportCellEntry> GetWaypointExportCellEntries()
        {
            var waypoints = _service.GetWaypoints();
            return waypoints.Select(w =>
            {
                var dto = WaypointDto.FromWaypoint(w.Value);
                return new WaypointExportCellEntry
                {
                    Title = dto.Title,
                    DetailText = dto.DetailText,
                    Enabled = true,
                    RightTopText = dto.Position.RelativeToSpawn().ToString(),
                    RightTopOffY = 3f,
                    DetailTextFont = CairoFont.WhiteDetailText().WithFontSize((float)GuiStyle.SmallFontSize),
                    Waypoint = dto
                };
            }).ToList();
        }

        #endregion

        #region Control Event Handlers

        private void OnScroll(float dy)
        {
            var bounds = SingleComposer.GetElement("waypointsList").Bounds;
            bounds.fixedY = 0f - dy;
            bounds.CalcWorldBounds();
        }

        private void OnCellClickLeftSide(int val)
        {
            var cell = _waypointsList.elementCells.Cast<WaypointExportGuiCell>().ToList()[val];
            _service.WorldMap.RecentreMap(cell.Waypoint.Position.ToVec3d());
        }

        private void OnCellClickRightSide(int val)
        {
            var cell = _waypointsList.elementCells.Cast<WaypointExportGuiCell>().ToList()[val];
            cell.On = !cell.On;
            cell.Enabled = cell.On;
            RefreshValues();
        }

        private bool OnOpenExportsFolderButtonPressed()
        {
            NetUtil.OpenUrlInBrowser(_exportsDirectory);
            return true;
        }

        private bool OnExportSelectedWaypointButtonPressed()
        {
            var waypoints = _waypointsList.elementCells
                .Cast<WaypointExportGuiCell>()
                .Where(p => p.On)
                .Select(w => w.Waypoint)
                .ToList();
            WaypointExportConfirmationDialogue.ShowDialogue(waypoints);
            return true;
        }

        #endregion
    }
}