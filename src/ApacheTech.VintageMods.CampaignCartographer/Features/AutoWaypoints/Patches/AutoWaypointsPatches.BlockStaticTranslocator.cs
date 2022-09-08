using ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions;
using Gantry.Core.Extensions.Api;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    public partial class AutoWaypointsPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockStaticTranslocator), "OnEntityCollide")]
        public static void Patch_BlockStaticTranslocator_OnEntityCollide_Postfix(BlockStaticTranslocator __instance, ICoreClientAPI ___api, Entity entity, BlockPos pos)
        {
            if (___api.Side.IsServer()) return;
            if (++_timesRunBlock > 1) return;
            ___api.RegisterDelayedCallback(_ => _timesRunBlock = 0, 1000 * 3);
            if (!Settings.Translocators) return;
            if (entity != ___api.World.Player.Entity) return;
            __instance.ProcessWaypoints(pos);
        }
    }
}