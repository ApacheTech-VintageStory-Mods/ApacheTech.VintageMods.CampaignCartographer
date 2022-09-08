using Gantry.Core.GameContent.GUI;
using Gantry.Services.FileSystem.Dialogue;
using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Dialogue
{
    /// <summary>
    ///     APL for the AutoWaypoints feature.
    /// </summary>
    /// <seealso cref="AutomaticFeatureSettingsDialogue{TFeatureSettings}" />
    /// <seealso cref="FeatureSettingsDialogue{AutoWaypointsSettings}" />
    /// <seealso cref="GenericDialogue" />
    /// <seealso cref="GuiDialog" />
    [UsedImplicitly]
    public sealed class AutoWaypointsDialogue : AutomaticFeatureSettingsDialogue<AutoWaypointsSettings>
    {
        public AutoWaypointsDialogue(ICoreClientAPI capi, AutoWaypointsSettings settings) : base(capi, settings, "AutoWaypoints")
        {
            // Everything is set up procedurally, within the base class.
        }
    }
}
