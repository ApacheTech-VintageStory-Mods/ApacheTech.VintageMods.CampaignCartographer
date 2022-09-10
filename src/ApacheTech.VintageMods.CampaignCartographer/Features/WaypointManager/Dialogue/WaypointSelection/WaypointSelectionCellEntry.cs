using ApacheTech.VintageMods.CampaignCartographer.Domain;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.WaypointSelection
{
    /// <summary>
    ///     Cell information, displayed within a <see cref="WaypointSelectionCellEntry"/>, in the cell list on the <see cref="WaypointSelectionDialogue"/> screen.
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