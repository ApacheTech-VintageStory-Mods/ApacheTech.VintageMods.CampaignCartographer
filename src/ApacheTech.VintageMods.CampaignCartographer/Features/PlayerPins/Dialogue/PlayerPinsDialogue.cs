using System;
using ApacheTech.Common.Extensions.Enum;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.DataStructures;
using Cairo;
using Gantry.Core;
using Gantry.Core.DependencyInjection.Annotation;
using Gantry.Core.Extensions.GameContent;
using Gantry.Services.FileSystem.Dialogue;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using Color = System.Drawing.Color;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Dialogue
{
    /// <summary>
    ///     GUI Window: Player Pins Settings.
    /// </summary>
    /// <seealso cref="FeatureSettingsDialogue{TFeatureSettings}" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PlayerPinsDialogue : FeatureSettingsDialogue<PlayerPinsSettings>
    {
        [SidedConstructor(EnumAppSide.Client)]
        public PlayerPinsDialogue(ICoreClientAPI capi, PlayerPinsSettings settings)
            : base(capi, settings, "PlayerPins")
        {
            Movable = true;
        }

        protected override void RefreshValues()
        {
            if (!IsOpened()) return;

            SetPlayerPinSwitch("btnTogglePlayerPins", (int)PlayerPinHelper.Relation);
            SetColourSliderValue("sliderR", PlayerPinHelper.Colour.R);
            SetColourSliderValue("sliderG", PlayerPinHelper.Colour.G);
            SetColourSliderValue("sliderB", PlayerPinHelper.Colour.B);
            SetColourSliderValue("sliderA", PlayerPinHelper.Colour.A);
            SetScaleSliderValue("sliderScale", PlayerPinHelper.Scale);
            SetPreviewColour("pnlPreview");

            capi.ModLoader
                .GetModSystem<WorldMapManager>()
                .PlayerMapLayer()
                .OnMapOpenedClient();
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            const int switchSize = 20;
            const int switchPadding = 20;
            const double sliderWidth = 200.0;
            var font = CairoFont.WhiteSmallText();

            var names = new[]
            {
                LangEx.FeatureString("PlayerPins", "Dialogue.Self"),
                LangEx.FeatureString("PlayerPins", "Dialogue.Friend"),
                LangEx.FeatureString("PlayerPins", "Dialogue.Others")
            };
            var values = new[] { "Self", "Friend", "Others" };

            var sliderBounds = ElementBounds.Fixed(160, GuiStyle.TitleBarHeight, switchSize, switchSize);
            var textBounds = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight + 1.0, 150, switchSize);

            var bounds = sliderBounds.FlatCopy().WithFixedWidth(sliderWidth).WithFixedHeight(GuiStyle.TitleBarHeight + 1.0);
            composer.AddStaticText(LangEx.FeatureString("PlayerPins", "Dialogue.lblToggleSwitch"), font, textBounds);
            composer.AddHoverText(LangEx.FeatureString("PlayerPins", "Dialogue.lblToggleSwitch.HoverText"), font, 260, textBounds);
            composer.AddDropDown(values, names, 0, OnSelectionChanged, bounds, font, "btnTogglePlayerPins");

            textBounds = textBounds.BelowCopy(fixedDeltaY: switchPadding + 5);
            sliderBounds = sliderBounds.BelowCopy(fixedDeltaY: switchPadding + 5);
            composer.AddStaticText(LangEx.FeatureString("PlayerPins", "Dialogue.lblRed"), font, textBounds);
            composer.AddHoverText(LangEx.FeatureString("PlayerPins", "Dialogue.lblRed.HoverText"), font, 260, textBounds);
            composer.AddSlider(OnRChanged, sliderBounds.FlatCopy().WithFixedWidth(sliderWidth), "sliderR");

            textBounds = textBounds.BelowCopy(fixedDeltaY: switchPadding);
            sliderBounds = sliderBounds.BelowCopy(fixedDeltaY: switchPadding);
            composer.AddStaticText(LangEx.FeatureString("PlayerPins", "Dialogue.lblGreen"), font, textBounds);
            composer.AddHoverText(LangEx.FeatureString("PlayerPins", "Dialogue.lblGreen.HoverText"), font, 260, textBounds);
            composer.AddSlider(OnGChanged, sliderBounds.FlatCopy().WithFixedWidth(sliderWidth), "sliderG");

            textBounds = textBounds.BelowCopy(fixedDeltaY: switchPadding);
            sliderBounds = sliderBounds.BelowCopy(fixedDeltaY: switchPadding);
            composer.AddStaticText(LangEx.FeatureString("PlayerPins", "Dialogue.lblBlue"), font, textBounds);
            composer.AddHoverText(LangEx.FeatureString("PlayerPins", "Dialogue.lblBlue.HoverText"), font, 260, textBounds);
            composer.AddSlider(OnBChanged, sliderBounds.FlatCopy().WithFixedWidth(sliderWidth), "sliderB");

            textBounds = textBounds.BelowCopy(fixedDeltaY: switchPadding);
            sliderBounds = sliderBounds.BelowCopy(fixedDeltaY: switchPadding);
            composer.AddStaticText(LangEx.FeatureString("PlayerPins", "Dialogue.lblOpacity"), font, textBounds);
            composer.AddHoverText(LangEx.FeatureString("PlayerPins", "Dialogue.lblOpacity.HoverText"), font, 260, textBounds);
            composer.AddSlider(OnAChanged, sliderBounds.FlatCopy().WithFixedWidth(sliderWidth), "sliderA");

            textBounds = textBounds.BelowCopy(fixedDeltaY: switchPadding);
            sliderBounds = sliderBounds.BelowCopy(fixedDeltaY: switchPadding);
            composer.AddStaticText(LangEx.FeatureString("PlayerPins", "Dialogue.lblScale"), font, textBounds);
            composer.AddHoverText(LangEx.FeatureString("PlayerPins", "Dialogue.lblScale.HoverText"), font, 260, textBounds);
            composer.AddSlider(OnScaleChanged, sliderBounds.FlatCopy().WithFixedWidth(sliderWidth), "sliderScale");

            textBounds = textBounds.BelowCopy(fixedDeltaY: switchPadding);
            composer.AddDynamicCustomDraw(textBounds.FlatCopy().WithFixedWidth(textBounds.fixedWidth + sliderWidth + 10), OnPreviewPanelDraw, "pnlPreview");

            textBounds = textBounds.BelowCopy(fixedDeltaY: switchPadding);
            composer.AddSmallButton(LangEx.FeatureString("PlayerPins", "Dialogue.Randomise"), OnRandomise,
                textBounds.FlatCopy().WithFixedWidth(360).WithFixedHeight(GuiStyle.TitleBarHeight + 1.0));

            SingleComposer = composer.EndChildElements().Compose();
        }

        private void OnSelectionChanged(string code, bool selected)
        {
            PlayerPinHelper.Relation = Enum.TryParse(code, out PlayerRelation relation) ? relation : PlayerRelation.Self;
            RefreshValues();
        }

        #region Set GUI Values

        private void SetColourSliderValue(string name, int value)
        {
            SingleComposer.GetSlider(name).SetValues(value, 0, 255, 1);
        }

        private void SetScaleSliderValue(string name, int value)
        {
            SingleComposer.GetSlider(name).SetValues(value, -5, 20, 1);
        }

        private void SetPlayerPinSwitch(string name, int value)
        {
            SingleComposer.GetDropDown(name).SetSelectedIndex(value);
        }

        private void SetPreviewColour(string name)
        {
            SingleComposer.GetCustomDraw(name).Redraw();
        }

        #endregion

        #region GUI Business Logic Callbacks
        private bool OnRandomise()
        {
            var rng = new Random(DateTime.Now.Millisecond);
            PlayerPinHelper.Colour = Color.FromArgb(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256));
            PlayerPinHelper.Scale = rng.Next(-5, 21);
            RefreshValues();
            return true;
        }

        private static void OnPreviewPanelDraw(Context ctx, ImageSurface surface, ElementBounds currentBounds)
        {
            var colour = PlayerPinHelper.Colour.ToNormalisedRgba();

            ctx.SetSourceRGBA(0, 0, 0, 1);
            ctx.LineWidth = 5.0;
            ctx.Rectangle(0, 0, surface.Width, surface.Height);
            ctx.Stroke();

            ctx.SetSourceRGBA(colour[0], colour[1], colour[2], colour[3]);
            ctx.Rectangle(2, 2, surface.Width - 4, surface.Height - 4);
            ctx.Fill();
        }

        private bool OnAChanged(int a) => OnColourChanged(ColourChannel.A, a);

        private bool OnRChanged(int r) => OnColourChanged(ColourChannel.R, r);

        private bool OnGChanged(int g) => OnColourChanged(ColourChannel.G, g);

        private bool OnBChanged(int b) => OnColourChanged(ColourChannel.B, b);

        private bool OnColourChanged(ColourChannel channel, int value)
        {
            PlayerPinHelper.Colour = PlayerPinHelper.Colour.UpdateColourChannel(channel, (byte)value);
            RefreshValues();
            return true;
        }

        private bool OnScaleChanged(int s)
        {
            PlayerPinHelper.Scale = s;
            RefreshValues();
            return true;
        }

        #endregion
    }
}