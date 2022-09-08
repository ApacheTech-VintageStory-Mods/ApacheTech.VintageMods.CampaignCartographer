using ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Core.Extensions.Api;
using Gantry.Core.ModSystems;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Add a waypoint to a teleporter block, within five blocks of the player.(.wptp)
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class TeleporterWaypoints : ClientModSystem
    {
        private ICoreClientAPI _capi;

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The core API implemented by the client. The main interface for accessing the client. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            FluentChat.ClientCommand("wptp")
                .RegisterWith(_capi = capi)
                .HasDescription(LangEx.FeatureString("PredefinedWaypoints.TeleporterWaypoints", "Description"))
                .HasDefaultHandler(DefaultHandler);
        }

        private void DefaultHandler(int groupId, CmdArgs args)
        {
            var found = false;
            bool Predicate(BlockEntityTeleporter p)
            {
                return found = true;
            }

            var pos = _capi.World.Player.Entity.Pos.AsBlockPos;
            var teleporter = _capi.World.GetNearestBlockEntity<BlockEntityTeleporter>(pos, 5f, 1f, Predicate);

            if (!found)
            {
                var teleporterNotFoundMessage = LangEx.FeatureString("PredefinedWaypoints.TeleporterWaypoints", "TeleporterNotFound");
                _capi.ShowChatMessage(teleporterNotFoundMessage);
                return;
            }
            
            var titleTemplate = LangEx.FeatureCode("PredefinedWaypoints.TeleporterWaypoints", "TeleporterWaypointTitle");
            teleporter.AddWaypoint(titleTemplate);
        }
    }
}