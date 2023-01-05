using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.Extensions;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.ModSystems;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Systems
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Add a waypoint to a trader, within five blocks of the player. (.wpt)
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class TraderWaypoints : ClientModSystem
    {
        private ICoreClientAPI _capi;
        private WaypointTemplateService _waypointService;

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The core API implemented by the client. The main interface for accessing the client. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            _waypointService = IOC.Services.Resolve<WaypointTemplateService>();
            FluentChat.RegisterCommand("wpt", _capi = capi)!
                .WithDescription(LangEx.FeatureString("PredefinedWaypoints.TraderWaypoints", "Description"))
                .WithHandler(DefaultHandler);
        }

        private void DefaultHandler(IPlayer player, int groupId, CmdArgs args)
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
                _capi.ShowChatMessage(LangEx.FeatureString("PredefinedWaypoints.TraderWaypoints", "TraderNotFound"));
            }
            else
            {
                var displayName = trader.GetBehavior<EntityBehaviorNameTag>().DisplayName;
                var wpTitle = Lang.Get("tradingwindow-" + trader.Code.Path, displayName);

                _waypointService.GetTemplateByKey("trader")?
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