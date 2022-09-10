using ApacheTech.VintageMods.CampaignCartographer.Domain;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Model;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.Imports
{
    /// <summary>
    ///     Defines the information stored within a single GUI Cell Element.
    ///     A list of these cells is displayed in <see cref="WaypointImportDialogue"/>,
    ///     to allow the user to import waypoints from a JSON file.
    /// </summary>
    /// <seealso cref="SavegameCellEntry" />
    public class WaypointImportCellEntry : CellEntry<WaypointFileModel>
    {
    }
}