using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services;
using HarmonyLib;
using Vintagestory.API.Common;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    public partial class AutoWaypointsPatches
    {
        private static AutoWaypointPatchHandler _handler;
        private static int _timesRunBlock;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Block), "OnBlockInteractStart")]
        public static void Patch_Block_OnBlockInteractStart_Postfix(Block __instance)
        {
            if (ApiEx.Side.IsServer()) return; // Single-player race condition fix.
            if (++_timesRunBlock > 1) return;
            ApiEx.Client.RegisterDelayedCallback(_ => _timesRunBlock = 0, 1000);

            _handler ??= ModServices.IOC.Resolve<AutoWaypointPatchHandler>();
            _handler.HandleInteraction(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Block), "OnBlockBroken")]
        public static void Patch_Block_OnBlockBroken_Postfix(Block __instance)
        {
            if (ApiEx.Side.IsServer()) return; // Single-player race condition fix.
            if (++_timesRunBlock > 1) return;
            ApiEx.Client.RegisterDelayedCallback(_ => _timesRunBlock = 0, 1000);

            _handler ??= ModServices.IOC.Resolve<AutoWaypointPatchHandler>();
            _handler.HandleInteraction(__instance);
        }
    }
}