using System;
using System.Threading;
using System.Threading.Tasks;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions;
using ApacheTech.VintageMods.Core.Extensions.DotNet;
using ApacheTech.VintageMods.Core.Extensions.Game;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints
{
    public sealed class WaypointPurger
    {
        private readonly ICoreClientAPI _capi;
        private readonly WorldMapManager _worldMap;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointPurger"/> class.
        /// </summary>
        /// <param name="capi">The capi.</param>
        /// <param name="worldMap">The world map.</param>
        public WaypointPurger(ICoreClientAPI capi, WorldMapManager worldMap)
        {
            _capi = capi;
            _worldMap = worldMap;
        }

        /// <summary>
        ///     Purges waypoints within a given radius of a block position.
        /// </summary>
        /// <param name="origin">The position to scan for waypoints around.</param>
        /// <param name="radius">The radius at which to scan.</param>
        public void Around(BlockPos origin, float radius)
        {
            Task.Factory.StartNew(() =>
                PurgeWaypointsAsync(p => p.IsInHorizontalRangeOf(origin, radius * radius)));
        }

        /// <summary>
        ///     Purges waypoints within a given radius of the player
        /// </summary>
        /// <param name="radius">The radius at which to scan.</param>
        public void NearPlayer(float radius)
        {
            var playerPos = _capi.World.Player.Entity.Pos.AsBlockPos;
            Around(playerPos, radius);
        }

        /// <summary>
        ///     Purges waypoints that match a specified icon.
        /// </summary>
        /// <param name="icon">The icon to scan for.</param>
        public void ByIcon(string icon)
        {
            Task.Factory.StartNew(() =>
                PurgeWaypointsAsync(p => p.Icon.Equals(icon, StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        ///     Purges all waypoints. use with caution. Irreversible.
        /// </summary>
        public void All()
        {
            Task.Factory.StartNew(() =>
                PurgeWaypointsAsync(_ => true));
        }

        /// <summary>
        ///     Purges waypoints that match a specified colour.
        /// </summary>
        /// <param name="colour">The colour to scan for.</param>
        public void ByColour(string colour)
        {
            Task.Factory.StartNew(() =>
                PurgeWaypointsAsync(p => p.Color == colour.ColourValue()));
        }

        /// <summary>
        ///     Purges waypoints where the title starts with a specified value.
        /// </summary>
        /// <param name="partialTitle">The partial title to scan for.</param>
        public void ByTitle(string partialTitle)
        {
            Task.Factory.StartNew(() =>
                PurgeWaypointsAsync(p => p.Title.StartsWith(partialTitle, StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        /// Ats the position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void AtPosition(BlockPos position)
        {
            Task.Factory.StartNew(() =>
                PurgeWaypointsAsync(p => p.Position.AsBlockPos.Equals(position)));
        }

        private async Task PurgeWaypointsAsync(Func<Waypoint, bool> predicate)
        {
            var purgedCount = 0;
            var i = _worldMap.WaypointMapLayer().ownWaypoints.Count - 1;
            while (i > 0)
            {
                var waypoints = await Task.FromResult(_worldMap.WaypointMapLayer().ownWaypoints);
                var waypoint = waypoints[i];
                if (predicate(waypoint))
                {
                    purgedCount++;
                    _capi.SendChatMessage($"/waypoint remove {i}");
                    Thread.Sleep(0);
                }
                i--;
            }
            var waypointCode = LangEx.FeatureCode("WaypointUtil.Dialogue.Imports", "Waypoint");
            _capi.EnqueueShowChatMessage(
                LangEx.FeatureString("WaypointUtil", "PurgedWaypointCount", 
                    purgedCount.FormatLargeNumber(), 
                    LangEx.Pluralise(waypointCode, purgedCount)));
        }
    }
}