using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using HarmonyLib;
using Vintagestory.GameContent;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    public partial class AutoWaypointsPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GuiDialogTrader), "OnGuiOpened")]
        public static void Patch_GuiDialogTrader_OnGuiOpened_Prefix()
        {
            if (!Settings.Traders) return;
            ApiEx.ClientMain.EnqueueMainThreadTask(() =>
            {
                ApiEx.Client.TriggerChatMessage(".wpt");
            }, "");
        }
    }
}