using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.System;
using ApacheTech.VintageMods.Core.GameContent.GUI;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.Core.Services.FileSystem.Abstractions.Contracts;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue.PredefinedWaypoints
{
    /// <summary>
    ///     GUI window that enables players to manage pre-defined waypoint types that can be added to the world map, through chat.
    /// </summary>
    /// <seealso cref="GenericDialogue" />
    public class PredefinedWaypointsDialogue : GenericDialogue
    {
        private List<PredefinedWaypointsCellEntry> _waypointCells;
        private readonly SortedDictionary<string, ManualWaypointTemplateModel> _waypointTypes = new();
        private readonly IJsonModFile _file;
        private ElementBounds _clippedBounds;
        private ElementBounds _cellListBounds;
        private GuiElementCellList<PredefinedWaypointsCellEntry> _waypointsList;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="PredefinedWaypointsDialogue" /> class.
        /// </summary>
        /// <param name="capi">Client API pass-through</param>
        public PredefinedWaypointsDialogue(ICoreClientAPI capi) : base(capi)
        {
            _file = ModServices.FileSystem.GetJsonFile("waypoint-types.json");
            _waypointTypes.AddOrUpdateRange(_file.ParseAsMany<ManualWaypointTemplateModel>(), w => w.Syntax);
            _waypointCells = GetCellEntries();
            Title = GetText("Title");
            Alignment = EnumDialogArea.CenterMiddle;

            ClientSettings.Inst.AddWatcher<float>("guiScale", _ =>
            {
                Compose();
                RefreshValues();
            });
        }

        private static string GetText(string text, params object[] args)
        {
            return LangEx.FeatureString("ManualWaypoints.Dialogue.PreDefinedWaypoints", text, args);
        }

        #region Form Composition

        protected override void Compose()
        {
            base.Compose();
            var cellList = SingleComposer.GetCellList<PredefinedWaypointsCellEntry>("waypointsList");
            cellList.ReloadCells(_waypointCells);
        }

        protected override void RefreshValues()
        {
            if (SingleComposer is null) return;
            _cellListBounds.CalcWorldBounds();
            _clippedBounds.CalcWorldBounds();
            SingleComposer.GetScrollbar("scrollbar").SetHeights((float)_clippedBounds.fixedHeight, (float)_cellListBounds.fixedHeight);
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            var platform = capi.World.GetField<ClientPlatformAbstract>("Platform");
            var scaledWidth = Math.Max(600, platform.WindowSize.Width * 0.5) / ClientSettings.GUIScale;
            var scaledHeight = Math.Max(600, (platform.WindowSize.Height - 65) * 0.85) / ClientSettings.GUIScale;

            var outerBounds = ElementBounds
                .Fixed(EnumDialogArea.LeftTop, 0, 0, scaledWidth, 35);

            var insetBounds = outerBounds
                .BelowCopy(0, 3)
                .WithFixedSize(scaledWidth, scaledHeight);

            var buttonRowBoundsLeftFixed = ElementBounds
                .FixedSize(150, 30)
                .WithFixedPadding(10, 2)
                .WithAlignment(EnumDialogArea.LeftFixed)
                .FixedUnder(insetBounds, 10.0);

            var buttonRowBoundsRightFixed = ElementBounds
                .FixedSize(150, 30)
                .WithFixedPadding(10, 2)
                .WithAlignment(EnumDialogArea.RightFixed)
                .FixedUnder(insetBounds, 10.0);

            _clippedBounds = insetBounds
                .ForkContainingChild(3, 3, 3, 3);

            _cellListBounds = _clippedBounds
                .ForkContainingChild(0.0, 0.0, 0.0, -3.0)
                .WithFixedPadding(10.0);

            _waypointsList = new GuiElementCellList<PredefinedWaypointsCellEntry>(capi, _cellListBounds, CellCreator, _waypointCells);

            composer
                .AddInset(insetBounds)
                .AddVerticalScrollbar(OnScroll, ElementStdBounds.VerticalScrollbar(insetBounds), "scrollbar")
                .BeginClip(_clippedBounds)
                .AddInteractiveElement(_waypointsList, "waypointsList")
                .EndClip()
                .AddSmallButton(GetText("LoadDefault"), OnLoadDefaultWaypointTypes, buttonRowBoundsLeftFixed)
                .AddSmallButton(GetText("AddNew"), OnAddNewWaypointTypeButtonPressed, buttonRowBoundsRightFixed);
        }

        private bool OnLoadDefaultWaypointTypes()
        {
            MessageBox.Show(GetText("LoadDefault"), GetText("LoadDefault.Confirmation"),
                ButtonLayout.OkCancel,
                () =>
                {
                    var waypointTypes = ModServices.FileSystem.GetJsonFile("default-waypoints.json").ParseAsMany<ManualWaypointTemplateModel>();
                    _waypointTypes.AddOrUpdateRange(waypointTypes, w => w.Syntax);
                    _file.SaveFrom(_waypointTypes.Values);
                    _waypointsList.ReloadCells(_waypointCells = GetCellEntries());
                    RefreshValues();
                });
            return true;
        }

        private bool OnAddNewWaypointTypeButtonPressed()
        {
            var dialogue = ModServices.IOC.CreateInstance<AddEditWaypointTypeDialogue>(new ManualWaypointTemplateModel(), WaypointTypeMode.Add).With(p =>
            {
                p.Title = GetText("AddNew");
                p.OnOkAction = AddNewWaypointType;
            });
            dialogue.TryOpen();
            return true;
        }

        private void AddNewWaypointType(ManualWaypointTemplateModel model)
        {
            if (_waypointTypes.ContainsKey(model.Syntax))
            {
                MessageBox.Show(GetText("Error"), GetText("AddNew.Validation", model.Syntax));
                return;
            }
            _waypointTypes.Add(model.Syntax, model);
            _file.SaveFrom(_waypointTypes.Values);
            _waypointsList.ReloadCells(_waypointCells = GetCellEntries());
            RefreshValues();
        }

        #endregion

        #region Cell List Management

        private IGuiElementCell CellCreator(PredefinedWaypointsCellEntry cell, ElementBounds bounds)
        {
            return new PredefinedWaypointsGuiCell(ApiEx.Client, cell, bounds)
            {
                On = cell.Model.Enabled,
                OnMouseDownOnCellLeft = OnCellClickLeftSide,
                OnMouseDownOnCellRight = OnCellClickRightSide
            };
        }

        private List<PredefinedWaypointsCellEntry> GetCellEntries()
        {
            if (!_waypointTypes.Any()) return new List<PredefinedWaypointsCellEntry>();
            return _waypointTypes.Select(kvp =>
            {
                var dto = kvp.Value;
                var detailText = new StringBuilder();
                detailText.AppendLine(GetText("CellTitle", dto.Title));
                detailText.AppendLine(GetText("CellRadius", dto.HorizontalCoverageRadius, dto.VerticalCoverageRadius));
                detailText.AppendLine(GetText("CellPinned", dto.Pinned));

                return new PredefinedWaypointsCellEntry
                {
                    Title = dto.Syntax,
                    DetailText = detailText.ToString(),
                    Enabled = true,
                    RightTopText = string.Empty,
                    RightTopOffY = 3f,
                    DetailTextFont = CairoFont.WhiteDetailText().WithFontSize((float)GuiStyle.SmallFontSize),
                    Model = dto
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
            var cell = _waypointsList.elementCells.Cast<PredefinedWaypointsGuiCell>().ToList()[val];
            var dialogue = ModServices.IOC.CreateInstance<AddEditWaypointTypeDialogue>(cell.Model, WaypointTypeMode.Edit).With(p =>
            {
                p.Title = GetText("Edit");
                p.OnOkAction = EditWaypointType;
                p.OnDeleteAction = DeleteWaypointType;
            });
            dialogue.TryOpen();
        }

        private void DeleteWaypointType(ManualWaypointTemplateModel model)
        {
            if (!_waypointTypes.ContainsKey(model.Syntax))
            {
                MessageBox.Show(GetText("Error"), GetText("Edit.Validation", model.Syntax));
                return;
            }
            _waypointTypes.Remove(model.Syntax);
            _file.SaveFrom(_waypointTypes.Values);
            _waypointsList.ReloadCells(_waypointCells = GetCellEntries());
            RefreshValues();
        }

        private void EditWaypointType(ManualWaypointTemplateModel model)
        {
            if (!_waypointTypes.ContainsKey(model.Syntax))
            {
                MessageBox.Show(GetText("Error"), GetText("Edit.Validation", model.Syntax));
                return;
            }
            _waypointTypes[model.Syntax] = model;
            _file.SaveFrom(_waypointTypes.Values);
            _waypointsList.ReloadCells(_waypointCells = GetCellEntries());
            RefreshValues();
        }

        private void OnCellClickRightSide(int val)
        {
            var cell = _waypointsList.elementCells.Cast<PredefinedWaypointsGuiCell>().ToList()[val];
            cell.On = !cell.On;
            cell.Enabled = cell.On;
            cell.Model.Enabled = cell.On;
            RefreshValues();
        }

        #endregion

        public override bool TryClose()
        {
            _file.SaveFrom(_waypointTypes.Values);
            ModServices.IOC.Resolve<WaypointService>().LoadWaypoints();
            return base.TryClose();
        }
    }
}