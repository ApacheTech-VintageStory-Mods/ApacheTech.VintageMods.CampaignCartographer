using JetBrains.Annotations;
using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Packets
{
    /// <summary>
    ///     A packet sent between the client and server, detailing information about a whisper sent between two online players.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WhisperPacket
    {
        /// <summary>
        ///     Gets or sets the name of the recipient.
        /// </summary>
        /// <value>The name of the recipient.</value>
        public string RecipientName { get; set; }

        /// <summary>
        ///     Gets or sets the chat group to send the message in.
        /// </summary>
        /// <value>The identifier of the chat channel.</value>
        public int GroupId { get; set; }

        /// <summary>
        ///     Gets or sets the message to send.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }
    }
}