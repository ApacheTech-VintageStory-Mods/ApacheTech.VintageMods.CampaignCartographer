using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Commands;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;

// ReSharper disable All

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Contains a GUI that can be used to control the settings for the feature.
    ///      • Add a waypoint at the player's current location, via a chat command.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    public sealed class ManualWaypoints : ClientModSystem
    {
        public override void StartClientSide(ICoreClientAPI capi)
        {
            var command = ModServices.IOC.Resolve<ManualWaypointsChatCommand>();
            capi.RegisterCommand(command);
        }
    }
}
