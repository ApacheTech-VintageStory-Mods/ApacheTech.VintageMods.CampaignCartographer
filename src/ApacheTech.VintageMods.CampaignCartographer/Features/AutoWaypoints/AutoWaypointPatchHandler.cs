using System;
using System.Collections.Generic;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.Extensions;
using Gantry.Core;
using Gantry.Core.DependencyInjection.Annotation;
using Gantry.Core.Extensions;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Features;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Acts as a mediator between block patches, and the waypoint service.
    /// </summary>
    /// <seealso cref="WorldSettingsConsumer{T}" />
    [UsedImplicitly]
    public sealed class AutoWaypointPatchHandler : WorldSettingsConsumer<AutoWaypointsSettings>
    {
        // DEV: Still lots of O/C issues in this class, but without needing
        //      a VERY robust and scalable solution, this will suffice.

        private readonly WaypointTemplateService _service;
        private readonly CrossMaps _crossMaps;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="AutoWaypointPatchHandler"/> class.
        /// </summary>
        [SidedConstructor(EnumAppSide.Client)]
        public AutoWaypointPatchHandler(WaypointTemplateService waypointService, IFileSystemService fileSystemService)
        {
            _service = waypointService;
            _crossMaps = fileSystemService.GetJsonFile("crossmap.json").ParseAs<CrossMaps>();
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
                case BlockMushroom:
                    HandleMushrooms(block);
                    break;
                case BlockBerryBush:
                    HandleBerries(block);
                    break;
                default:
                    // For some reason, the resin log is not of type BlockLog.
                    if (!block.Variant.ContainsKey("type")) break;
                    if (block.Variant["type"].StartsWith("resin"))
                        HandleResin(block);
                    break;
            }
        }

        private void HandleMinerals(Block block)
        {
            if (!HandleLooseOres(block)) HandleLooseStones(block);
        }

        private void HandleMushrooms(Block block)
        {
            if (!Settings.Mushrooms) return;
            AddWaypointFor(block, MapOrganicMaterial(block.Code.Path));
        }

        private void HandleBerries(Block block)
        {
            if (!Settings.BerryBushes) return;
            AddWaypointFor(block, MapOrganicMaterial(block.Code.Path));
        }

        private void HandleResin(Block block)
        {
            if (!Settings.Resin) return;
            AddWaypointFor(block, MapOrganicMaterial(block.Code.Path));
        }

        private void HandleLooseStones(Block block)
        {
            if (!Settings.LooseStones) return;
            AddWaypointFor(block, MapLooseStones(block.Code.Path));
        }

        private bool HandleLooseOres(Block block)
        {
            return Settings.SurfaceDeposits && 
                   AddWaypointFor(block, MapSurfaceOre(block.Code.Path));
        }

        private string MapSurfaceOre(string assetCode)
        {
            return Map(_crossMaps.Ores, assetCode);
        }

        private string MapLooseStones(string assetCode)
        {
            return Map(_crossMaps.Stones, assetCode);
        }

        private string MapOrganicMaterial(string assetCode)
        {
            return Map(_crossMaps.Organics, assetCode);
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
            var waypoint = _service.GetTemplateByKey(syntax);

            if (waypoint is null)
            {
                ApiEx.Client.EnqueueShowChatMessage(LangEx.FeatureString("PredefinedWaypoints", "InvalidSyntax", syntax));
                return false;
            }

            waypoint
                .With(p => p.Title = title)
                .AddToMap(position);
            return true;
        }
    }
}