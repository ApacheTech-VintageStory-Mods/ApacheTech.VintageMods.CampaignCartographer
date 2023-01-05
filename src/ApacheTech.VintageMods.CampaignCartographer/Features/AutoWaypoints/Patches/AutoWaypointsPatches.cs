using Gantry.Services.FileSystem.Configuration.Consumers;
using Gantry.Services.HarmonyPatches.Annotations;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Patches
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [HarmonySidedPatch(EnumAppSide.Client)]
    public partial class AutoWaypointsPatches : WorldSettingsConsumer<AutoWaypointsSettings>
    {

    }
}