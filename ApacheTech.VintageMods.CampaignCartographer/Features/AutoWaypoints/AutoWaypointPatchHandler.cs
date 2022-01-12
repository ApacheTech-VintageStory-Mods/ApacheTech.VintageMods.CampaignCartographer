using System;
using System.Collections.Generic;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Extensions.System;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Acts as a mediator between block patches, and the waypoint service.
    /// </summary>
    /// <seealso cref="WorldSettingsConsumer{AutoWaypointsSettings}" />
    public sealed class AutoWaypointPatchHandler : WorldSettingsConsumer<AutoWaypointsSettings>
    {
        // DEV: Still lots of O/C issues in this class, but without needing
        //      a VERY robust and scalable solution, this will suffice.

        private readonly WaypointService _service;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="AutoWaypointPatchHandler"/> class.
        /// </summary>
        public AutoWaypointPatchHandler()
        {
            _service = ModServices.IOC.Resolve<WaypointService>();
        }

        /// <summary>
        ///     Called when a player interacts with a block.
        ///     Filters out uninteresting blocks, and passes interesting blocks to the waypoint service for processing.
        /// </summary>
        /// <param name="block">The block.</param>
        public void HandleInteraction(Block block)
        {
            switch (block)
            {
                case BlockLooseOres:
                    HandleMinerals(block);
                    break;
                case BlockLooseStones:
                    HandleLooseStones(block);
                    break;
                case BlockMushroom or BlockBerryBush or BlockLog:
                    HandleOrganics(block);
                    break;
                default:
                    // For some reason, the resin log is not of type BlockLog.
                    if (!block.Variant.ContainsKey("type")) break;
                    if (block.Variant["type"].StartsWith("resin"))
                        HandleOrganics(block);
                    break;
            }
        }

        private void HandleMinerals(Block block)
        {
            if (!HandleLooseOres(block)) HandleLooseStones(block);
        }

        private void HandleOrganics(Block block)
        {
            if (!Settings.Organics) return;
            AddWaypointFor(block, MapOrganicMaterial(block.Code.Path));
        }

        private void HandleLooseStones(Block block)
        {
            if (!Settings.LooseStones) return;
            AddWaypointFor(block, MapLooseStones(block.Code.Path));
        }

        private bool HandleLooseOres(Block block)
        {
            if (!Settings.SurfaceDeposits) return false;
            return AddWaypointFor(block, MapSurfaceOre(block.Code.Path));
        }

        private static string MapSurfaceOre(string assetCode)
        {
            return Map(CrossMaps.Ores, assetCode);
        }

        private static string MapLooseStones(string assetCode)
        {
            return Map(CrossMaps.Stones, assetCode);
        }

        private static string MapOrganicMaterial(string assetCode)
        {
            return Map(CrossMaps.Organics, assetCode);
        }

        private static string Map(IDictionary<string, string> dictionary, string assetCode)
        {
            var value = dictionary.FirstOrNull(p => assetCode.Contains(p.Key));
            if (!string.IsNullOrWhiteSpace(value)) return value;
            var exception = new ArgumentOutOfRangeException(nameof(assetCode), assetCode,
                "The given value was not found within the collection.");
            ApiEx.Client.Logger.Error(exception.ToString());
            return null;
        }

        private bool AddWaypointFor(Block block, string syntax)
        {
            if (string.IsNullOrWhiteSpace(syntax)) return false;
            var position = ApiEx.ClientMain.Player.Entity.BlockSelection.Position;
            var title = block.GetPlacedBlockName(ApiEx.ClientMain, position);
            var waypoint = _service.GetWaypointModel(syntax);

            if (waypoint is null)
            {
                ApiEx.Client.EnqueueShowChatMessage(LangEx.FeatureString("ManualWaypoints", "InvalidSyntax", syntax));
                return false;
            }

            waypoint?
                .With(p => p.Title = title)
                .AddToMap(position);
            return true;
        }
    }
}