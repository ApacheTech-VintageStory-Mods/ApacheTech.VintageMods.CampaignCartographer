using Vintagestory.API.MathTools;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Abstractions
{
    /// <summary>
    ///     Represents a waypoint that can be added to the world map.
    /// </summary>
    public interface IWaypoint
    {
        /// <summary>
        ///     Gets or sets the title of the waypoint.
        /// </summary>
        /// <value>The title of the waypoint.</value>
        string Title { get; set; }

        /// <summary>
        ///     Gets or sets the icon that will be displayed on the map.
        /// </summary>
        /// <value>The icon that will be displayed on the map.</value>
        string Icon { get; set; }

        /// <summary>
        ///     Gets or sets the colour of the icon to be displayed.
        /// </summary>
        /// <value>The colour of the icon to be displayed.</value>
        string Colour { get; set; }

        /// <summary>
        ///     Gets or sets the position on the world map to display the waypoint. World-Position. Not Relative to spawn.
        /// </summary>
        /// <value>The <see cref="BlockPos"/> position on the world map to display the waypoint.</value>
        BlockPos Position { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this waypoint is pinned to the map, so that
        ///     it is still rendered when the screen caret is not focussed on the map region.
        /// </summary>
        /// <value><c>true</c> if pinned; otherwise, <c>false</c>.</value>
        bool Pinned { get; set; }
    }
}