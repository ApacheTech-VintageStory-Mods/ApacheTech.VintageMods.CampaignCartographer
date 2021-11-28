using System;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Packets;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Handlers
{
    public class WhisperCommandHandler
    {
        private readonly GpsSettings _settings;

        public WhisperCommandHandler(GpsSettings settings)
        {
            _settings = settings;
        }

        public void OnServerWhisperCommandHandler(IServerPlayer player, int groupId, CmdArgs args)
        {
            try
            {
                var option = args.PopWord();
                var state = option switch
                {
                    "enable" => true,
                    "disable" => false,
                    _ => throw new ArgumentOutOfRangeException()
                };
                SetWhispersAllowed(state);
                player.SendMessage(groupId, Lang.Get($"wpex:features.gps.server.gps-whisper-{option}d"), EnumChatType.CommandSuccess);
            }
            catch (ArgumentOutOfRangeException)
            {
                player.SendInvalidSyntaxMessage(groupId);
            }
        }

        private void SetWhispersAllowed(bool value)
        {
            ModServices.Network.DefaultServerChannel
                .BroadcastPacket(_settings
                    .With(p => p.WhispersAllowed = value));
        }
    }
}