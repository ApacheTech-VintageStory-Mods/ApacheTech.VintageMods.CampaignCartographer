using ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Dialogue.PredefinedWaypoints;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.Imports;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.WaypointSelection
{
    /// <summary>
    ///     Cell information, displayed within a <see cref="PredefinedWaypointsGuiCell"/>, in the cell list on the <see cref="PredefinedWaypointsDialogue"/> screen.
    /// </summary>
    /// <seealso cref="SavegameCellEntry" />
    public class WaypointSelectionCellEntry : CellEntry<SelectableWaypointTemplate>
    {
        /// <summary>
        ///     The position within the Waypoint list, regardless of sorting.
        /// </summary>
        public int Index { get; init; }
    }
}