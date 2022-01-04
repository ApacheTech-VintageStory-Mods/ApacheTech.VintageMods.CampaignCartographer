using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins
{
    /// <summary>
    ///     A strongly-typed representation of the settings in the JSON file for the PlayerPins feature. This class cannot be inherited.
    /// </summary>
    [JsonObject]
    public sealed class PlayerPinsSettings
    {
        public Color SelfColour { get; set; } = Color.White;
        public int SelfScale { get; set; }

        public Color FriendColour { get; set; } = Color.Cyan;
        public int FriendScale { get; set; }

        public Color OthersColour { get; set; } = Color.FromArgb(76, 76, 76);
        public int OthersScale { get; set; }
        public Dictionary<string, string> Friends { get; set; } = new();
    }
}