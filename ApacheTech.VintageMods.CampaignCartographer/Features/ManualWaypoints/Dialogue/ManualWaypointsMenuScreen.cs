using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue.PredefinedWaypoints;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.GameContent.GUI;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

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
        }

        public override bool DisableMouseGrab => true;

        /// <summary>
        ///     Fires when the GUI is opened.
        /// </summary>
        public override void OnGuiOpened()
        {
            if (capi.IsSinglePlayer && !ApiEx.ClientMain.GetField<bool>("OpenedToLan"))
            {
                ApiEx.ClientMain.PauseGame(true);
            }
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            const double width = 400.0;
            const double height = 200.0;
            
            var squareBounds = ElementBounds.FixedSize(EnumDialogArea.CenterTop, width, height).WithFixedOffset(0, 30); 
            
            composer
                .AddStaticImage(squareBounds, AssetLocation.Create("campaigncartographer:textures/dialogue/menu-logo.png"))
                .AddSmallButton(LangEx.FeatureString("ManualWaypoints.Dialogue.MenuScreen", "EditBlockSelectionWaypointMarker"), OnEditBlockSelectionMarkerButtonPressed, ButtonBounds(0.5f, width, height))
                .AddSmallButton(LangEx.FeatureString("ManualWaypoints.Dialogue.MenuScreen", "EditPreDefinedWaypoints"), OnEditPreDefinedWaypointsPressed, ButtonBounds(1.0f, width, height))
                .AddSmallButton(LangEx.FeatureString("ManualWaypoints.Dialogue.MenuScreen", "DonateToModAuthor"), OnDonateButtonPressed, ButtonBounds(2.0f, width, height))
                .AddSmallButton(Lang.Get("pause-back2game"), TryClose, ButtonBounds(2.5f, width, height));
        }

        private static ElementBounds ButtonBounds(float offset, double width, double height)
        {
            return ElementStdBounds
                .MenuButton(offset)
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

        /// <summary>
        ///     Attempts to close this dialogue- triggering the OnCloseDialogue event.
        /// </summary>
        /// <returns>Was this dialogue successfully closed?</returns>
        public override bool TryClose()
        {
            if (capi.IsSinglePlayer && !ApiEx.ClientMain.GetField<bool>("OpenedToLan"))
            {
                ApiEx.ClientMain.PauseGame(false);
            }
            return base.TryClose();
        }
    }
}
