using ApacheTech.VintageMods.Core.Abstractions.GUI;
using Vintagestory.API.Client;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Dialogue
{
    /// <summary>
    ///     APL for the AutoWaypoints feature.
    /// </summary>
    /// <seealso cref="AutomaticFeatureSettingsDialogue{AutoWaypointsSettings}" />
    /// <seealso cref="FeatureSettingsDialogue{AutoWaypointsSettings}" />
    /// <seealso cref="GenericDialogue" />
    /// <seealso cref="GuiDialog" />
    public sealed class AutoWaypointsDialogue : AutomaticFeatureSettingsDialogue<AutoWaypointsSettings>
    {
        public AutoWaypointsDialogue(ICoreClientAPI capi, AutoWaypointsSettings settings) : base(capi, settings, "AutoWaypoints")
        {
            // Everything is set up procedurally, within the base class.
        }
    }
}
