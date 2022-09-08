using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.Extensions;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.Extensions.GameContent;
using Gantry.Core.GameContent.AssetEnum;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions
{
    public static class BlockEntityTeleporterExtensions
    {
        private static readonly WaypointTemplateService WaypointService;

        /// <summary>
        /// 	Initialises static members of the <see cref="BlockEntityTeleporterExtensions"/> class.
        /// </summary>
        static BlockEntityTeleporterExtensions()
        {
            WaypointService = IOC.Services.Resolve<WaypointTemplateService>();
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

            WaypointService.GetTemplateByKey("tl")?
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