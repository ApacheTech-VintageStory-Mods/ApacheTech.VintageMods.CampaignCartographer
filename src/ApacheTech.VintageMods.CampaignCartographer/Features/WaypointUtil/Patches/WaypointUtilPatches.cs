using Gantry.Services.FileSystem.Configuration.Consumers;
using Gantry.Services.HarmonyPatches.Annotations;
using JetBrains.Annotations;
using Vintagestory.API.Common;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [SettingsConsumer(EnumAppSide.Client)]
    public partial class WaypointUtilPatches : WorldSettingsConsumer<WaypointUtilSettings>
    {
    }
}
