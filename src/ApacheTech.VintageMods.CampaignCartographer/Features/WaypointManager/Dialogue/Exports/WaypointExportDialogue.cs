using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.WaypointSelection;
using ApacheTech.VintageMods.CampaignCartographer.Services.Repositories;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Gantry.Core;
using Gantry.Core.Extensions;
using Gantry.Core.GameContent.GUI;
using Gantry.Services.FileSystem;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Util;
using Vintagestory.Client.NoObf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.Exports
{
    /// <summary>
    ///     Dialogue Window: Allows the user to export waypoints to JSON files.
    /// </summary>
    /// <seealso cref="GenericDialogue" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WaypointExportDialogue : WaypointSelectionDialogue
    {
        private readonly WaypointQueriesRepository _queriesRepo;

        private readonly string _exportsDirectory =
            ModPaths.CreateDirectory(Path.Combine(ModPaths.ModDataWorldPath, "Saves"));

        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointExportDialogue" /> class.
        /// </summary>
        /// <param name="capi">Client API pass-through</param>
        /// <param name="queriesRepo">IOC Injected Waypoint Service.</param>
        public WaypointExportDialogue(ICoreClientAPI capi, WaypointQueriesRepository queriesRepo) : base(capi, queriesRepo)
        {
            _queriesRepo = queriesRepo;
            Title = LangEx.FeatureString("WaypointManager.Dialogue", "Title");
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
            var waypoints = _queriesRepo.GetSortedWaypoints(SortOrder, SelectableWaypointTemplate.FromWaypoint);
            return waypoints.Select(w =>
            {
                var dto = w.Value;
                var current = Waypoints.FirstOrDefault(p => p.Model.Equals(w.Value));
                dto.Selected = current?.Model.Selected ?? true;

                return new WaypointSelectionCellEntry
                {
                    Title = dto.Title,
                    RightTopText = $"{dto.Position.AsBlockPos.RelativeToSpawn()} ({dto.Position.AsBlockPos.HorizontalManhattenDistance(playerPos).FormatLargeNumber()}m)",
                    RightTopOffY = 3f,
                    DetailTextFont = CairoFont.WhiteDetailText().WithFontSize((float)GuiStyle.SmallFontSize),
                    Model = dto,
                    Index = w.Key
                };
            }).ToList();
        }

        #endregion

        #region Control Event Handlers

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
                .Select(w => (PositionedWaypointTemplate)w.Model)
                .ToList();
            WaypointExportConfirmationDialogue.ShowDialogue(waypoints);
            return true;
        }

        #endregion
    }
}