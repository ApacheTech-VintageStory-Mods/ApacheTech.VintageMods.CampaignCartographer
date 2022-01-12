using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public class GuiDialogTraderPatches : WorldSettingsConsumer<AutoWaypointsSettings>
    {
        private static int _timesRun;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GuiDialogTrader), "OnGuiOpened")]
        public static void Patch_GuiDialogTrader_OnGuiOpened_Postfix()
        {
            if (ApiEx.Side.IsServer()) return; // Single-player race condition fix.
            if (!Settings.Traders) return;
            if (++_timesRun > 1) return;
            ApiEx.Client.RegisterDelayedCallback(_ => _timesRun = 0, 1000);
            ApiEx.ClientMain.EnqueueMainThreadTask(() =>
            {
                ApiEx.Client.TriggerChatMessage(".wpt");
            }, "");
        }
    }
}