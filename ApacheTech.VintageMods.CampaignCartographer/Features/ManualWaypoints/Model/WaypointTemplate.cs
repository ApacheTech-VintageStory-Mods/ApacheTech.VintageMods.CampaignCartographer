using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using Newtonsoft.Json;
using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model
{
    [JsonObject]
    [ProtoContract]
    public class WaypointTemplate
    {
        /// <summary>
        ///     Gets or sets the title of the waypoint.
        /// </summary>
        /// <value>The title of the waypoint.</value>
        [JsonRequired]
        [ProtoMember(1)]
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the subtitle of the waypoint.
        /// </summary>
        /// <value>The subtitle of the waypoint.</value>
        [ProtoMember(2)]
        public string DetailText { get; set; }

        /// <summary>
        ///     Gets or sets the icon that will be saved tot he server.
        /// </summary>
        /// <value>The icon that will be saved to the server.</value>
        [JsonRequired]
        [ProtoMember(3)]
        public string ServerIcon { get; set; } = WaypointIcon.Circle;

        /// <summary>
        ///     Gets or sets the icon that will be displayed on the map.
        /// </summary>
        /// <value>The icon that will be displayed on the map.</value>
        [JsonRequired]
        [ProtoMember(4)]
        public string DisplayedIcon { get; set; } = WaypointIcon.Circle;

        /// <summary>
        ///     Gets or sets the colour of the icon to be displayed.
        /// </summary>
        /// <value>The colour of the icon to be displayed.</value>
        [JsonRequired]
        [ProtoMember(5)]
        public string Colour { get; set; } = NamedColour.Black;

        /// <summary>
        ///     Determines how far apart waypoints of the same type can be.
        /// </summary>
        /// <value>The minimum distance between two waypoints of the same type.</value>
        [ProtoMember(7)]
        public int HorizontalCoverageRadius { get; set; } = 10;

        /// <summary>
        ///     Determines how far apart waypoints of the same type can be.
        /// </summary>
        /// <value>The minimum distance between two waypoints of the same type.</value>
        [ProtoMember(6)]
        public int VerticalCoverageRadius { get; set; } = 10;

        /// <summary>
        ///     Gets or sets a value indicating whether this waypoint is pinned to the map, so that
        ///     it is still rendered when the screen caret is not focussed on the map region.
        /// </summary>
        /// <value><c>true</c> if pinned; otherwise, <c>false</c>.</value>
        [JsonRequired]
        [ProtoMember(7)]
        public bool Pinned { get; set; }

        /// <summary>
        ///     Determines whether or not this waypoint type is enabled for manual waypoint addition.
        /// </summary>
        /// <returns><c>true</c> if this waypoint type should be added to the manual waypoints syntax list; otherwise <c>false</c></returns>
        [ProtoMember(8)]
        public bool Enabled { get; set; } = true;

        /// <summary>
        ///     Creates an instance of <see cref="WaypointTemplate"/> with generic default settings.
        /// </summary>
        [JsonIgnore]
        [ProtoIgnore]
        public static WaypointTemplate Default => new()
        {
            Title = LangEx.FeatureString("ManualWaypoints.WaypointTemplate", "DefaultTitle"),
            DetailText = "",
            ServerIcon = WaypointIcon.Circle,
            DisplayedIcon = WaypointIcon.Circle,
            Colour = NamedColour.Black,
            HorizontalCoverageRadius = 10,
            VerticalCoverageRadius = 10,
            Pinned = false,
        };
    }
}
