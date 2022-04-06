using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Extensions;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

// ReSharper disable CommentTypo
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

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The core API implemented by the client. The main interface for accessing the client. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            FluentChat.ClientCommand("wptl")
                .RegisterWith(_capi = capi)
                .HasDescription(LangEx.FeatureString("ManualWaypoints.TranslocatorWaypoints", "Description"))
                .HasDefaultHandler(DefaultHandler);
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
            block.ProcessWaypoints(blockPos);
        }
    }
}