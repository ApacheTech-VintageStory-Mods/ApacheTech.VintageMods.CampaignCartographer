using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.Extensions;
using Newtonsoft.Json;
using ProtoBuf;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures
{
    /// <summary>
    ///     Represents a Waypoint, with a set position, that can be added the the map within the game.
    /// </summary>
    [JsonObject]
    [ProtoContract]
    public class SelectableWaypointTemplate : PositionedWaypointTemplate
    {
        [JsonRequired]
        [ProtoMember(7)]
        public bool Selected { get; set; } = true;

        public static SelectableWaypointTemplate FromWaypoint(Waypoint waypoint)
        {
            return new SelectableWaypointTemplate
            {
                Colour = waypoint.Color.ToHexString(),
                DisplayedIcon = waypoint.Icon,
                ServerIcon = waypoint.Icon,
                Title = waypoint.Title,
                Pinned = waypoint.Pinned,
                Position = waypoint.Position,
                Selected = true
            };
        }
    }
}
