using System;
using System.Collections.Generic;
using System.Linq;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.DotNet;
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
        private GuiElementCellList<PredefinedWaypointsCellEntry> _cellList;
        private string _filterString;
        private bool _disabledTypesOnly;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="PredefinedWaypointsDialogue" /> class.
        /// </summary>
        /// <param name="capi">Client API pass-through</param>
        public PredefinedWaypointsDialogue(ICoreClientAPI capi) : base(capi)
        {
            _file = ModServices.FileSystem.GetJsonFile("waypoint-types.json");
            _waypointTypes.AddOrUpdateRange(_file.ParseAsMany<ManualWaypointTemplateModel>(), w => w.Syntax);
            
            _waypointCells = GetCellEntries();
            Title = LangEntry("Title");
            Alignment = EnumDialogArea.CenterMiddle;

            ClientSettings.Inst.AddWatcher<float>("guiScale", _ =>
            {
                Compose();
                RefreshValues();
            });
        }

        private static string LangEntry(string text, params object[] args)
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
            var scaledHeight = Math.Min(600, (platform.WindowSize.Height - 65) * 0.85) / ClientSettings.GUIScale;

            var outerBounds = ElementBounds
                .Fixed(EnumDialogArea.LeftTop, 0, 0, scaledWidth, 35);

            AddSearchBox(composer, ref outerBounds);

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

            _cellList = new GuiElementCellList<PredefinedWaypointsCellEntry>(capi, _cellListBounds, CellCreator, _waypointCells);

            composer
                .AddInset(insetBounds)
                .AddVerticalScrollbar(OnScroll, ElementStdBounds.VerticalScrollbar(insetBounds), "scrollbar")
                .BeginClip(_clippedBounds)
                .AddInteractiveElement(_cellList, "waypointsList")
                .EndClip()
                .AddSmallButton(LangEntry("LoadDefault"), OnLoadDefaultWaypointTypes, buttonRowBoundsLeftFixed)
                .AddSmallButton(LangEntry("AddNew"), OnAddNewWaypointTypeButtonPressed, buttonRowBoundsRightFixed);
        }

        private void AddSearchBox(GuiComposer composer, ref ElementBounds bounds)
        {
            const int switchSize = 30;
            const int gapBetweenRows = 20;
            var font = CairoFont.WhiteSmallText();
            var lblSearchText = $"{LangEx.CommonPhrase("search")}:";
            var lblDisabledOnlyText = LangEntry("lblDisabledOnly");

            var lblSearchTextLength = font.GetTextExtents(lblSearchText).Width + 10;
            var lblDisabledOnlyTextLength = font.GetTextExtents(lblDisabledOnlyText).Width + 10;

            var left = ElementBounds.Fixed(0, 5, lblSearchTextLength, switchSize).FixedUnder(bounds, 3);
            var right = ElementBounds.Fixed(lblSearchTextLength + 10, 0, 200, switchSize).FixedUnder(bounds, 3);

            composer.AddStaticText(lblSearchText, font, EnumTextOrientation.Left, left);
            composer.AddAutoSizeHoverText(LangEntry("lblSearch.HoverText"), font, 160, left);
            composer.AddTextInput(right, OnFilterTextChanged);

            right = ElementBounds.FixedSize(EnumDialogArea.RightFixed, switchSize, switchSize).FixedUnder(bounds, 3);
            left = ElementBounds.FixedSize(EnumDialogArea.RightFixed, lblDisabledOnlyTextLength, switchSize).FixedUnder(bounds, 8).WithFixedOffset(-40, 0);

            composer.AddStaticText(lblDisabledOnlyText, font, EnumTextOrientation.Left, left);
            composer.AddSwitch(OnCurrentlyPlayingToggle, right);

            bounds = bounds.BelowCopy(fixedDeltaY: gapBetweenRows);
        }

        private void OnCurrentlyPlayingToggle(bool state)
        {
            _disabledTypesOnly = state;
            FilterCells();
            RefreshValues();
        }

        private void OnFilterTextChanged(string filterString)
        {
            _filterString = filterString;
            FilterCells();
            RefreshValues();
        }

        private void FilterCells()
        {
            bool Filter(IGuiElementCell cell)
            {
                var c = (PredefinedWaypointsGuiCell)cell;
                var model = c.Model;
                var state = string.IsNullOrWhiteSpace(_filterString) || 
                            model.Title.ToLowerInvariant().Contains(_filterString.ToLowerInvariant()) || 
                            model.Syntax.ToLowerInvariant().Contains(_filterString.ToLowerInvariant());

                if (_disabledTypesOnly && c.On) state = false;

                return state;
            }

            _cellList.CallMethod("FilterCells", (Func<IGuiElementCell, bool>)Filter);
        }

        private bool OnLoadDefaultWaypointTypes()
        {
            MessageBox.Show(LangEntry("LoadDefault"), LangEntry("LoadDefault.Confirmation"),
                ButtonLayout.OkCancel,
                () =>
                {
                    var waypointTypes = ModServices.FileSystem.GetJsonFile("default-waypoints.json").ParseAsMany<ManualWaypointTemplateModel>();
                    _waypointTypes.AddOrUpdateRange(waypointTypes, w => w.Syntax);
                    _file.SaveFrom(_waypointTypes.Values);
                    _cellList.ReloadCells(_waypointCells = GetCellEntries());
                    FilterCells();
                    RefreshValues();
                });
            return true;
        }

        private bool OnAddNewWaypointTypeButtonPressed()
        {
            var dialogue = ModServices.IOC.CreateInstance<AddEditWaypointTypeDialogue>(new ManualWaypointTemplateModel(), WaypointTypeMode.Add).With(p =>
            {
                p.Title = LangEntry("AddNew");
                p.OnOkAction = AddNewWaypointType;
            });
            dialogue.TryOpen();
            return true;
        }

        private void AddNewWaypointType(ManualWaypointTemplateModel model)
        {
            if (_waypointTypes.ContainsKey(model.Syntax))
            {
                MessageBox.Show(LangEntry("Error"), LangEntry("AddNew.Validation", model.Syntax));
                return;
            }
            _waypointTypes.Add(model.Syntax, model);
            _file.SaveFrom(_waypointTypes.Values);
            _cellList.ReloadCells(_waypointCells = GetCellEntries());
            FilterCells();
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
                return new PredefinedWaypointsCellEntry
                {
                    Title = dto.Title,
                    DetailText = string.Empty,
                    Enabled = true,
                    RightTopText = dto.Syntax,
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
            var cell = _cellList.elementCells.Cast<PredefinedWaypointsGuiCell>().ToList()[val];
            var dialogue = ModServices.IOC.CreateInstance<AddEditWaypointTypeDialogue>(cell.Model, WaypointTypeMode.Edit).With(p =>
            {
                p.Title = LangEntry("Edit");
                p.OnOkAction = EditWaypointType;
                p.OnDeleteAction = DeleteWaypointType;
            });
            dialogue.TryOpen();
        }

        private void DeleteWaypointType(ManualWaypointTemplateModel model)
        {
            var title = LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Delete.Title");
            var message = LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Delete.Message");
            MessageBox.Show(title, message, ButtonLayout.OkCancel,
                () =>
                {
                    if (!_waypointTypes.ContainsKey(model.Syntax))
                    {
                        MessageBox.Show(LangEntry("Error"), LangEntry("Edit.Validation", model.Syntax));
                        return;
                    }
                    _waypointTypes.Remove(model.Syntax);
                    _file.SaveFrom(_waypointTypes.Values);
                    _cellList.ReloadCells(_waypointCells = GetCellEntries());
                    FilterCells();
                    RefreshValues();
                });
        }

        private void EditWaypointType(ManualWaypointTemplateModel model)
        {
            if (!_waypointTypes.ContainsKey(model.Syntax))
            {
                MessageBox.Show(LangEntry("Error"), LangEntry("Edit.Validation", model.Syntax));
                return;
            }
            _waypointTypes[model.Syntax] = model;
            _file.SaveFrom(_waypointTypes.Values);
            _cellList.ReloadCells(_waypointCells = GetCellEntries());
            FilterCells();
            RefreshValues();
        }

        private void OnCellClickRightSide(int val)
        {
            var cell = _cellList.elementCells.Cast<PredefinedWaypointsGuiCell>().ToList()[val];
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