using ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Core.Extensions.Api;
using Gantry.Core.ModSystems;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Add a waypoint to a translocator, within five blocks of the player. ***`(.wptl)`***
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
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
                .HasDescription(LangEx.FeatureString("PredefinedWaypoints.TranslocatorWaypoints", "Description"))
                .HasDefaultHandler(DefaultHandler);
        }

        private void DefaultHandler(int groupId, CmdArgs args)
        {
            var pos = _capi.World.Player.Entity.Pos.AsBlockPos;
            var block = _capi.World.GetNearestBlock<BlockStaticTranslocator>(pos, 5f, 1f, out var blockPos);

            if (block is null)
            {
                var translocatorNotFoundMessage = LangEx.FeatureString("PredefinedWaypoints.TranslocatorWaypoints", "TranslocatorNotFound");
                _capi.ShowChatMessage(translocatorNotFoundMessage);
                return;
            }
            block.ProcessWaypoints(blockPos);
        }
    }
}