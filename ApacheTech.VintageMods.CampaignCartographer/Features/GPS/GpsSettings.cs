using ApacheTech.VintageMods.Core.Abstractions.Features;
using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS
{
    /// <summary>
    ///     Represents a DTO object to pass GPS settings between the client and server.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public sealed class GpsSettings : FeatureSettings
    {
        /// <summary>
        ///     Determines whether or not players can send each other private messages, with GPS information.
        /// </summary>
        /// <value><c>true</c> if whispers are allowed; otherwise, <c>false</c>.</value>
        public bool WhispersAllowed { get; set; }
    }
}
