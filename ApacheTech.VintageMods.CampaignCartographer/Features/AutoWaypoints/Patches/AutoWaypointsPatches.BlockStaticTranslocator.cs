using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Extensions;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using HarmonyLib;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    public partial class AutoWaypointsPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockStaticTranslocator), "OnEntityCollide")]
        public static void Patch_BlockStaticTranslocator_OnEntityCollide_Postfix(BlockStaticTranslocator __instance, Entity entity, BlockPos pos)
        {
            var capi = ApiEx.Client;
            if (capi is null) return; // Single-player race condition fix.
            if (++_timesRunBlock > 1) return;
            capi.RegisterDelayedCallback(_ => _timesRunBlock = 0, 1000 * 3);
            if (!Settings.Translocators) return;
            if (entity != capi.World.Player.Entity) return;
            __instance.ProcessWaypoints(pos);
        }
    }
}