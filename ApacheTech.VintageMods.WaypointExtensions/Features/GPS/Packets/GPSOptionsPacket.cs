using ProtoBuf;

namespace ApacheTech.VintageMods.WaypointExtensions.Features.GPS.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public sealed class GPSOptionsPacket
    {
        public bool WhispersAllowed { get; set; }
    }
}
