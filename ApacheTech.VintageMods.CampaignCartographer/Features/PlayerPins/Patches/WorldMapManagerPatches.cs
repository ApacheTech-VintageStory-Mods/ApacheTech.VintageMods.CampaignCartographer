using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

// ReSharper disable UnusedMember.Global
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public sealed class WorldMapManagerPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WorldMapManager), "RegisterDefaultMapLayers")]
        public static void Patch_WorldMapManager_RegisterDefaultMapLayers_Postfix()
        {
            if (ApiEx.Side.IsServer()) return; // Single-player race condition fix.
            ApiEx.Client.ModLoader
                .GetModSystem<WorldMapManager>()
                .RegisterMapLayer<PlayerPinsMapLayer>("players");
        }
    }
}