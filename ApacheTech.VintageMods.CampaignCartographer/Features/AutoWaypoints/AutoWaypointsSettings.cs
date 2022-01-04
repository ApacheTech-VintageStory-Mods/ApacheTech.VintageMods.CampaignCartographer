using ProtoBuf;

// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class AutoWaypointsSettings
    {
        public bool Translocators { get; set; }

        public bool Teleporters { get; set; }

        public bool Traders { get; set; }

        public bool SurfaceDeposits { get; set; }

        public bool LooseStones { get; set; }

        public bool Organics { get; set; }
    }
}