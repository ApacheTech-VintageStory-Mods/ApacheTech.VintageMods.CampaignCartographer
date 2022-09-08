using System.Collections.Generic;
using ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions;
using Gantry.Core;
using Gantry.Core.Extensions.GameContent;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    public partial class AutoWaypointsPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockEntityTeleporterBase), "OnEntityCollide")]
        public static void Patch_BlockEntityTeleporter_OnEntityCollide_Prefix(BlockEntityTeleporterBase __instance, IReadOnlyDictionary<long, TeleportingEntity> ___tpingEntities)
        {
            if (ApiEx.Side.IsServer()) return;
            if (__instance is BlockEntityStaticTranslocator) return;
            if (!Settings.Teleporters) return;
            var playerId = ApiEx.Client.World.Player.Entity.EntityId;
            if (!___tpingEntities.ContainsKey(playerId)) return;
            if (___tpingEntities[playerId].Entity.Pos.AsBlockPos.WaypointExistsWithinRadius(1, 1)) return;

            var titleTemplate = LangEx.FeatureCode("PredefinedWaypoints.TeleporterWaypoints", "TeleporterWaypointTitle");
            __instance.AddWaypoint(titleTemplate);
        }
    }
}