using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class WorldNamePacket
    {
        public string Name { get; set; }
    }
}