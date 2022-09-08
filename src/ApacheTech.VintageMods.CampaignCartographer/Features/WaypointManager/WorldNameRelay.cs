using System.Threading.Tasks;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Packets;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.ModSystems;
using Gantry.Services.Network;
using Gantry.Services.Network.Extensions;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager
{
    /// <summary>
    ///     Allows the client to request the name of the game world from the server. Stupidly, this is only necessary within SinglePlayer.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WorldNameRelay : UniversalModSystem
    {
        private IClientNetworkChannel _clientChannel;
        private IServerNetworkChannel _serverChannel;
        private string _worldName;

        /// <summary>
        /// Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="api">The API.</param>
        public override void StartClientSide(ICoreClientAPI api)
        {
            _clientChannel = IOC.Services
                .Resolve<IClientNetworkService>()
                .DefaultClientChannel?
                .RegisterMessageType<WorldNamePacket>()
                .SetMessageHandler<WorldNamePacket>(OnWorldNamePacketReceived);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            _serverChannel = IOC.Services
                .Resolve<IServerNetworkService>()
                .DefaultServerChannel?
                .RegisterMessageType<WorldNamePacket>()
                .SetMessageHandler<WorldNamePacket>(OnWorldNamePacketReceivedOnServer);
        }

        private void OnWorldNamePacketReceivedOnServer(IServerPlayer fromPlayer, WorldNamePacket packet)
        {
            _serverChannel.SendPacket(packet.With(p => p.Name = Sapi.WorldManager.SaveGame.WorldName), fromPlayer);
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
                    // TODO: Now that the API has been de-obfuscated, this needs to be re-addressed.
                    var serverInfo = ApiEx.ClientMain.GetField<ServerInformation>("ServerInfo");
                    return serverInfo.GetField<string>("ServerName");
                }

                _clientChannel.SendPacket<WorldNamePacket>();
                while (_worldName is null)
                {
                    // TODO: Now that the API has been de-obfuscated, this needs to be re-addressed.
                    // TODO: Potential infinite loop, if server doesn't pass back a value.
                    // However, this would only lock a ThreadPool thread, rather than the UI thread.
                    _ = Task.Delay(20);
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