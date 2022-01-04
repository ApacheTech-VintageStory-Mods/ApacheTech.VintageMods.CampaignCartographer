using ApacheTech.VintageMods.Core.Abstractions.GUI;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue
{
    public class WaypointExportDialogue : GenericDialogue
    {
        public WaypointExportDialogue(ICoreClientAPI capi) : base(capi)
        {

        }

        protected override void ComposeBody(GuiComposer composer)
        {
            
        }

        protected override void RefreshValues()
        {

        }

        public override string ToggleKeyCombinationCode => "wpExports";
    }
}
