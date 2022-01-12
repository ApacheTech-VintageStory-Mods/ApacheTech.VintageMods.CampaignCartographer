using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Extensions
{
    public static class BlockEntityTeleporterExtensions
    {
        private static readonly WaypointService WaypointService;

        /// <summary>
        /// 	Initialises static members of the <see cref="BlockEntityTeleporterExtensions"/> class.
        /// </summary>
        static BlockEntityTeleporterExtensions()
        {
            WaypointService = ModServices.IOC.Resolve<WaypointService>();
        }

        /// <summary>
        ///     Adds waypoints at each end of a teleporter.
        /// </summary>
        /// <param name="teleporter">The teleporter to add the waypoints to.</param>
        /// <param name="titleTemplate">The code within the lang file, for the title template of the waypoint.</param>
        public static void AddWaypoint(this BlockEntityTeleporterBase teleporter, string titleTemplate)
        {
            var tpLocation = teleporter.GetField<TeleporterLocation>("tpLocation");
            var sourcePos = teleporter.Pos;

            if (sourcePos.WaypointExistsAtPos(p => p.Icon == WaypointIcon.Spiral)) return;

            var sourceName = tpLocation.SourceName?.IfNullOrWhitespace("Unknown");
            var targetName = tpLocation.TargetName?.IfNullOrWhitespace("Unknown");

            var title = Lang.Get(titleTemplate, sourceName, targetName);

            WaypointService.GetWaypointModel("tl")?
                .With(p =>
                {
                    p.Title = title;
                    p.Colour = NamedColour.SpringGreen;
                })
                .AddToMap(sourcePos);
            
            ApiEx.Client.Logger.VerboseDebug($"Added Waypoint: {title}");
        }
    }
}