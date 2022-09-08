using System.Collections.Generic;
using System.Linq;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.WaypointSelection;
using ApacheTech.VintageMods.CampaignCartographer.Services.Repositories;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Annotation;
using Gantry.Core.Extensions;
using Gantry.Core.GameContent.GUI;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.Imports
{
    /// <summary>
    ///     Dialogue Window: Allows the user to export waypoints to JSON files.
    /// </summary>
    /// <seealso cref="GenericDialogue" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WaypointImportConfirmationDialogue : WaypointSelectionDialogue
    {
        private readonly WaypointQueriesRepository _queriesRepo;
        private readonly List<PositionedWaypointTemplate> _waypoints;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointImportConfirmationDialogue" /> class.
        /// </summary>
        /// <param name="capi">Client API pass-through</param>
        /// <param name="queriesRepo">IOC Injected Waypoint Query Repository.</param>
        /// <param name="waypoints"></param>
        [SidedConstructor]
        public WaypointImportConfirmationDialogue(
            ICoreClientAPI capi, WaypointQueriesRepository queriesRepo, List<PositionedWaypointTemplate> waypoints) : base(capi, queriesRepo)
        {
            _queriesRepo = queriesRepo;
            _waypoints = waypoints;
            Title = LangEx.FeatureString("WaypointManager.Dialogue.ImportConfirmation", "Title");
            Alignment = EnumDialogArea.CenterMiddle;
            Modal = true;
            ModalTransparency = 0f;
            LeftButtonText = LangEx.FeatureString("WaypointManager.Dialogue.ImportConfirmation", "LeftButtonText");
            RightButtonText = LangEx.FeatureString("WaypointManager.Dialogue.ImportConfirmation", "RightButtonText");
            ShowTopRightButton = false;

            ClientSettings.Inst.AddWatcher<float>("guiScale", _ =>
            {
                Compose();
                RefreshValues();
            });
        }

        public static WaypointImportConfirmationDialogue Create(List<PositionedWaypointTemplate> waypoints)
        {
            return IOC.Services.CreateInstance<WaypointImportConfirmationDialogue>(waypoints);
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
            var waypoints = _queriesRepo.SortWaypoints(_waypoints, SortOrder);
            return waypoints.Select(dto => new WaypointSelectionCellEntry
            {
                Title = dto.Value.Title,
                RightTopText = $"{dto.Value.Position.AsBlockPos.RelativeToSpawn()} ({dto.Value.Position.AsBlockPos.HorizontalManhattenDistance(playerPos).FormatLargeNumber()}m)",
                RightTopOffY = 3f,
                DetailTextFont = CairoFont.WhiteDetailText().WithFontSize((float)GuiStyle.SmallFontSize),
                Model = SelectableWaypointTemplate.FromWaypoint(dto.Value),
                Index = dto.Key
            }).ToList();
        }

        #endregion

        #region Control Event Handlers

        protected override bool OnLeftButtonPressed()
        {
            var dialogue = IOC.Services.Resolve<WaypointImportDialogue>();
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
                .Select(w => w.Model.With(x => x.Position = x.Position.AsBlockPos.RelativeToSpawn().ToVec3d()))
                .ToList();

            var code = LangEx.FeatureCode("WaypointManager.Dialogue.Imports", "File");
            var pluralisedFile = LangEx.Pluralise(code, waypoints.Count);
            var totalCount = waypoints.Count;

            var title = LangEx.FeatureString("WaypointManager.Dialogue.ImportConfirmation", "ConfirmationTitle");
            var message = LangEx.FeatureString("WaypointManager.Dialogue.ImportConfirmation", "ConfirmationMessage",
                totalCount.FormatLargeNumber(), pluralisedFile);

            MessageBox.Show(title, message, ButtonLayout.OkCancel, () =>
            {
                IOC.Services
                    .Resolve<WaypointCommandsRepository>()
                    .AddWaypoints(waypoints);
                TryClose();
            });
            return true;
        }

        #endregion
    }
}