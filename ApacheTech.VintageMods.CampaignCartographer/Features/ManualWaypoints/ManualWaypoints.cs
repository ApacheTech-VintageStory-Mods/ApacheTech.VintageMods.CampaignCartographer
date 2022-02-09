using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Commands;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

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
            capi.RegisterCommand(ModServices.IOC.Resolve<ManualWaypointsChatCommand>());

            capi.Input.RegisterTransientGuiDialogueHotKey<ManualWaypointsMenuScreen>(LangEx.ModTitle(), GlKeys.F7);

            FluentChat.ClientCommand("wpsettings")
                .RegisterWith(capi)
                .HasDescription(LangEx.FeatureString("ManualWaypoints", "SettingsCommandDescription"))
                .HasDefaultHandler(OnClientDefaultHandler);
        }

        private void OnClientDefaultHandler(int groupId, CmdArgs args)
        {
            var dialogue = ModServices.IOC.Resolve<ManualWaypointsMenuScreen>();
            while (dialogue.IsOpened(dialogue.ToggleKeyCombinationCode))
                dialogue.TryClose();
            dialogue.TryOpen();
        }
    }
}
