using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using Vintagestory.API.Common;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public partial class AutoWaypointsPatches : WorldSettingsConsumer<AutoWaypointsSettings>
    {

    }
}