using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.GUI
{
    public class TestWindow : GuiDialog
    {
        public TestWindow(ICoreClientAPI capi) : base(capi)
        {
            ComposeDialogue();
        }

        public override string ToggleKeyCombinationCode => "test-window";

        private void ComposeDialogue()
        {
            ClearComposers();
            
            // Auto-sized dialog at the center of the screen
            var dialogueBounds = ElementStdBounds.AutosizedMainDialog
                .WithAlignment(EnumDialogArea.CenterMiddle)
                .WithFixedPosition(0.0, 75.0);

            dialogueBounds.horizontalSizing = ElementSizing.Fixed;
            dialogueBounds.verticalSizing = ElementSizing.Fixed;
            dialogueBounds.fixedWidth = 900.0;
            dialogueBounds.fixedHeight = 600.0;



            var font = CairoFont.ButtonText();

            // Just a simple 300x100 pixel box with 40 pixels top spacing for the title bar
            var textBounds = ElementBounds.Empty;
            textBounds.horizontalSizing = ElementSizing.FitToChildren;

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element

            var bgBounds = new ElementBounds()
                .WithSizing(ElementSizing.FitToChildren)
                .WithFixedPadding(GuiStyle.ElementToDialogPadding);

            bgBounds.verticalSizing = ElementSizing.FitToChildren;
            bgBounds.horizontalSizing = ElementSizing.Fixed;
            bgBounds.fixedWidth = 900.0 - 2.0 * GuiStyle.ElementToDialogPadding;
            bgBounds.WithChildren(textBounds);

            SingleComposer = capi.Gui.CreateCompo("myAwesomeDialog", dialogueBounds)
                .AddShadedDialogBG(dialogueBounds)
                .AddStaticText("This is a piece of text at the center of your screen - Enjoy!", CairoFont.WhiteDetailText(), textBounds)
                .Compose();
        }
    }
}