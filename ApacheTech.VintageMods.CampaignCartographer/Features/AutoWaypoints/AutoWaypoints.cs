using ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Dialogue;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Automatic Waypoint Addition (.wpAuto)
    ///      - Contains a GUI that can be used to control the settings for the feature (Shift + F7).
    ///      - Enable / Disable all automatic waypoint placements.
    ///      - Automatically add waypoints for Translocators, as the player travels between them.
    ///      - Automatically add waypoints for Teleporters, as the player travels between them.
    ///      - Automatically add waypoints for Traders, as the player interacts with them.
    ///      - Automatically add waypoints for Meteors, when the player punches a Meteoric Iron Block.
    ///      - Server: Send Teleporter information to clients, when creating Teleporter waypoints.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    public sealed class AutoWaypoints : ClientModSystem
    {
        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The client-side API.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            FluentChat.ClientCommand("wpAuto")
                .RegisterWith(capi)
                .HasDescription(LangEx.FeatureString("AutoWaypoints", "SettingsCommandDescription"))
                .HasDefaultHandler((_, _) => ModServices.IOC.Resolve<AutoWaypointsDialogue>().TryOpen());
        }
    }
}