using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using Newtonsoft.Json;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints
{
    /// <summary>
    ///     Represents a DTO object that stores settings information for the Manual Waypoints feature.
    /// </summary>
    [JsonObject]
    public sealed class ManualWaypointsSettings
    {
        /// <summary>
        ///     Gets or sets the block selection waypoint template.
        /// </summary>
        /// <value>The block selection waypoint template.</value>
        public BlockSelectionWaypointTemplate BlockSelectionWaypointTemplate { get; set; }
    }
}   