using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public class BlockEntityStaticTranslocatorPatches : WorldSettingsConsumer<AutoWaypointsSettings>
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockStaticTranslocator), "OnEntityCollide")]
        public static bool Patch_BlockStaticTranslocator_OnEntityCollide_Prefix(Entity entity)
        {
            if (ApiEx.Side.IsServer()) return true; // Single-player race condition fix.
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