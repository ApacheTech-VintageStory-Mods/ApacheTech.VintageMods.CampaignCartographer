using System;
using Newtonsoft.Json;
using ProtoBuf;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures
{
    /// <summary>
    ///     Represents a Waypoint that has a connection to another waypoint on the map.
    /// </summary>
    [JsonObject]
    [ProtoContract]
    public class ConnectedWaypointTemplate : WaypointTemplate
    {
        /// <summary>
        ///     The ID of the Waypoint that this Waypoint is connected to.
        /// </summary>
        /// <value>A <see cref="Guid"/> representing the Waypoint that is connected to this one.</value>
        [ProtoMember(6)]
        public Guid EndPoint { get; set; } = Guid.Empty;
    }
}