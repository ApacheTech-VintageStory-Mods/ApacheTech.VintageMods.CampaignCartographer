using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedParameter.Local
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public class GuiDialogTraderPatches : WorldSettingsConsumer<AutoWaypointsSettings>
    {
        private static int _timesRun;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GuiDialogTrader), "OnGuiOpened")]
        private static void Patch_GuiDialogTrader_OnGuiOpened_Postfix(GuiDialogTrader __instance)
        {
            if (!Settings.Traders) return;
            if (++_timesRun > 1) return;
            ApiEx.Client.RegisterDelayedCallback(_ => _timesRun = 0, 1000 * 30);
            ApiEx.ClientMain.EnqueueMainThreadTask(() =>
            {
                ApiEx.Client.TriggerChatMessage(".wpt");
            }, "");
        }
    }
}