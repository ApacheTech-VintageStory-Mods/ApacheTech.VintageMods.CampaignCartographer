using System.Collections.Generic;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Extensions;
using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public class BlockEntityTeleporterBasePatches : WorldSettingsConsumer<AutoWaypointsSettings>
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockEntityTeleporterBase), "OnEntityCollide")]
        public static bool Patch_BlockEntityTeleporter_OnEntityCollide_Prefix(BlockEntityTeleporterBase __instance, IReadOnlyDictionary<long, TeleportingEntity> ___tpingEntities)
        {
            if (ApiEx.Side.IsServer()) return true; // Single-player race condition fix.
            if (__instance is BlockEntityStaticTranslocator) return true;
            if (!Settings.Teleporters) return true;
            var playerId = ApiEx.Client.World.Player.Entity.EntityId;
            if (!___tpingEntities.ContainsKey(playerId)) return true;
            if (___tpingEntities[playerId].Entity.Pos.AsBlockPos.WaypointExistsWithinRadius(1, 1)) return true;

            var titleTemplate = LangEx.FeatureCode("ManualWaypoints.TeleporterWaypoints", "TeleporterWaypointTitle");
            __instance.AddWaypoint(titleTemplate);
            return true;
        }
    }
}