using System;
using Newtonsoft.Json;
using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model
{
    /// <summary>
    ///     Represents meta data pertaining to a waypoint that is manually added to the map. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="WaypointTemplate" />
    /// <seealso cref="ICloneable" />
    [JsonObject]
    [ProtoContract]
    public sealed class ManualWaypointTemplateModel : WaypointTemplate, ICloneable
    {
        /// <summary>
        ///     Gets or sets the syntax the user must type to add this type of waypoint.
        /// </summary>
        /// <value>The syntax value of the waypoint.</value>
        [JsonRequired]
        [ProtoMember(9)]
        public string Syntax { get; set; }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}