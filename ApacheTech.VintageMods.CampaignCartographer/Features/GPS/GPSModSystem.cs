using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Extensions;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS
{
    /// <summary>
    ///     Feature: Global Positioning System.
    /// 
    ///      - Display your current XYZ coordinates.
    ///      - Copy your current XYZ coordinates to clipboard.
    ///      - Send your current XYZ coordinates as a chat message to the current chat group.
    ///      - Whisper your current XYZ coordinates to a single player (server settings permitting).
    ///      - Server: Enable/Disable permissions to whisper to other members of the server.
    /// </summary>
    /// <seealso cref="UniversalModSystem" />
    public sealed class GPSModSystem : UniversalModSystem
    {
        public override void StartClientSide(ICoreClientAPI capi)
        {
            ModServices.IOC
                .CreateSidedInstance<GPS>()
                .StartClientSide(capi);
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            ModServices.IOC
                .CreateSidedInstance<GPS>()
                .StartServerSide(sapi);
        }
    }
}
