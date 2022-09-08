using System;
using System.Linq;
using System.Text;
using ApacheTech.VintageMods.FluentChatCommands;
using Gantry.Core;
using Gantry.Services.FileSystem.Configuration;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Commands
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class FriendClientChatCommand
    {
        private readonly PlayerPinsSettings _settings;
        private readonly ICoreClientAPI _capi;

        public FriendClientChatCommand(ICoreClientAPI capi, PlayerPinsSettings settings)
        {
            _settings = settings;
            _capi = capi;
        }

        public void Register()
        {
            FluentChat.ClientCommand("friend")
                .HasDescription(LangEx.FeatureString("PlayerPins", "Friends.Description"))
                .RegisterWith(_capi)
                .HasDefaultHandler(DefaultHandler)
                .HasSubCommand("add").WithHandler(OnAdd)
                .HasSubCommand("remove").WithHandler(OnRemove)
                .HasSubCommand("list").WithHandler((_, id, args) => DefaultHandler(id, args));
        }

        private void OnAdd(string subCommandName, int groupId, CmdArgs args)
        {
            if (args.Length == 0)
            {
                UserFeedback("NoPlayerNameGiven");
                return;
            }
            var partialName = args.PopWord();
            var players = ApiEx.ClientMain.AllOnlinePlayers
                .Where(p => p.PlayerName.StartsWith(partialName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (!players.Any())
            {
                UserFeedback("PlayerNotFound", partialName);
                return;
            }

            if (players.Count > 1)
            {
                UserFeedback("MultiplePlayersFound", partialName);
                return;
            }

            var player = players.First();
            if (_settings.Friends.Values.Contains(player.PlayerUID))
            {
                UserFeedback("PlayerAlreadyAdded", player.PlayerName);
                return;
            }

            _settings.Friends.Add(player.PlayerName, player.PlayerUID);
            ModSettings.World.Save(_settings);
            UserFeedback("PlayerAdded", player.PlayerName);
        }

        private void OnRemove(string subCommandName, int groupId, CmdArgs args)
        {
            if (!_settings.Friends.Any())
            {
                UserFeedback("YouHaveNoFriends");
                return;
            }

            if (args.Length == 0)
            {
                UserFeedback("NoPlayerNameGiven");
                return;
            }

            var partialName = args.PopWord();
            var players = _settings.Friends.Keys
                .Where(p => p.StartsWith(partialName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (!players.Any())
            {
                UserFeedback("PlayerNotFound", partialName);
                return;
            }

            if (players.Count > 1)
            {
                UserFeedback("MultiplePlayersFound", partialName);
                return;
            }

            var player = players.First();
            _settings.Friends.Remove(player);
            ModSettings.World.Save(_settings);
            UserFeedback("PlayerRemoved", player);
        }

        private void DefaultHandler(int groupId, CmdArgs args)
        {
            if (!_settings.Friends.Any())
            {
                UserFeedback("YouHaveNoFriends");
                return;
            }

            var sb = new StringBuilder(LangEx.FeatureString("PlayerPins", "Friends.FriendsList"));
            sb.AppendLine("\n");

            foreach (var friend in _settings.Friends.Keys)
            {
                sb.AppendLine(friend);
            }

            _capi.ShowChatMessage(sb.ToString());
        }

        private void UserFeedback(string action, params object[] args)
        {
            _capi.ShowChatMessage(LangEx.FeatureString("PlayerPins", $"Friends.{action}", args));
        }
    }
}