using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

// ReSharper disable UnusedType.Global
// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Add a waypoint to a trader, within five blocks of the player. (.wpt)
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    public sealed class TraderWaypoints : ClientModSystem
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
            FluentChat.ClientCommand("wpt")
                .RegisterWith(_capi = capi)
                .HasDescription(LangEx.FeatureString("ManualWaypoints.TraderWaypoints", "Description"))
                .HasDefaultHandler(DefaultHandler);
        }

        private void DefaultHandler(int groupId, CmdArgs args)
        {
            var found = false;

            var trader = (EntityTrader)_capi.World.GetNearestEntity(_capi.World.Player.Entity.Pos.XYZ, 10f, 10f, p =>
            {
                if (!p.Code.Path.StartsWith("humanoid-trader-") || !p.Alive) return false;
                found = true;
                return true;
            });

            if (!found)
            {
                _capi.ShowChatMessage(LangEx.FeatureString("ManualWaypoints.TraderWaypoints", "TraderNotFound"));
            }
            else
            {
                var displayName = trader.GetBehavior<EntityBehaviorNameTag>().DisplayName;
                var wpTitle = Lang.Get("tradingwindow-" + trader.Code.Path, displayName);

                _waypointService.GetWaypointModel("trader")?
                    .With(p =>
                    {
                        p.Title = wpTitle;
                        p.Colour = TraderModel.GetColourFor(trader);
                    })
                    .AddToMap(trader.Pos.AsBlockPos);
            }
        }
    }
}