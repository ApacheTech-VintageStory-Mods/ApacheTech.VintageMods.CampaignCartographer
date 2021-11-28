using ApacheTech.VintageMods.CampaignCartographer.Services.GUI;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Extensions;
using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Automatic Waypoint Addition(.wpAuto)
    ///      - Contains a GUI that can be used to control the settings for the feature.
    ///      - Enable / Disable all automatic waypoint placements.
    ///      - Automatically add waypoints for Translocators, as the player travels between them.
    ///      - Automatically add waypoints for Teleporters, as the player travels between them.
    ///      - Automatically add waypoints for Traders, as the player interacts with them.
    ///      - Automatically add waypoints for Meteors, when the player shift-right-clicks on Meteoric Iron Blocks.
    ///      - Server: Send Teleporter information to clients, when creating Teleporter waypoints.
    /// </summary>
    /// <seealso cref="AutoWaypoints" />
    /// <seealso cref="UniversalModSystem" />
    internal class AutoWaypointsModSystem : UniversalModSystem
    {
        public override void StartClientSide(ICoreClientAPI capi)
        {
            capi.Input.RegisterGuiDialogueHotKey<TestWindow>(
                Lang.Get("wpex:controls.test-window-toggle-description"),
                GlKeys.U,
                HotkeyType.GUIOrOtherControls);

            ModServices.IOC
                .CreateSidedInstance<AutoWaypoints>()
                .StartClientSide(capi);
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            ModServices.IOC
                .CreateSidedInstance<AutoWaypoints>()
                .StartServerSide(sapi);
        }
    }
}
