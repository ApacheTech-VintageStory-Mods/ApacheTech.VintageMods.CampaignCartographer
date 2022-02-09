using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil
{
    public class ModalOverlay : GuiDialog
    {
        public ModalOverlay(ICoreClientAPI capi) : base(capi)
        {
        }

        /// <summary>
        ///     Attempts to open this dialogue.
        /// </summary>
        /// <returns>
        ///     Returns <see langword="true"/> if the dialogue window was opened correctly; otherwise, returns <see langword="false"/>
        /// </returns>
        public override bool TryOpen()
        {
            var openWindows = ApiEx.Client.OpenedGuis;
            foreach (var gui in openWindows)
            {
                if (gui is not GuiDialog window) continue;
                if (window.ToggleKeyCombinationCode is null) continue;
                if (!window.ToggleKeyCombinationCode.Equals(ToggleKeyCombinationCode)) continue;
                window.Focus();
                return false;
            }
            if (base.TryOpen()) Compose();
            return opened;
        }

        public override bool DisableMouseGrab => true;

        public override string ToggleKeyCombinationCode => "ModalOverlay";

        private void Compose()
        {
            var platform = capi.World.GetField<ClientPlatformAbstract>("Platform");
            var fullScreenWidth = platform.WindowSize.Width;
            var fullScreenHeight = platform.WindowSize.Height;

            SingleComposer = capi.Gui.CreateCompo(ToggleKeyCombinationCode, ElementBounds.Fixed(0, 0, fullScreenWidth, fullScreenHeight))
                    .BeginChildElements(ElementBounds.Fixed(0, 0, fullScreenWidth, fullScreenHeight))
                    .AddDynamicCustomDraw(ElementBounds.Fixed(0, 0, fullScreenWidth, fullScreenHeight), (ctx, _, bounds) =>
                    {
                        ctx.Rectangle(0, 0, bounds.OuterWidth, bounds.OuterHeight);
                        ctx.SetSourceRGBA(0, 0, 0, 0.4);
                        ctx.Fill();
                    })
                    .EndChildElements()
                    .Compose();
        }
    }
}
