using System;
using System.Threading.Tasks;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.Repositories;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.Extensions;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.Extensions;
using Gantry.Core.Extensions.GameContent;
using Gantry.Core.GameContent.AssetEnum;
using JetBrains.Annotations;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class BlockEntityStaticTranslocatorExtensions
    {
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
            var service = IOC.Services.Resolve<WaypointTemplateService>();
            var repo = IOC.Services.Resolve<WaypointCommandsRepository>();
            var model = service.GetTemplateByKey("tl");

            bool Filter(Waypoint p)
            {
                var iconMatches = string.Equals(p.Icon, model.ServerIcon, StringComparison.InvariantCultureIgnoreCase);
                var colourMatches = p.Color == NamedColour.Red.ToArgb();
                return iconMatches && colourMatches;
            }

            var retVal = await pos.WaypointExistsAtPosAsync(Filter);
            if (retVal) repo.RemoveAllWaypointsAtPosition(pos);

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

            var service = IOC.Services.Resolve<WaypointTemplateService>();
            service.GetTemplateByKey("tl")?
                .With(p =>
                {
                    p.Title = message;
                })
                .AddToMap(sourcePos, forceWaypoint);

            ApiEx.Client.Logger.VerboseDebug($"Added Waypoint: Translocator to ({displayPos.X}, {displayPos.Y}, {displayPos.Z})");
        }

        public static void AddBrokenTranslocatorWaypoint(this BlockStaticTranslocator block, BlockPos blockPos)
        {
            var message = LangEx.FeatureString("PredefinedWaypoints.TranslocatorWaypoints", "BrokenTranslocatorTitle");
            var displayPos = blockPos.RelativeToSpawn();
            if (blockPos.WaypointExistsAtPos(p => p.Icon == WaypointIcon.Spiral)) return;

            var service = IOC.Services.Resolve<WaypointTemplateService>();
            service.GetTemplateByKey("tl")?
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
            var titleTemplate = LangEx.FeatureCode("PredefinedWaypoints.TranslocatorWaypoints", "TranslocatorWaypointTitle");
            translocator.AddWaypointsForEndpoints(titleTemplate);
        }
    }
}