using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using Vintagestory.API.Client;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue.PredefinedWaypoints
{
    /// <summary>
    ///     Cell information, displayed within a <see cref="PredefinedWaypointsGuiCell"/>, in the cell list on the <see cref="PredefinedWaypointsDialogue"/> screen.
    /// </summary>
    /// <seealso cref="SavegameCellEntry" />
    public class PredefinedWaypointsCellEntry : SavegameCellEntry
    {
        /// <summary>
        ///     Gets or sets the waypoint model this cell relates to.
        /// </summary>
        /// <value>The model for the waypoint template that this cell represents.</value>
        public ManualWaypointTemplateModel Model { get; set; }
    }
}