using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class AutoWaypointsSettings
    {
        public bool Translocators { get; set; }

        public bool Traders { get; set; }

        public bool Teleporters { get; set; }

        public bool Meteors { get; set; }
    }
}