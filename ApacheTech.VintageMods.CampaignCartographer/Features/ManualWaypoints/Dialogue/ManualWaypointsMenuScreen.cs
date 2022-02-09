using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue.PredefinedWaypoints;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Exports;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Imports;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.GameContent.GUI;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

// ReSharper disable AccessToModifiedClosure
// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue
{
    /// <summary>
    ///     Manual Waypoints main menu screen.
    /// </summary>
    /// <seealso cref="GenericDialogue" />
    public sealed class ManualWaypointsMenuScreen : GenericDialogue
    {
        public ManualWaypointsMenuScreen(ICoreClientAPI capi) : base(capi)
        {
            Title = LangEx.FeatureString("ManualWaypoints.Dialogue.MenuScreen", "Title");
            Alignment = EnumDialogArea.CenterMiddle;
            ShowTitleBar = false;
            Modal = true;
            ModalTransparency = 0f;
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            const double width = 400.0;
            const double height = 200.0;
            
            var squareBounds = ElementBounds.FixedSize(EnumDialogArea.CenterTop, width, height).WithFixedOffset(0, 30);
            var row = 0f;

            composer
                .AddStaticImage(squareBounds, AssetLocation.Create("campaigncartographer:textures/dialogue/menu-logo.png"))
                .AddSmallButton(LangEx.FeatureString("ManualWaypoints.Dialogue.MenuScreen", "EditBlockSelectionWaypointMarker"), OnEditBlockSelectionMarkerButtonPressed, ButtonBounds(ref row, width, height))
                .AddSmallButton(LangEx.FeatureString("ManualWaypoints.Dialogue.MenuScreen", "EditPreDefinedWaypoints"), OnEditPreDefinedWaypointsPressed, ButtonBounds(ref row, width, height))
                .AddSmallButton(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "Title"), OnExportWaypointButtonPressed, ButtonBounds(ref row, width, height))
                .AddSmallButton(LangEx.FeatureString("WaypointUtil.Dialogue.Imports", "Title"), OnImportWaypointButtonPressed, ButtonBounds(ref row, width, height))
                .Execute(() => row += 0.5f)
                .AddSmallButton(LangEx.FeatureString("ManualWaypoints.Dialogue.MenuScreen", "DonateToModAuthor"), OnDonateButtonPressed, ButtonBounds(ref row, width, height))
                .AddSmallButton(Lang.Get("pause-back2game"), TryClose, ButtonBounds(ref row, width, height));
        }

        private static bool OnImportWaypointButtonPressed()
        {
            var dialogue = ModServices.IOC.Resolve<WaypointImportDialogue>();
            while (dialogue.IsOpened(dialogue.ToggleKeyCombinationCode))
                dialogue.TryClose();
            return dialogue.TryOpen();
        }

        private static bool OnExportWaypointButtonPressed()
        {
            var dialogue = ModServices.IOC.Resolve<WaypointExportDialogue>();
            while (dialogue.IsOpened(dialogue.ToggleKeyCombinationCode))
                dialogue.TryClose();
            return dialogue.TryOpen();
        }

        private static ElementBounds ButtonBounds(ref float row, double width, double height)
        {
            row += 0.5f;
            return ElementStdBounds
                .MenuButton(row)
                .WithFixedOffset(0, height)
                .WithFixedSize(width, 30);
        }

        private bool OnDonateButtonPressed()
        {
            capi.Gui.OpenLink("https://bit.ly/APGDonate");
            return true;
        }

        private static bool OnEditPreDefinedWaypointsPressed()
        {
            var dialogue = ModServices.IOC.Resolve<PredefinedWaypointsDialogue>();
            while (dialogue.IsOpened(dialogue.ToggleKeyCombinationCode))
                dialogue.TryClose();
            dialogue.TryOpen();
            return true;
        }

        private static bool OnEditBlockSelectionMarkerButtonPressed()
        {
            var settings = ModSettings.World.Feature<ManualWaypointsSettings>("ManualWaypoints");
            var dialogue = EditBlockSelectionWaypointDialogue.Create(settings.BlockSelectionWaypointTemplate ?? new BlockSelectionWaypointTemplate()).With(p =>
            {
                p.Title = "Edit Block Selection Waypoint Marker";
                p.OnOkAction = waypointTemplate =>
                {
                    settings.BlockSelectionWaypointTemplate = waypointTemplate;
                    ModSettings.World.Save("ManualWaypoints", settings);
                };
            });
            dialogue.TryOpen();
            return true;
        }
    }
}
