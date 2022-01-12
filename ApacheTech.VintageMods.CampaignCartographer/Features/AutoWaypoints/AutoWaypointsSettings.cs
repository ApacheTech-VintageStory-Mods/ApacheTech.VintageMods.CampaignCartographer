using ProtoBuf;

// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Contains client managed, per world settings for the Auto Waypoints feature. 
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class AutoWaypointsSettings
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
        ///     Determines whether Mushrooms, Resin, and Berry Bushes should be automatically waypointed.
        /// </summary>
        public bool Organics { get; set; }
    }
}