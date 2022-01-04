using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedParameter.Local
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local

#pragma warning disable IDE0051 // Remove unused private members

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public class BlockEntityStaticTranslocatorPatches : WorldSettingsConsumer<AutoWaypointsSettings>
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockStaticTranslocator), "OnEntityCollide")]
        private static bool Patch_BlockStaticTranslocator_OnEntityCollide_Prefix(BlockStaticTranslocator __instance, Entity entity)
        {
            if (!Settings.Translocators) return true;
            if (entity != ApiEx.Client.World.Player.Entity) return true;
            if (entity.Pos.AsBlockPos.WaypointExistsWithinRadius(1, 1)) return true;
            ApiEx.ClientMain.EnqueueMainThreadTask(() =>
            {
                ApiEx.Client.TriggerChatMessage(".wptl");
            }, "");
            return true;
        }
    }
}