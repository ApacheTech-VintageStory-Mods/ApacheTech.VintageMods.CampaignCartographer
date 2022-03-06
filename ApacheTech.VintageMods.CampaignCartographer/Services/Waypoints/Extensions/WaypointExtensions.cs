using System;
using System.Collections.Generic;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.DotNet;
using ApacheTech.VintageMods.Core.Extensions.Game;
using Vintagestory.API.MathTools;

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
        public static void AddToMap(this ManualWaypointTemplateModel waypoint, BlockPos position = null)
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
                    position.AddWaypointAtPos(waypoint.DisplayedIcon.ToLower(), waypoint.Colour.ToLower(), waypoint.Title, waypoint.Pinned);
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
    }
}