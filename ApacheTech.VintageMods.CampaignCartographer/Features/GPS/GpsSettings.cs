using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public sealed class GpsSettings
    {
        public bool WhispersAllowed { get; set; }
    }
}
