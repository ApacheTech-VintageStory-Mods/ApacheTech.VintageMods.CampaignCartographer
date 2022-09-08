using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ApacheTech.Common.Extensions.Functional;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Gantry.Core.Extensions.GameContent;
using Gantry.Core;
using Gantry.Core.Extensions.Api;
using Gantry.Core.GameContent.AssetEnum;
using JetBrains.Annotations;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.Extensions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class WaypointTemplateExtensions
    {
        private static readonly List<BlockPos> PositionsBeingHandled = new();

        /// <summary>
        ///     Adds a <see cref="CoverageWaypointTemplate"/> to the world map. These are waypoints that haven't been added before.
        /// </summary>
        /// <param name="waypoint">The waypoint.</param>
        /// <param name="position">The position.</param>
        /// <param name="force"></param>
        public static void AddToMap(this CoverageWaypointTemplate waypoint, BlockPos position = null, bool force = false)
        {
            // DEV NOTE:    This method looks needlessly complicated because of race conditions when running in single-player mode.
            //              In these cases, it's possible for this method to be run multiple times, resulting in multiple waypoints
            //              being added. This was intended as a temporary fix, however, it seems like without adding a global cooldown
            //              for waypoint related actions, this is the best way to resolve the issues. This still enables waypoints
            //              to be added in rapid succession, but limits the calls to one pass-through per block, per second.

            position ??= ApiEx.Client.World.Player.Entity.Pos.AsBlockPos;
            if (PositionsBeingHandled.Contains(position)) return;
            PositionsBeingHandled.Add(position);
            try
            {
                if (!force && position.WaypointExistsWithinRadius(waypoint.HorizontalCoverageRadius, waypoint.VerticalCoverageRadius,
                        p =>
                        {
                            var sameIcons = p.Icon.EndsWith(waypoint.DisplayedIcon, StringComparison.InvariantCultureIgnoreCase);
                            var sameColour = p.Color == waypoint.Colour.ColourValue();
                            return sameIcons && sameColour;
                        })) return;
                ApiEx.ClientMain.EnqueueMainThreadTask(() =>
                {
                    position.AddWaypointAtPos(waypoint.DisplayedIcon.ToLower(), waypoint.Colour.ToLower(), waypoint.Title, waypoint.Pinned);
                }, "");
            }
            finally
            {
                ApiEx.Client.RegisterDelayedCallback(_ => { PositionsBeingHandled.Remove(position); }, 1000);
            }
        }

        /// <summary>
        ///     Adds a <see cref="WaypointTemplate"/> to the world map. These are waypoints that are being imported into the game, and have a position already.
        /// </summary>
        /// <param name="waypoint">The waypoint to add.</param>
        /// <param name="position">The position to add the waypoint at.</param>
        public static void AddToMap(this WaypointTemplate waypoint, BlockPos position)
        {
            // DEV NOTE:    This method looks needlessly complicated because of race conditions when running in single-player mode.
            //              In these cases, it's possible for this method to be run multiple times, resulting in multiple waypoints
            //              being added. This was intended as a temporary fix, however, it seems like without adding a global cooldown
            //              for waypoint related actions, this is the best way to resolve the issues. This still enables waypoints
            //              to be added in rapid succession, but limits the calls to one pass-through per block, per second.

            if (PositionsBeingHandled.Contains(position)) return;
            PositionsBeingHandled.Add(position);
            try
            {
                ApiEx.ClientMain.EnqueueMainThreadTask(() =>
                {
                    position.AddWaypointAtPos(waypoint.ServerIcon.ToLower(), waypoint.Colour.ToLower(), waypoint.Title, waypoint.Pinned);
                }, "");
            }
            finally
            {
                ApiEx.Client.RegisterDelayedCallback(_ => { PositionsBeingHandled.Remove(position); }, 1000);
            }
        }


        /// <summary>
        ///     Converts a waypoint template to a waypoint that can be added to the map.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public static Waypoint ToWaypoint(this WaypointTemplate template)
        {
            return new Waypoint
            {
                Title = template.Title,
                Color = template.Colour.ToInt(),
                Pinned = template.Pinned,
                Icon = template.ServerIcon,
                Temporary = false
            };
        }


        /// <summary>
        ///     Converts a waypoint template to a waypoint that can be added to the map.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public static Waypoint ToPositionedWaypoint(this PositionedWaypointTemplate template)
        {
            return new Waypoint
            {
                Position = template.Position,
                Title = template.Title,
                Color = template.Colour.ToInt(),
                Pinned = template.Pinned,
                Icon = template.ServerIcon,
                Temporary = false
            };
        }

        public static bool IsSameAs(this WaypointTemplate @this, WaypointTemplate other)
        {
            return @this.Fork(x => x.All(p => p),
                x => x.Title == other.Title,
                x => x.Colour.ToInt() == other.Colour.ToInt(),
                x => x.Pinned == other.Pinned,
                x => x.ServerIcon == other.ServerIcon);
        }

        public static int ToInt(this string colourString)
        {
            if (colourString.StartsWith("#")) return ColorUtil.Hex2Int(colourString);
            return !NamedColour.TryParse(colourString, false, out var namedColour)
                ? Color.FromName(NamedColour.Black).ToArgb() | -16777216
                : Color.FromName(namedColour).ToArgb() | -16777216;
        }

        public static string ToHexString(this int intColour)
        {
            return ColorUtil.Int2Hex(intColour);
        }
    }
}
