using System.Text;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using Vintagestory.API.Common;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Commands
{
    /// <summary>
    ///     Quickly and easily add waypoints at your current position, via the chat window.
    ///     There are over 130 pre-defined waypoints for many different block types, and areas of interest.
    /// </summary>
    /// <seealso cref="ClientChatCommand" />
    public sealed class ManualWaypointsChatCommand : ClientChatCommand
    {
        private readonly WaypointService _waypointService;
        private string SyntaxList => _waypointService is null
            ? "---" 
            : string.Join(" | ", _waypointService.WaypointTypes.Keys);

        /// <summary>
        /// 	Initialises a new instance of the <see cref="ManualWaypointsChatCommand"/> class.
        /// </summary>
        /// <param name="waypointService">The waypoint service.</param>
        public ManualWaypointsChatCommand(WaypointService waypointService)
        {
            _waypointService = waypointService;
            Command = "wp";
            Description = GetDescription();
            Syntax = GetSyntax();
        }

        /// <summary>
        ///     Handles calls to the .wp command.
        /// </summary>
        /// <param name="player">The player that called the command.</param>
        /// <param name="groupId">The chat channel the command was called from.</param>
        /// <param name="args">The arguments passed into the command.</param>
        public override void CallHandler(IPlayer player, int groupId, CmdArgs args)
        {
            if (args.Length == 0)
            {
                ApiEx.Client.ShowChatMessage(GetHelpMessage());
                return;
            }

            var option = args.PopWord();
            var pin = false;
            if (option == "pin")
            {
                pin = true;
                option = args.PopWord("");
            }

            var pos = ApiEx.Client.World.Player.Entity.Pos.AsBlockPos;
            var syntax = option.ToLowerInvariant();
            var waypoint = _waypointService.GetWaypointModel(syntax);

            if (waypoint is null)
            {
                ApiEx.Client.EnqueueShowChatMessage(LangEx.FeatureString("ManualWaypoints", "InvalidSyntax", syntax));
                return;
            }

            waypoint
                .With(p => p.Title = args.PopAll().IfNullOrWhitespace(p.Title))
                .AddToMap(pos, pin);
        }

        /// <summary>
        ///     Gets the help message of the command.
        /// </summary>
        /// <returns>A string representation of the localised help message of the command.</returns>
        public override string GetHelpMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine(LangEx.FeatureString("ManualWaypoints.ManualWaypoints", "Title"));
            sb.AppendLine();
            sb.AppendLine(GetDescription());
            sb.AppendLine();
            sb.AppendLine(GetSyntax());
            return sb.ToString();
        }

        /// <summary>
        ///     Gets the syntax message for the command.
        /// </summary>
        /// <returns>A string representation of the localised syntax of the command.</returns>
        public override string GetSyntax()
        {
            return LangEx.FeatureString("ManualWaypoints.ManualWaypoints", "SyntaxMessage_Full", SyntaxList);
        }

        /// <summary>
        ///     Gets the description of the command.
        /// </summary>
        /// <returns>A string representation of the localised description of the command.</returns>
        public override string GetDescription()
        {
            return LangEx.FeatureString("ManualWaypoints.ManualWaypoints", "Description");
        }
    }
}