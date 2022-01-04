using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
using Vintagestory.API.Common;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public class BlockPatches
    {
        private static readonly AutoWaypointPatchHandler _handler;
        private static int _timesRun;

        static BlockPatches()
        {
            _handler = new AutoWaypointPatchHandler();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Block), "OnBlockInteractStart")]
        public static void Patch_Block_OnBlockInteractStart_Postfix(Block __instance)
        {
            if (++_timesRun > 1) return;
            ApiEx.Client.RegisterDelayedCallback(_ => _timesRun = 0, 1000);
            _handler.HandleInteraction(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Block), "OnBlockBroken")]
        public static void Patch_Block_OnBlockBroken_Postfix(Block __instance)
        {
            if (++_timesRun > 1) return;
            ApiEx.Client.RegisterDelayedCallback(_ => _timesRun = 0, 1000);
            _handler.HandleInteraction(__instance);
        }
    }
}