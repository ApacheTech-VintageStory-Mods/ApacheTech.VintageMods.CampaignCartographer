using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using Vintagestory.API.Common;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public partial class PlayerPinsPatches : WorldSettingsConsumer<PlayerPinsSettings>
    {

    }
}