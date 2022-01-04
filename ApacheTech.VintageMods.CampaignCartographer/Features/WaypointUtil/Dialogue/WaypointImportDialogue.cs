using ApacheTech.VintageMods.Core.Abstractions.GUI;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue
{
    public class WaypointImportDialogue : GenericDialogue
    {
        public WaypointImportDialogue(ICoreClientAPI capi) : base(capi)
        {

        }

        protected override void ComposeBody(GuiComposer composer)
        {
            throw new System.NotImplementedException();
        }

        protected override void RefreshValues()
        {

        }

        public override string ToggleKeyCombinationCode => "wpExports";
    }
}