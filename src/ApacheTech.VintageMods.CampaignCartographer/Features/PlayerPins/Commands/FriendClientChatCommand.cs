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
            FluentChat.RegisterCommand("friend", _capi)!
                .WithDescription(LangEx.FeatureString("PlayerPins", "Friends.Description"))
                .WithHandler(DefaultHandler)
                .HasSubCommand("add", s => s.WithHandler(OnAdd).Build())
                .HasSubCommand("remove", s => s.WithHandler(OnRemove).Build())
                .HasSubCommand("list", s => s.WithHandler(DefaultHandler).Build());
        }

        private void OnAdd(IPlayer player, int groupId, CmdArgs args)
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

            var friend = players.First();
            if (_settings.Friends.Values.Contains(friend.PlayerUID))
            {
                UserFeedback("PlayerAlreadyAdded", friend.PlayerName);
                return;
            }

            _settings.Friends.Add(friend.PlayerName, friend.PlayerUID);
            ModSettings.World.Save(_settings);
            UserFeedback("PlayerAdded", friend.PlayerName);
        }

        private void OnRemove(IPlayer player, int groupId, CmdArgs args)
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

            var friend = players.First();
            _settings.Friends.Remove(friend);
            ModSettings.World.Save(_settings);
            UserFeedback("PlayerRemoved", friend);
        }

        private void DefaultHandler(IPlayer player, int groupId, CmdArgs args)
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