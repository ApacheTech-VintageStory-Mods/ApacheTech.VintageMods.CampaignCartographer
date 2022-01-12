using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using Newtonsoft.Json;
using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model
{
    /// <summary>
    ///     Represents meta data for the block selection waypoint.
    /// </summary>
    [JsonObject]
    [ProtoContract]
    public class BlockSelectionWaypointTemplate
    {
        /// <summary>
        ///     Gets or sets the icon that will be saved tot he server.
        /// </summary>
        /// <value>The icon that will be saved to the server.</value>
        [JsonRequired]
        [ProtoMember(1)]
        public string ServerIcon { get; set; } = WaypointIcon.Circle;

        /// <summary>
        ///     Gets or sets the icon that will be displayed on the map.
        /// </summary>
        /// <value>The icon that will be displayed on the map.</value>
        [JsonRequired]
        [ProtoMember(2)]
        public string DisplayedIcon { get; set; } = WaypointIcon.Circle;

        /// <summary>
        ///     Gets or sets the colour of the icon to be displayed.
        /// </summary>
        /// <value>The colour of the icon to be displayed.</value>
        [JsonRequired]
        [ProtoMember(3)]
        public string Colour { get; set; } = NamedColour.Black;

        /// <summary>
        ///     Determines how far apart waypoints of the same type can be.
        /// </summary>
        /// <value>The minimum distance between two waypoints of the same type.</value>
        [ProtoMember(4)]
        public int HorizontalCoverageRadius { get; set; } = 10;

        /// <summary>
        ///     Determines how far apart waypoints of the same type can be.
        /// </summary>
        /// <value>The minimum distance between two waypoints of the same type.</value>
        [ProtoMember(5)]
        public int VerticalCoverageRadius { get; set; } = 10;
    }
}