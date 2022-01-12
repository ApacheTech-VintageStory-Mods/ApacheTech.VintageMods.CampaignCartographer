using System.Threading.Tasks;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.Core.Services.Network.Extensions;
using ApacheTech.VintageMods.Core.Services.Network.Packets;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Services
{
    /// <summary>
    ///     Allows the client to request the name of the game world from the server. Stupidly, this is only necessary within SinglePlayer.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    public class WorldNameRelay : ClientModSystem
    {
        private IClientNetworkChannel _clientChannel;
        private string _worldName;

        /// <summary>
        /// Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="api">The API.</param>
        public override void StartClientSide(ICoreClientAPI api)
        {
            _clientChannel = ModServices.Network.DefaultClientChannel?
                .RegisterMessageType<WorldNamePacket>()
                .SetMessageHandler<WorldNamePacket>(OnWorldNamePacketReceived);
        }

        /// <summary>
        ///     Asynchronously get the world name.
        ///     If on a multi-player server, it will get the server name.
        ///     If on single-player, it will get the world name, as shown on the log in screen.
        ///     If the network is not available on single-player, it will get a default fallback name.
        /// </summary>
        /// <returns>System.Threading.Tasks.Task&lt;System.String&gt;.</returns>
        public Task<string> GetWorldNameAsync()
        {
            return Task<string>.Factory.StartNew(() =>
            {
                if (_worldName is not null) return _worldName;
                if (!(ApiEx.Client.IsSinglePlayer && _clientChannel.Connected))
                {
                    var serverInfo = ApiEx.ClientMain.GetField<ServerInformation>("ServerInfo");
                    return serverInfo.GetField<string>("ServerName");
                }
                _clientChannel.SendPacket<WorldNamePacket>();
                while (_worldName is null)
                {
                    Task.Delay(20);
                }
                return _worldName;
            });
        }

        /// <summary>
        ///     Sets the name of the single-player world, when it's received from the server.
        /// </summary>
        /// <param name="packet">The packet.</param>
        private void OnWorldNamePacketReceived(WorldNamePacket packet)
        {
            _worldName = 
                packet.Name ?? 
                LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "DefaultWorldName");
        }
    }
}
