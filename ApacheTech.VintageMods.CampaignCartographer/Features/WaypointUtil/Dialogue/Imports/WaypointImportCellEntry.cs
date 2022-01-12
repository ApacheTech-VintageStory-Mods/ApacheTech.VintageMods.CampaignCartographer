using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Model;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Imports
{
    /// <summary>
    ///     Defines the information stored within a single GUI Cell Element.
    ///     A list of these cells is displayed in <see cref="WaypointImportDialogue"/>,
    ///     to allow the user to import waypoints from a JSON file.
    /// </summary>
    /// <seealso cref="SavegameCellEntry" />
    public class WaypointImportCellEntry : SavegameCellEntry
    {
        /// <summary>
        ///     Gets the DTO model that defines the structure of the JSON import file.
        /// </summary>
        public WaypointFileModel Model { get; init; }
    }
}