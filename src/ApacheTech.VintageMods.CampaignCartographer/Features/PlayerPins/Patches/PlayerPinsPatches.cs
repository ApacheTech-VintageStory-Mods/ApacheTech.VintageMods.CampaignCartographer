using Gantry.Services.FileSystem.Configuration.Consumers;
using Gantry.Services.HarmonyPatches.Annotations;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [SettingsConsumer(EnumAppSide.Client)]
    public partial class PlayerPinsPatches : WorldSettingsConsumer<PlayerPinsSettings>
    {
    }
}