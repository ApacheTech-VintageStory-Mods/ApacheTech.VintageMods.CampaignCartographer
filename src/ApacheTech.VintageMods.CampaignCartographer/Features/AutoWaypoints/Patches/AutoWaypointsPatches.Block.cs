using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.Extensions.Api;
using HarmonyLib;
using Vintagestory.API.Common;

// ReSharper disable InconsistentNaming

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
            if (ApiEx.Side.IsServer()) return;
            if (++_timesRunBlock > 1) return;
            ApiEx.Client.RegisterDelayedCallback(_ => _timesRunBlock = 0, 1000);

            _handler ??= IOC.Services.Resolve<AutoWaypointPatchHandler>();
            _handler.HandleInteraction(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Block), "OnBlockBroken")]
        public static void Patch_Block_OnBlockBroken_Postfix(Block __instance)
        {
            if (ApiEx.Side.IsServer()) return;
            if (++_timesRunBlock > 1) return;
            ApiEx.Client.RegisterDelayedCallback(_ => _timesRunBlock = 0, 1000);

            _handler ??= IOC.Services.Resolve<AutoWaypointPatchHandler>();
            _handler.HandleInteraction(__instance);
        }
    }
}