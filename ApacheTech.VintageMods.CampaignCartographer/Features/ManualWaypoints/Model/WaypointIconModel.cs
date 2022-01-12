using System.Collections.Generic;
using ApacheTech.VintageMods.Core.Extensions;
using Vintagestory.API.Util;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model
{
    /// <summary>
    ///     Represents an icon that can be used to mark a waypoint on the world map.
    /// </summary>
    public class WaypointIconModel
    {
        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointIconModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isVanilla">if set to <c>true</c> [is vanilla].</param>
        public WaypointIconModel(string name, bool isVanilla = false)
        {
            Name = name.ToLowerInvariant();
            DisplayName = name.UcFirst().SplitPascalCase();
            Glyph = $"<icon name=\"wp{name.UcFirst()}\">";
            IsVanilla = isVanilla;
        }

        /// <summary>
        ///     Gets or sets the internal name for the icon.
        /// </summary>
        /// <value>The internal name for the icon.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the displayed name for the icon.
        /// </summary>
        /// <value>The displayed name for the icon.</value>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the glyph that is displayed.
        /// </summary>
        /// <value>The glyph.</value>
        public string Glyph { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this icon is vanilla.
        /// </summary>
        /// <value><c>true</c> if this icon is vanilla; otherwise, <c>false</c>.</value>
        public bool IsVanilla { get; set; }

        /// <summary>
        ///     Gets a list of the vanilla icons.
        /// </summary>
        public static List<WaypointIconModel> GetVanillaIcons()
        {
            return new List<WaypointIconModel>()
            {
                new("circle", true),
                new("bee", true),
                new("cave", true),
                new("home", true),
                new("ladder", true),
                new("pick", true),
                new("rocks", true),
                new("ruins", true),
                new("spiral", true),
                new("star1", true),
                new("star2", true),
                new("trader", true),
                new("vessel",true)
            };
        }
    }
}