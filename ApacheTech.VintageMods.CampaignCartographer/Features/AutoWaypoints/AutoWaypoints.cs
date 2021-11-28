using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Annotation;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
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
    /// <seealso cref="AutoWaypointsModSystem" />
    /// <seealso cref="IUniversalFeatureHandler" />
    public class AutoWaypoints : IUniversalFeatureHandler
    {
        private readonly AutoWaypointsSettings _settings;

        [SidedServiceProviderConstructor(EnumAppSide.Server)]
        public AutoWaypoints() { }


        [SidedServiceProviderConstructor(EnumAppSide.Client)]
        public AutoWaypoints(AutoWaypointsSettings settings)
        {
            _settings = settings;
        }

        public void StartClientSide(ICoreClientAPI capi)
        {
            
        }

        public void StartServerSide(ICoreServerAPI sapi)
        {

        }
    }
}