using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public sealed class GpsSettings
    {
        public bool WhispersAllowed { get; set; }
    }
}
