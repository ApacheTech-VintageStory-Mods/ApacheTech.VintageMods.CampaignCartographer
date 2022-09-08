using System;
using System.Collections.Generic;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Newtonsoft.Json;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Model
{
    /// <summary>
    ///     Represents a JSON file that contains exported waypoints from a world.
    /// </summary>
    [JsonObject]
    public class WaypointFileModel
    {
        /// <summary>
        ///     Gets or sets the name given to this export file.
        /// </summary>
        /// <value>The name that is displayed in the imports screen.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the world from which these waypoints were exported.
        /// </summary>
        /// <value>The world from which these waypoints were exported.</value>
        public string World { get; set; }

        /// <summary>
        ///     Gets or sets the number of waypoints contained within the file.
        /// </summary>
        /// <value>The number of waypoints contained within the file.</value>
        public int Count { get; set; }

        /// <summary>
        ///     Gets or sets the date and time the export file was created.
        /// </summary>
        /// <value>The creation date of the export file.</value>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets a list of waypoints contained within the export file.
        /// </summary>
        /// <value>The list of exported waypoints.</value>
        public List<PositionedWaypointTemplate> Waypoints { get; set; }
    }
}