using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.Extensions;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Core.ModSystems;
using Gantry.Services.FileSystem.Configuration;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Add a waypoint for the block the player is currently targetting. `(.wps)`
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class BlockSelectionWaypoints : ClientModSystem
    {
        private ICoreClientAPI _capi;

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">
        ///     The core API implemented by the client.
        ///     The main interface for accessing the client.
        ///     Contains all sub-components, and some miscellaneous methods.
        /// </param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            FluentChat.ClientCommand("wps")
                .RegisterWith(_capi = capi)
                .HasDescription(LangEx.FeatureString("PredefinedWaypoints.BlockSelectionWaypoints", "Description"))
                .HasDefaultHandler(DefaultHandler);
        }

        private void DefaultHandler(int groupId, CmdArgs args)
        {
            var blockSelection = _capi.World.Player.CurrentBlockSelection;
            if (blockSelection is null) return;
            var position = blockSelection.Position;
            var block = _capi.World.BlockAccessor.GetBlock(position, BlockLayersAccess.Default);
            var title = block.GetPlacedBlockName(_capi.World, position);
            
            var template = ModSettings.World.
                Feature<PredefinedWaypointsSettings>()
                .BlockSelectionWaypointTemplate;

            var waypoint = new PredefinedWaypointTemplate
            {
                Colour = template.Colour,
                DisplayedIcon = template.DisplayedIcon,
                ServerIcon = template.ServerIcon,
                Title = title,
                HorizontalCoverageRadius = template.HorizontalCoverageRadius,
                VerticalCoverageRadius = template.VerticalCoverageRadius
            };
            waypoint.AddToMap(position);
        }
    }
}