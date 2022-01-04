using System;
using System.Linq;
using System.Text;
using ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Dialogue;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins
{
    /// <summary>
    ///     Client-side entry point for the PlayerPins feature.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    public class PlayerPins : ClientModSystem
    {
        private PlayerPinsSettings _settings;

        public override void StartClientSide(ICoreClientAPI capi)
        {
            FluentChat.ClientCommand("playerpins")
                .RegisterWith(capi)
                .HasDescription(LangEx.FeatureString("PlayerPins", "Description"))
                .HasDefaultHandler((_, _) => ModServices.IOC.Resolve<PlayerPinsDialogue>().TryOpen());

            FluentChat.ClientCommand("friend")
                .HasDescription(LangEx.FeatureString("PlayerPins", "Friends.Description"))
                .RegisterWith(capi)
                .HasDefaultHandler(DefaultHandler)
                .HasSubCommand("add").WithHandler(OnAdd)
                .HasSubCommand("remove").WithHandler(OnRemove)
                .HasSubCommand("list").WithHandler((_, id, args) => DefaultHandler(id, args));

            _settings = ModServices.IOC.Resolve<PlayerPinsSettings>();
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
            ModSettings.World.Save("PlayerPins", _settings);
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
            ModSettings.World.Save("PlayerPins", _settings);
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

            Capi.ShowChatMessage(sb.ToString());
        }

        private void UserFeedback(string action, params object[] args)
        {
            Capi.ShowChatMessage(LangEx.FeatureString("PlayerPins", $"Friends.{action}", args));
        }
    }
}