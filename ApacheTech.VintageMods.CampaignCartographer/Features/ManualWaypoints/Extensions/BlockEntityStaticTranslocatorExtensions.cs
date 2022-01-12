using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Extensions
{
    public static class BlockEntityStaticTranslocatorExtensions
    {
        private static readonly WaypointService WaypointService;

        /// <summary>
        /// 	Initialises static members of the <see cref="BlockEntityStaticTranslocatorExtensions"/> class.
        /// </summary>
        static BlockEntityStaticTranslocatorExtensions()
        {
            WaypointService = ModServices.IOC.Resolve<WaypointService>();
        }

        /// <summary>
        ///     Adds waypoints at each end of a fixed translocator.
        /// </summary>
        /// <param name="translocator">The translocator to add the waypoints to.</param>
        /// <param name="titleTemplate">The title of the waypoint. Can include format placeholders for X, Y, and Z.</param>
        public static void AddWaypointsForEndpoints(this BlockEntityStaticTranslocator translocator, string titleTemplate)
        {
            var blockPos = translocator.Pos;
            var targetPos = translocator.TargetLocation;
            AddWaypoint(blockPos, targetPos, titleTemplate);
            AddWaypoint(targetPos, blockPos, titleTemplate);
        }

        /// <summary>
        ///     Adds waypoints at each end of a fixed translocator.
        /// </summary>
        /// <param name="sourcePos"></param>
        /// <param name="destPos"></param>
        /// <param name="titleTemplate"></param>
        private static void AddWaypoint(BlockPos sourcePos, BlockPos destPos, string titleTemplate)
        {
            var displayPos = destPos.RelativeToSpawn();
            var message = Lang.Get(titleTemplate, displayPos.X, displayPos.Y, displayPos.Z);

            WaypointService.GetWaypointModel("tl")?
                .With(p =>
                {
                    p.Title = message;
                })
                .AddToMap(sourcePos);

            ApiEx.Client.Logger.VerboseDebug($"Added Waypoint: Translocator to ({displayPos.X}, {displayPos.Y}, {displayPos.Z})");
        }
    }
}