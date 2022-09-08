using ApacheTech.VintageMods.CampaignCartographer.Features.CentreMap.Packets;
using Gantry.Core;
using Gantry.Core.ModSystems;
using JetBrains.Annotations;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.CentreMap
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class CentreMapServer : ServerModSystem
    {
        private IServerNetworkChannel _serverChannel;

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreServerAPI in Start()
        /// </summary>
        /// <param name="sapi">The core API implemented by the server. The main interface for accessing the server. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            _serverChannel = sapi.Network.RegisterChannel("centreMap")
                .RegisterMessageType<PlayerSpawnPositionDto>()
                .SetMessageHandler<PlayerSpawnPositionDto>(OnServerSpawnPointRequestReceived);
        }

        /// <summary>
        ///     Called when on the server, when the client sends a <see cref="PlayerSpawnPositionDto"/> packet.
        /// </summary>
        /// <param name="fromPlayer">The player that sent the packet.</param>
        /// <param name="packet">The packet that was sent.</param>
        private void OnServerSpawnPointRequestReceived(IServerPlayer fromPlayer, PlayerSpawnPositionDto packet)
        {
            var spawnPosition = ApiEx.ServerMain.GetSpawnPosition(fromPlayer.PlayerUID).AsBlockPos;
            _serverChannel.SendPacket(new PlayerSpawnPositionDto(spawnPosition), fromPlayer);
        }
    }
}