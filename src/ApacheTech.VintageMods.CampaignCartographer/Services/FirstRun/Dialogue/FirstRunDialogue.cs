using System;
using ApacheTech.Common.Extensions.Harmony;
using Gantry.Core;
using Gantry.Core.GameContent.GUI;
using Gantry.Core.GameContent.GUI.Helpers;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.FirstRun.Dialogue
{
    [UsedImplicitly]
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

        public override bool DisableMouseGrab => true;

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
                .AddStaticImage(AssetLocation.Create("campaigncartographer:textures/dialogue/menu-logo.png"), squareBounds)
                .AddStaticText(
                    LangEx.FeatureString("FirstRun.Dialogue", "Paragraph1"),
                    CairoFont
                        .WhiteDetailText()
                        .WithOrientation(EnumTextOrientation.Center),
                    ButtonBounds(0.7f, width, height)
                        .FlatCopy()
                        .WithAlignment(EnumDialogArea.CenterFixed))
                .AddStaticText(
                    LangEx.FeatureString("FirstRun.Dialogue", "Paragraph2"),
                    CairoFont.WhiteDetailText()
                        .WithOrientation(EnumTextOrientation.Center),
                    ButtonBounds(1.7f, width, height).FlatCopy().WithAlignment(EnumDialogArea.CenterFixed))
                .AddStaticText(
                    LangEx.FeatureString("FirstRun.Dialogue", "RememberPreference"),
                    CairoFont.WhiteDetailText().WithOrientation(EnumTextOrientation.Right),
                    ButtonBounds(2.7f, width - 45, height).FlatCopy().WithAlignment(EnumDialogArea.RightFixed).WithFixedOffset(-45, 5))
                .AddSwitch(OnRememberSettingsChanged, ButtonBounds(2.7f, 30, height).FlatCopy().WithAlignment(EnumDialogArea.RightFixed).WithFixedOffset(-5, 0))
                .AddSmallButton(LangEx.ConfirmationString("no"), OnNo, ButtonBounds(3.4f, 150, height).FlatCopy().WithAlignment(EnumDialogArea.LeftFixed))
                .AddSmallButton(LangEx.ConfirmationString("yes"), OnYes, ButtonBounds(3.4f, 150, height).FlatCopy().WithAlignment(EnumDialogArea.RightFixed));
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
            ConfirmAndExit(false);
            return true;
        }

        private bool OnYes()
        {
            ConfirmAndExit(true);
            return true;
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

        private void ConfirmAndExit(bool loadWaypoints)
        {
            var title = LangEx.FeatureString("FirstRun.Dialogue", "Title");
            var suffix = loadWaypoints ? "ConfirmMessage" : "CancelMessage";
            var message = LangEx.FeatureString("FirstRun.Dialogue", suffix);
            MessageBox.Show(title, message, ButtonLayout.OkCancel, () =>
            {
                OnReturnAction(loadWaypoints, _rememberSettings);
                TryClose();
            });
        }

        public override bool OnEscapePressed()
        {
            ConfirmAndExit(false);
            return false;
        }
    }
}