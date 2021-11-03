using JetBrains.Annotations;
using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WhisperPacket
    {
        public string RecipientName { get; set; }
        public int GroupId { get; set; }
        public string Message { get; set; }
    }
}