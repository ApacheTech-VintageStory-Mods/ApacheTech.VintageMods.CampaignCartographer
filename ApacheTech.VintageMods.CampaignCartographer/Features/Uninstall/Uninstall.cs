using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.Uninstall
{
    /// <summary>
    ///     Feature: Uninstall (.wpUninstall | /wpUninstall)
    ///
    ///      - Removes all files and folders related to the mod, from the data directory.
    ///      - Restores all custom waypoint icons to the default `dot` icon.
    ///      - Server: Exports all home location waypoints to file.
    ///      - Server: Exports saved waypoints database to file.
    ///      - Client: Exports waypoints database to file.
    /// </summary>
    /// <seealso cref="UniversalModSystem" />
    internal sealed class Uninstall : UniversalModSystem
    {
        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreServerAPI in Start()
        /// </summary>
        /// <param name="sapi">The server-side API.</param>
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            FluentChat
                .ServerCommand("wpuninstall")
                .RegisterWith(sapi);
        }

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The client-side API.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            FluentChat
                .ClientCommand("wpuninstall")
                .RegisterWith(capi);
        }
    }
}
