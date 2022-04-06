using System;
using System.Threading.Tasks;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.DotNet;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
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
            Task.Factory.StartNew(async () =>
            {
                var blockPos = translocator.Pos;
                var targetPos = translocator.TargetLocation;
                await AddWaypointAsync(blockPos, targetPos, titleTemplate);
                await AddWaypointAsync(targetPos, blockPos, titleTemplate);
            });
        }

        private static async Task<bool> ClearWaypointsForEndpoint(BlockPos pos)
        {
            var model = WaypointService.GetWaypointModel("tl");

            bool Filter(Waypoint p)
            {
                var iconMatches = string.Equals(p.Icon, model.ServerIcon, StringComparison.InvariantCultureIgnoreCase);
                var colourMatches = p.Color == NamedColour.Red.ToArgb();
                return iconMatches && colourMatches;
            }

            var retVal = await pos.WaypointExistsAtPosAsync(Filter);
            if (retVal) await WaypointService.Purge.AtPositionAsync(pos, silent: true);

            return retVal;
        }



        /// <summary>
        ///     Adds waypoints at each end of a fixed translocator.
        /// </summary>
        /// <param name="sourcePos"></param>
        /// <param name="destPos"></param>
        /// <param name="titleTemplate"></param>
        private static async Task AddWaypointAsync(BlockPos sourcePos, BlockPos destPos, string titleTemplate)
        {
            var displayPos = destPos.RelativeToSpawn();
            var message = Lang.Get(titleTemplate, displayPos.X, displayPos.Y, displayPos.Z);
            var forceWaypoint = await ClearWaypointsForEndpoint(sourcePos);

            WaypointService.GetWaypointModel("tl")?
                .With(p =>
                {
                    p.Title = message;
                })
                .AddToMap(sourcePos, forceWaypoint);

            ApiEx.Client.Logger.VerboseDebug($"Added Waypoint: Translocator to ({displayPos.X}, {displayPos.Y}, {displayPos.Z})");
        }

        public static void AddBrokenTranslocatorWaypoint(this BlockStaticTranslocator block, BlockPos blockPos)
        {
            var message = LangEx.FeatureString("ManualWaypoints.TranslocatorWaypoints", "BrokenTranslocatorTitle");
            var displayPos = blockPos.RelativeToSpawn();
            if (blockPos.WaypointExistsAtPos(p => p.Icon == WaypointIcon.Spiral)) return;

            WaypointService.GetWaypointModel("tl")?
                .With(p =>
                {
                    p.Title = message;
                    p.Colour = NamedColour.Red;
                })
                .AddToMap(blockPos);

            ApiEx.Client.Logger.VerboseDebug($"Added Waypoint: Broken Translocator at ({displayPos.X}, {displayPos.Y}, {displayPos.Z})");
        }

        public static void ProcessWaypoints(this BlockStaticTranslocator block, BlockPos blockPos)
        {
            var translocator = (BlockEntityStaticTranslocator)ApiEx.Client.World.GetBlockAccessorPrefetch(false, false).GetBlockEntity(blockPos);
            if (!block.Repaired || translocator is null || !translocator.GetField<bool>("canTeleport"))
            {
                block.AddBrokenTranslocatorWaypoint(blockPos);
                return;
            }
            if (!translocator.FullyRepaired) return;
            var titleTemplate = LangEx.FeatureCode("ManualWaypoints.TranslocatorWaypoints", "TranslocatorWaypointTitle");
            translocator.AddWaypointsForEndpoints(titleTemplate);
        }
    }
}