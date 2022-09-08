using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Newtonsoft.Json;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints
{
    /// <summary>
    ///     Represents a DTO object that stores settings information for the Manual Waypoints feature.
    /// </summary>
    [JsonObject]
    public sealed class PredefinedWaypointsSettings
    {
        /// <summary>
        ///     Gets or sets the block selection waypoint template.
        /// </summary>
        /// <value>The block selection waypoint template.</value>
        public CoverageWaypointTemplate BlockSelectionWaypointTemplate { get; set; } = new();
    }
}   