using System.Text;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.Extensions;
using Gantry.Core;
using Gantry.Core.Extensions;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Commands
{
    /// <summary>
    ///     Quickly and easily add waypoints at your current position, via the chat window.
    ///     There are over 130 pre-defined waypoints for many different block types, and areas of interest.
    /// </summary>
    /// <seealso cref="ClientChatCommand" />
    [UsedImplicitly]
    public sealed class PredefinedWaypointsChatCommand : ClientChatCommand
    {
        private readonly WaypointTemplateService _waypointService;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="PredefinedWaypointsChatCommand"/> class.
        /// </summary>
        /// <param name="waypointService">The waypoint service.</param>
        public PredefinedWaypointsChatCommand(WaypointTemplateService waypointService)
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
            
            var syntax = option.ToLowerInvariant();
            var waypoint = _waypointService.GetTemplateByKey(syntax);

            if (waypoint is null)
            {
                ApiEx.Client.EnqueueShowChatMessage(LangEx.FeatureString("PredefinedWaypoints", "InvalidSyntax", syntax));
                return;
            }

            waypoint
                .With(p =>
                {
                    p.Title = args.PopAll().IfNullOrWhitespace(p.Title);
                    p.Pinned |= pin;
                })
                .AddToMap();
        }

        /// <summary>
        ///     Gets the help message of the command.
        /// </summary>
        /// <returns>A string representation of the localised help message of the command.</returns>
        public override string GetHelpMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine(LangEx.FeatureString("PredefinedWaypoints.PredefinedWaypoints", "Title"));
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
            return LangEx.FeatureString("PredefinedWaypoints.PredefinedWaypoints", "SyntaxMessage_Full", _waypointService.GetSyntaxListText());
        }

        /// <summary>
        ///     Gets the description of the command.
        /// </summary>
        /// <returns>A string representation of the localised description of the command.</returns>
        public override string GetDescription()
        {
            return LangEx.FeatureString("PredefinedWaypoints.PredefinedWaypoints", "Description");
        }
    }
}