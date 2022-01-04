using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using Vintagestory.API.Client;

// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Dialogue
{
    public sealed class GpsAdminDialogue : FeatureSettingsDialogue<GpsSettings>
    {
        public GpsAdminDialogue(ICoreClientAPI capi, GpsSettings settings) : base(capi, settings, "GPS")
        {
        }

        public override string ToggleKeyCombinationCode => "gpsAdminDialogue";

        protected override void RefreshValues()
        {
            SingleComposer.GetSwitch("btnWhispers").SetValue(Settings.WhispersAllowed);
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            const int switchSize = 20;

            var sliderBounds = ElementBounds.Fixed(160, GuiStyle.TitleBarHeight, switchSize, switchSize);
            var textBounds = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight + 1.0, 150, switchSize);
            
            var font = CairoFont.WhiteSmallText();
            
            composer.AddStaticText(LangEx.FeatureString(FeatureName, "Dialogue.lblWhispers"), font, textBounds);
            composer.AddHoverText(LangEx.FeatureString(FeatureName, "Dialogue.lblWhispers.HoverText"), font, 260, textBounds);
            composer.AddSwitch(OnWhispersChanged, sliderBounds.FlatCopy().WithFixedWidth(switchSize), "btnWhispers");
        }

        private void OnWhispersChanged(bool state)
        {
            Settings.WhispersAllowed = state;
            RefreshValues();
        }
    }
}
