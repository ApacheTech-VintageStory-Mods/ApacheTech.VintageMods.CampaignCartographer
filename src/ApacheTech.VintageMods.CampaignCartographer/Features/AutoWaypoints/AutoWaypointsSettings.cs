using Gantry.Services.FileSystem.Configuration.Abstractions;
using Newtonsoft.Json;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Contains client managed, per world settings for the Auto Waypoints feature. 
    /// </summary>
    [JsonObject]
    public class AutoWaypointsSettings : FeatureSettings
    {
        /// <summary>
        ///     Determines whether translocators should be automatically waypointed.
        /// </summary>
        public bool Translocators { get; set; }

        /// <summary>
        ///     Determines whether teleporters should be automatically waypointed.
        /// </summary>
        public bool Teleporters { get; set; }

        /// <summary>
        ///     Determines whether traders should be automatically waypointed.
        /// </summary>
        public bool Traders { get; set; }

        /// <summary>
        ///     Determines whether mine-able ore deposits on the surface should be automatically waypointed.
        /// </summary>
        public bool SurfaceDeposits { get; set; }

        /// <summary>
        ///     Determines whether loose stones, of different rock types should be automatically waypointed.
        /// </summary>
        public bool LooseStones { get; set; }

        /// <summary>
        ///     Determines whether Mushrooms should be automatically waypointed.
        /// </summary>
        public bool Mushrooms { get; set; }

        /// <summary>
        ///     Determines whether Resin should be automatically waypointed.
        /// </summary>
        public bool Resin { get; set; }

        /// <summary>
        ///     Determines whether Berry Bushes should be automatically waypointed.
        /// </summary>
        public bool BerryBushes { get; set; }
    }
}