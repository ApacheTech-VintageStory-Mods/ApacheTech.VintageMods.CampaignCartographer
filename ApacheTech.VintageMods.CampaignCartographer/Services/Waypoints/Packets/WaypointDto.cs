using System;
using System.Drawing;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Abstractions;
using ApacheTech.VintageMods.Core.Extensions.DotNet;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using Newtonsoft.Json;
using ProtoBuf;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets
{
    /// <summary>
    ///     Represents a waypoint that can be exported, or imported into the game.
    /// </summary>
    /// <seealso cref="IWaypoint" />
    /// <seealso cref="IEquatable{WaypointDto}" />
    /// <seealso cref="IEquatable{Waypoint}" />
    [JsonObject]
    [ProtoContract]
    public sealed class WaypointDto : IWaypoint, IEquatable<WaypointDto>, IEquatable<Waypoint>
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
        public string ServerIcon { get; set; }

        /// <summary>
        ///     Gets or sets the icon that will be displayed on the map.
        /// </summary>
        /// <value>The icon that will be displayed on the map.</value>
        [JsonRequired]
        [ProtoMember(4)]
        public string DisplayedIcon { get; set; }

        /// <summary>
        ///     Gets or sets the colour of the icon to be displayed.
        /// </summary>
        /// <value>The colour of the icon to be displayed.</value>
        [JsonRequired]
        [ProtoMember(5)]
        public string Colour { get; set; }

        /// <summary>
        ///     Gets or sets the position on the world map to display the waypoint. World-Position. Not Relative to spawn.
        /// </summary>
        /// <value>The <see cref="BlockPos" /> position on the world map to display the waypoint.</value>
        [JsonRequired]
        [ProtoMember(6)]
        public BlockPos Position { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this waypoint is pinned to the map, so that
        ///     it is still rendered when the screen caret is not focussed on the map region.
        /// </summary>
        /// <value><c>true</c> if pinned; otherwise, <c>false</c>.</value>
        [JsonRequired]
        [ProtoMember(7)]
        public bool Pinned { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this waypoint is currently enabled within the imports/exports list.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        [ProtoIgnore]
        public bool Enabled { get; set; } = true;

        /// <summary>
        ///     Creates an instance of <see cref="WaypointDto"/> with generic default settings.
        /// </summary>
        [JsonIgnore]
        [ProtoIgnore]
        public static WaypointDto Default => new()
        {
            DisplayedIcon = WaypointIcon.Circle,
            ServerIcon = WaypointIcon.Circle,
            Position = new BlockPos(),
            Pinned = false,
            Colour = NamedColour.Black,
            Title = "New Waypoint",
            DetailText = ""
        };

        /// <summary>
        ///     Converts a native <see cref="Waypoint"/> object into a <see cref="WaypointDto"/>.
        /// </summary>
        /// <param name="waypoint">The waypoint to convert.</param>
        /// <returns>An instance of <see cref="WaypointDto"/>, with the same values as the original native <see cref="Waypoint"/>.</returns>
        public static WaypointDto FromWaypoint(Waypoint waypoint)
        {
            var colour = Color.FromArgb(255,
                ColorUtil.ColorR(waypoint.Color),
                ColorUtil.ColorG(waypoint.Color),
                ColorUtil.ColorB(waypoint.Color));

            return new WaypointDto
            {
                ServerIcon = waypoint.Icon,
                DisplayedIcon = waypoint.Icon,
                Position = waypoint.Position.AsBlockPos,
                Pinned = waypoint.Pinned,
                Colour = colour.ToArgbHexString(),
                Title = waypoint.Title,
                DetailText = waypoint.Text
            };
        }

        /// <summary>
        ///     Converts this instance to a vanilla <see cref="Waypoint"/> object that the game internals can work with.
        /// </summary>
        /// <returns>An instance of the game's native <see cref="Waypoint"/>, POCO.</returns>
        public Waypoint ToWaypoint()
        {
            return new Waypoint
            {
                Icon = ServerIcon,
                Position = Position.ToVec3d(),
                Pinned = Pinned,
                Color = Colour.ColourValue(),
                Title = Title,
                Text = DetailText
            };
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(WaypointDto other)
        {
            var sameIcon = DisplayedIcon.Equals(other?.DisplayedIcon, StringComparison.InvariantCultureIgnoreCase);
            var sameColour = Colour.Equals(other?.Colour, StringComparison.InvariantCultureIgnoreCase);
            var samePosition = Position.Equals(other?.Position);
            return sameIcon && sameColour && samePosition;
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Waypoint other)
        {
            var sameIcon = ServerIcon.Equals(other?.Icon, StringComparison.InvariantCultureIgnoreCase);
            var sameColour = Colour.ColourValue().Equals(other?.Color);
            var samePosition = Position.Equals(other?.Position.AsBlockPos);
            return sameIcon && sameColour && samePosition;
        }
    }
}