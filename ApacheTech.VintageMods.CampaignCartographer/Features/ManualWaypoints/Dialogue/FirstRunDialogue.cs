using System;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.GameContent.GUI;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue
{
    public sealed class FirstRunDialogue : GenericDialogue
    {
        private bool _rememberSettings;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="FirstRunDialogue"/> class.
        /// </summary>
        /// <param name="capi">ClientAPI Pass-through.</param>
        /// <param name="onReturnAction">The action to invoke once the form is closed.</param>
        public FirstRunDialogue(ICoreClientAPI capi, Action<bool, bool> onReturnAction) : base(capi)
        {
            Alignment = EnumDialogArea.CenterMiddle;
            ShowTitleBar = false;
            OnReturnAction = onReturnAction;
        }

        /// <summary>
        ///     Fires when the GUI is opened.
        /// </summary>
        public override void OnGuiOpened()
        {
            if (!capi.IsSinglePlayer || ApiEx.ClientMain.GetField<bool>("OpenedToLan")) return;
            ApiEx.ClientMain.PauseGame(true);
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            const double width = 400;
            const double height = 200;

            var squareBounds = ElementBounds.FixedSize(EnumDialogArea.CenterTop, width, height).WithFixedOffset(0, 30);

            composer
                .AddImage(squareBounds, AssetLocation.Create("campaigncartographer:textures/dialogue/menu-logo.png"))
                .AddStaticText(
                    LangEx.FeatureString("ManualWaypoints.Dialogue.FirstRun", "Paragraph1"),
                    CairoFont
                        .WhiteDetailText()
                        .WithOrientation(EnumTextOrientation.Center),
                    ButtonBounds(0.5f, width, height)
                        .FlatCopy()
                        .WithAlignment(EnumDialogArea.CenterFixed))
                .AddStaticText(
                    LangEx.FeatureString("ManualWaypoints.Dialogue.FirstRun", "Paragraph2"),
                    CairoFont.WhiteDetailText()
                        .WithOrientation(EnumTextOrientation.Center),
                    ButtonBounds(1.5f, width, height).FlatCopy().WithAlignment(EnumDialogArea.CenterFixed))
                .AddStaticText(
                    LangEx.FeatureString("ManualWaypoints.Dialogue.FirstRun", "RememberPreference"),
                    CairoFont.WhiteDetailText().WithOrientation(EnumTextOrientation.Right),
                    ButtonBounds(2.5f, width - 45, height).FlatCopy().WithAlignment(EnumDialogArea.LeftFixed).WithFixedOffset(45, 5))
                .AddSwitch(OnRememberSettingsChanged, ButtonBounds(2.5f, 30, height).FlatCopy().WithAlignment(EnumDialogArea.LeftFixed).WithFixedOffset(5, 0))
                .AddSmallButton(LangEx.GetCore("confirmation-no"), OnNo, ButtonBounds(3.2f, 150, height).FlatCopy().WithAlignment(EnumDialogArea.LeftFixed))
                .AddSmallButton(LangEx.GetCore("confirmation-yes"), OnYes, ButtonBounds(3.2f, 150, height).FlatCopy().WithAlignment(EnumDialogArea.RightFixed));
        }

        private void OnRememberSettingsChanged(bool state)
        {
            _rememberSettings = state;
        }

        private static ElementBounds ButtonBounds(float offset, double width, double height)
        {
            return ElementStdBounds
                .MenuButton(offset)
                .WithFixedOffset(0, height)
                .WithFixedSize(width, 30);
        }

        private Action<bool, bool> OnReturnAction { get; }

        private bool OnNo()
        {
            var title = LangEx.FeatureString("ManualWaypoints.Dialogue.FirstRun", "Title");
            var message = LangEx.FeatureString("ManualWaypoints.Dialogue.FirstRun", "CancelMessage");
            MessageBox.Show(title, message, ButtonLayout.OkCancel, () => OnReturnAction(false, _rememberSettings));
            return TryClose();
        }

        private bool OnYes()
        {
            var title = LangEx.FeatureString("ManualWaypoints.Dialogue.FirstRun", "Title");
            var message = LangEx.FeatureString("ManualWaypoints.Dialogue.FirstRun", "ConfirmMessage");
            MessageBox.Show(title, message, ButtonLayout.OkCancel, () => OnReturnAction(true, _rememberSettings));
            return TryClose();
        }

        /// <summary>
        ///     Attempts to close this dialogue- triggering the OnCloseDialogue event.
        /// </summary>
        /// <returns>Was this dialogue successfully closed?</returns>
        public override bool TryClose()
        {
            if (!capi.IsSinglePlayer || ApiEx.ClientMain.GetField<bool>("OpenedToLan")) return base.TryClose();
            ApiEx.ClientMain.PauseGame(false);
            return base.TryClose();
        }
    }
}