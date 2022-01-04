using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Extensions.System;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions
{
    public static class WaypointExtensions
    {
        private static readonly List<BlockPos> PositionsBeingHandled = new();

        public static void AddToMap(this WaypointInfoModel waypoint, BlockPos position = null, bool pinned = false)
        {
            position ??= ApiEx.Client.World.Player.Entity.Pos.AsBlockPos;
            if (PositionsBeingHandled.Contains(position)) return;
            PositionsBeingHandled.Add(position);
            try
            {
                if (position.WaypointExistsWithinRadius(waypoint.HorizontalCoverageRadius, waypoint.VerticalCoverageRadius, 
                        p =>
                        {
                            var sameIcons = p.Icon.EndsWith(waypoint.Icon, StringComparison.InvariantCultureIgnoreCase);
                            var sameColour = p.Color == waypoint.Colour.ColourValue();
                            return sameIcons && sameColour;
                        })) return;
                ApiEx.ClientMain.EnqueueMainThreadTask(() =>
                {
                    position.AddWaypointAtPos(waypoint.Icon.ToLower(), waypoint.Colour.ToLower(), waypoint.DefaultTitle, pinned);
                }, "");
            }
            finally
            {
                ApiEx.Client.RegisterDelayedCallback(_ => { PositionsBeingHandled.Remove(position); }, 1000);
            }
        }

        public static bool IsInHorizontalRangeOf(this Waypoint waypoint, BlockPos targetPosition, float squareDistance)
        {
            var waypointPos = waypoint.Position.AsBlockPos;
            var num = waypointPos.X - targetPosition.X;
            var num2 = waypointPos.Z - targetPosition.Z;
            var distance = num * num + num2 * num2;
            return distance <= squareDistance;
        }

        /// <summary>
        ///     Adds a waypoint at the a position within the world, relative to the global spawn point.
        /// </summary>
        /// <param name="pos">The position to add the waypoint at. World Pos - Not Relative to Spawn!</param>
        /// <param name="icon">The icon to use for the waypoint.</param>
        /// <param name="colour">The colour of the waypoint.</param>    
        /// <param name="title">The title to set.</param>
        /// <param name="pinned">if set to <c>true</c>, the waypoint will be pinned to the world map.</param>
        public static void AddWaypointAtPos(this BlockPos pos, string icon, string colour, string title, bool pinned)
        {
            if (pos is null) return;
            if (pos.WaypointExistsAtPos(p => p.Icon == icon && p.Title == title)) return;
            pos = pos.RelativeToSpawn();
            ApiEx.Client.SendChatMessage($"/waypoint addati {icon} {pos.X} {pos.Y} {pos.Z} {(pinned ? "true" : "false")} {colour} {title}");
        }

        /// <summary>
        ///     Asynchronously adds a waypoint at the a position within the world, relative to the global spawn point.
        /// </summary>
        /// <param name="pos">The position to add the waypoint at. World Pos - Not Relative to Spawn!</param>
        /// <param name="icon">The icon to use for the waypoint.</param>
        /// <param name="colour">The colour of the waypoint.</param>    
        /// <param name="title">The title to set.</param>
        /// <param name="pinned">if set to <c>true</c>, the waypoint will be pinned to the world map.</param>
        public static Task AddWaypointAtPosAsync(this BlockPos pos, string icon, string colour, string title, bool pinned)
        {
            return Task.Factory.StartNew(() => pos.AddWaypointAtPos(icon, colour, title, pinned));
        }

        /// <summary>
        ///     Determines whether a waypoint already exists within a radius of a specific position on the world map.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="horizontalRadius">The number of blocks away from the origin position to scan, on the X and Z axes.</param>
        /// <param name="verticalRadius">The number of blocks away from the origin position to scan, on the Y axis.</param>
        /// <param name="filter">A custom filter, to narrow the scope of the search.</param>
        /// <returns><c>true</c> if a waypoint already exists within range of the specified position, <c>false</c> otherwise.</returns>
        public static bool WaypointExistsWithinRadius(this BlockPos position, int horizontalRadius, int verticalRadius, Func<Waypoint, bool> filter = null)
        {
            var waypointMapLayer = ApiEx.Client.ModLoader.GetModSystem<WorldMapManager>().WaypointMapLayer();
            var waypoints =
                waypointMapLayer.ownWaypoints.Where(wp =>
                    wp.Position.AsBlockPos.InRangeCubic(position, horizontalRadius, verticalRadius)).ToList();
            if (!waypoints.Any()) return false;
            return filter == null || waypoints.Any(filter);
        }

        /// <summary>
        ///     Asynchronously determines whether a waypoint already exists within a radius of a specific position on the world map.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="horizontalRadius">The number of blocks away from the origin position to scan, on the X and Z axes.</param>
        /// <param name="verticalRadius">The number of blocks away from the origin position to scan, on the Y axis.</param>
        /// <param name="filter">A custom filter, to narrow the scope of the search.</param>
        /// <returns><c>true</c> if a waypoint already exists within range of the specified position, <c>false</c> otherwise.</returns>
        public static Task<bool> WaypointExistsWithinRadiusAsync(this BlockPos position, int horizontalRadius, int verticalRadius, Func<Waypoint, bool> filter = null)
        {
            return Task<bool>.Factory.StartNew(() => position.WaypointExistsWithinRadius(horizontalRadius, verticalRadius, filter));
        }

        /// <summary>
        ///     Determines whether a waypoint already exists at a specific position on the world map.
        /// </summary>
        /// <param name="pos">The position to check.</param>
        /// <param name="filter">A custom filter, to narrow the scope of the search.</param>
        /// <returns><c>true</c> if a waypoint already exists at the specified position, <c>false</c> otherwise.</returns>
        public static bool WaypointExistsAtPos(this BlockPos pos, Func<Waypoint, bool> filter = null)
        {
            var waypointMapLayer = ApiEx.Client.ModLoader.GetModSystem<WorldMapManager>().WaypointMapLayer();
            var waypoints = waypointMapLayer.ownWaypoints.Where(wp => wp.Position.AsBlockPos.Equals(pos)).ToList();
            if (!waypoints.Any()) return false;
            return filter == null || waypoints.Any(filter);
        }

        /// <summary>
        ///     Asynchronously determines whether a waypoint already exists at a specific position on the world map.
        /// </summary>
        /// <param name="pos">The position to check.</param>
        /// <param name="filter">A custom filter, to narrow the scope of the search.</param>
        /// <returns><c>true</c> if a waypoint already exists at the specified position, <c>false</c> otherwise.</returns>
        public static Task<bool> WaypointExistsAtPosAsync(this BlockPos pos, Func<Waypoint, bool> filter = null)
        {
            return Task<bool>.Factory.StartNew(() => pos.WaypointExistsAtPos(filter));
        }
    }
}