using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.DotNet;
using ApacheTech.VintageMods.Core.Extensions.Game;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
// ReSharper disable CompareOfFloatsByEqualityOperator

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions
{
    public static class WaypointExtensions
    {
        private static readonly List<BlockPos> PositionsBeingHandled = new();

        /// <summary>
        ///     Adds a <see cref="ManualWaypointTemplateModel"/> to the world map. These are waypoints that haven't been added before.
        /// </summary>
        /// <param name="waypoint">The waypoint.</param>
        /// <param name="position">The position.</param>
        /// <param name="pinned">The pinned.</param>
        public static void AddToMap(this ManualWaypointTemplateModel waypoint, BlockPos position = null, bool pinned = false)
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
                if (position.WaypointExistsWithinRadius(waypoint.HorizontalCoverageRadius, waypoint.VerticalCoverageRadius,
                        p =>
                        {
                            var sameIcons = p.Icon.EndsWith(waypoint.DisplayedIcon, StringComparison.InvariantCultureIgnoreCase);
                            var sameColour = p.Color == waypoint.Colour.ColourValue();
                            return sameIcons && sameColour;
                        })) return;
                ApiEx.ClientMain.EnqueueMainThreadTask(() =>
                {
                    position.AddWaypointAtPos(waypoint.DisplayedIcon.ToLower(), waypoint.Colour.ToLower(), waypoint.Title, pinned);
                }, "");
            }
            finally
            {
                ApiEx.Client.RegisterDelayedCallback(_ => { PositionsBeingHandled.Remove(position); }, 1000);
            }
        }

        /// <summary>
        ///     Adds a <see cref="WaypointDto"/> to the world map. These are waypoints that are being imported into the game, and have a position already.
        /// </summary>
        /// <param name="waypoint">The waypoint to add.</param>
        public static void AddToMap(this WaypointDto waypoint)
        {
            // DEV NOTE:    This method looks needlessly complicated because of race conditions when running in single-player mode.
            //              In these cases, it's possible for this method to be run multiple times, resulting in multiple waypoints
            //              being added. This was intended as a temporary fix, however, it seems like without adding a global cooldown
            //              for waypoint related actions, this is the best way to resolve the issues. This still enables waypoints
            //              to be added in rapid succession, but limits the calls to one pass-through per block, per second.

            var position = waypoint.Position;
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
        ///     Determines whether the specified waypoint is within a set horizontal distance of a specific block, or location within the game world.
        /// </summary>
        /// <param name="waypoint">The waypoint.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="squareDistance">The maximum square distance tolerance level.</param>
        /// <returns>System.Boolean.</returns>
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
        /// <param name="allowDuplicates">if set to <c>true</c>, the waypoint will not be placed, if another similar waypoint already exists at that position.</param>
        public static void AddWaypointAtPos(this BlockPos pos, string icon, string colour, string title, bool pinned, bool allowDuplicates = false)
        {
            if (pos is null) return;
            if (!allowDuplicates)
            {
                if (pos.WaypointExistsAtPos(p => p.Icon == icon)) return;
            }
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

        /// <summary>
        ///     The game does not implement any way of uniquely identifying waypoints, nor does it set waypoint objects as ValueTypes.
        ///     So this is a memberwise equality checker, to see if one waypoint is the same as another waypoint, when jumping through the numerous hoops required.
        ///     This method should not be needed... but here we are.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.Boolean.</returns>
        public static bool IsSameAs(this Waypoint source, Waypoint target)
        {
            if (source.Color != target.Color) return false;
            if (source.OwningPlayerGroupId != target.OwningPlayerGroupId) return false;
            if (source.Icon != target.Icon) return false;
            if (source.Position.X != target.Position.X) return false;
            if (source.Position.Y != target.Position.Y) return false;
            if (source.Position.Z != target.Position.Z) return false;
            if (source.Text != target.Text) return false;
            if (source.OwningPlayerUid != target.OwningPlayerUid) return false;
            if (source.Pinned != target.Pinned) return false;
            if (source.ShowInWorld != target.ShowInWorld) return false;
            if (source.Temporary != target.Temporary) return false;
            if (source.Title != target.Title) return false;
            return true;
        }
    }
}