using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets;
using Vintagestory.API.Client;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Exports
{
    /// <summary>
    ///     Defines the information stored within a single GUI Cell Element.
    ///     A list of these cells is displayed in <see cref="WaypointExportDialogue"/>,
    ///     to allow the user to export waypoints to a JSON file.
    /// </summary>
    /// <seealso cref="SavegameCellEntry" />
    public class WaypointExportCellEntry : SavegameCellEntry
    {
        /// <summary>
        ///     Gets the DTO model that defines the structure of a waypoint in the world.
        /// </summary>
        public WaypointDto Waypoint { get; set; }
    }
}