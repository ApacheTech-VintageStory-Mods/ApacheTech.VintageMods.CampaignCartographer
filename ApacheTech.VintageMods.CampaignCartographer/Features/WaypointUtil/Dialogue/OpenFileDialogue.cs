using ApacheTech.VintageMods.Core.Abstractions.GUI;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue
{
    public class OpenFolderDialogue : GenericDialogue
    {
        public OpenFolderDialogue(ICoreClientAPI capi) : base(capi)
        {
            Alignment = EnumDialogArea.CenterMiddle;
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            var font = CairoFont.WhiteSmallishText();
            const int rowPadding = 20;
            const int columnPadding = 10;
            const int leftSidebarWidth = 100;
            const int rightContentWidth = 300;
            const int rightContentOffsetX = leftSidebarWidth + columnPadding;
            const int fullWidth = rightContentOffsetX + rightContentWidth;

            var leftSidebar = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight + 1.0, leftSidebarWidth, rowPadding);
            var rightContent = ElementBounds.Fixed(rightContentOffsetX, GuiStyle.TitleBarHeight, rightContentWidth, rowPadding);
            var fullContainer = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight + 1.0, fullWidth, rowPadding);
            
            composer.AddContainer(fullContainer.FlatCopy().WithFixedHeight(fullWidth), "pnlContainer");
            fullContainer = fullContainer.BelowCopy(fixedDeltaY: fullWidth + rowPadding);
            composer.AddButton("Close", TryClose, fullContainer, font, key: "btnClose");
        }

        protected override void RefreshValues()
        {

        }

        public override string ToggleKeyCombinationCode => "openFileDialogue";
    }
}