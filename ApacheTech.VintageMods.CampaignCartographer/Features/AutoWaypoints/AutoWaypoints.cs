using System;
using ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Services.GUI;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Automatic Waypoint Addition (.wpAuto)
    ///      - Contains a GUI that can be used to control the settings for the feature (Shift + F7).
    ///      - Enable / Disable all automatic waypoint placements.
    ///      - Automatically add waypoints for Translocators, as the player travels between them.
    ///      - Automatically add waypoints for Teleporters, as the player travels between them.
    ///      - Automatically add waypoints for Traders, as the player interacts with them.
    ///      - Automatically add waypoints for Meteors, when the player punches a Meteoric Iron Block.
    ///      - Server: Send Teleporter information to clients, when creating Teleporter waypoints.
    /// </summary>
    /// <seealso cref="Features.AutoWaypoints" />
    /// <seealso cref="UniversalModSystem" />
    public sealed class AutoWaypoints : UniversalModSystem
    {
        public event EventHandler<TeleporterLocation> TeleporterLocationReceived;

        public override void StartServerSide(ICoreServerAPI api)
        {
            ModServices.Network.DefaultServerChannel
                .RegisterMessageType<TeleporterLocation>();
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            Capi.Input.RegisterGuiDialogueHotKey<AutoWaypointsDialogue>(
                LangEx.FeatureString("AutoWaypoints", "Title"),
                GlKeys.F7, HotkeyType.GUIOrOtherControls, shiftPressed: true);

            ModServices.Network.DefaultClientChannel
                .RegisterMessageType<TeleporterLocation>()
                .SetMessageHandler<TeleporterLocation>(OnReceiveTeleporterLocation);
        }

        private void OnReceiveTeleporterLocation(TeleporterLocation location)
        {
            TeleporterLocationReceived?.Invoke(this, location);
        }
    }
}