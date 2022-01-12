using System.Collections.Generic;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using Newtonsoft.Json;
using ProtoBuf;
using SmartAssembly.Attributes;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model
{
    /// <summary>
    ///     A model of all information required to set a Waypoint.
    /// </summary>
    [ProtoContract]
    [JsonObject]
    [DoNotPruneType]
    public sealed class WaypointInfoModel
    {
        /// <summary>
        ///     Gets the list of syntax arguments that are valid for this waypoint type.
        /// </summary>
        /// <value>The list of syntax arguments that are valid for this waypoint type.</value>
        [ProtoMember(1)]
        public List<string> Syntax { get; set; } = new ();

        /// <summary>
        ///     Gets the icon to use for the waypoint.
        /// </summary>
        /// <value>The icon to use for the waypoint.</value>
        [ProtoMember(2)]
        public string Icon { get; set; } = WaypointIcon.Circle;

        /// <summary>
        ///     Gets the colour to use for the waypoint marker.
        /// </summary>
        /// <value>The colour to use for the waypoint marker.</value>
        [ProtoMember(3)]
        public string Colour { get; set; } = NamedColour.Black;

        /// <summary>
        ///     Gets the default title of the waypoint marker.
        /// </summary>
        /// <value>The default title of the waypoint marker.</value>
        [ProtoMember(4)]
        public string DefaultTitle { get; set; } = LangEx.FeatureString("ManualWaypoints.WaypointTemplate", "DefaultTitle");

        /// <summary>
        ///     Determines how far apart waypoints of the same type can be.
        /// </summary>
        /// <value>The minimum distance between two waypoints of the same type.</value>
        [ProtoMember(5)]
        public int HorizontalCoverageRadius { get; set; } = 10;

        /// <summary>
        ///     Determines how far apart waypoints of the same type can be.
        /// </summary>
        /// <value>The minimum distance between two waypoints of the same type.</value>
        [ProtoMember(6)]
        public int VerticalCoverageRadius { get; set; } = 10;

        /// <summary>
        ///     Determines whether or not this waypoint type is enabled for manual entry.
        /// </summary>
        /// <returns><c>true</c> if this waypoint type should be added to the manual waypoints syntax list; otherwise <c>false</c></returns>
        [ProtoMember(7)]
        public bool Enabled { get; set; } = true;
    }
}