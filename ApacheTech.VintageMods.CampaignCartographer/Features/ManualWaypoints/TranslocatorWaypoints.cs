using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Extensions;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

// ReSharper disable UnusedType.Global
// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Add a waypoint to a translocator, within five blocks of the player. ***`(.wptl)`***
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    public sealed class TranslocatorWaypoints : ClientModSystem
    {
        private ICoreClientAPI _capi;
        private WaypointService _waypointService;

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The core API implemented by the client. The main interface for accessing the client. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            _waypointService = ModServices.IOC.Resolve<WaypointService>();
            FluentChat.ClientCommand("wptl")
                .RegisterWith(_capi = capi)
                .HasDescription(LangEx.FeatureString("ManualWaypoints.TranslocatorWaypoints", "Description"))
                .HasDefaultHandler(DefaultHandler)
                .HasSubCommand("auto").WithHandler(OnAutoSubCommand);
        }

        private void DefaultHandler(int groupId, CmdArgs args)
        {
            var pos = _capi.World.Player.Entity.Pos.AsBlockPos;
            var block = _capi.World.GetNearestBlock<BlockStaticTranslocator>(pos, 5f, 1f, out var blockPos);

            if (block is null)
            {
                var translocatorNotFoundMessage = LangEx.FeatureString("ManualWaypoints.TranslocatorWaypoints", "TranslocatorNotFound");
                _capi.ShowChatMessage(translocatorNotFoundMessage);
                return;
            }

            if (!block.Repaired)
            {
                var message = LangEx.FeatureString("ManualWaypoints.TranslocatorWaypoints", "BrokenTranslocatorTitle");
                var displayPos = blockPos.RelativeToSpawn();
                if (blockPos.WaypointExistsAtPos(p => p.Icon == WaypointIcon.Spiral)) return;

                _waypointService.GetWaypointModel("tl")?
                    .With(p =>
                    {
                        p.DefaultTitle = message;
                        p.Colour = NamedColour.Red;
                    })
                    .AddToMap(blockPos);

                ApiEx.Client.Logger.VerboseDebug($"Added Waypoint: Broken Translocator at ({displayPos.X}, {displayPos.Y}, {displayPos.Z})");
                return;
            }

            var translocator = (BlockEntityStaticTranslocator)_capi.World.GetBlockAccessorPrefetch(false, false).GetBlockEntity(blockPos);
            if (!translocator.FullyRepaired) return;

            var titleTemplate = LangEx.FeatureCode("ManualWaypoints.TranslocatorWaypoints", "TranslocatorWaypointTitle");
            translocator.AddWaypointsForEndpoints(titleTemplate);
        }

        private static void OnAutoSubCommand(string subCommandName, int groupId, CmdArgs args)
        {
            var settings = ModServices.IOC.Resolve<AutoWaypointsSettings>();
            settings.Translocators = !settings.Translocators;
            var state = LangEx.BooleanString(settings.Translocators);
            var message = LangEx.FeatureString("AutoWaypoints.TranslocatorWaypoints", "AutoTranslocatorsEnabled", state);
            ApiEx.Client.ShowChatMessage(message);
        }
    }
}