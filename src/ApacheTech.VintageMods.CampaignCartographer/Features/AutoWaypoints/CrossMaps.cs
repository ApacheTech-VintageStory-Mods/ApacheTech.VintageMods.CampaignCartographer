using JetBrains.Annotations;
using System.Collections.Generic;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    /// <summary>
    ///     Dictionaries that map between block codes, and waypoint types.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class CrossMaps
    {
        public Dictionary<string, string> Ores { get; init; }

        public Dictionary<string, string> Stones { get; init; }
        
        public Dictionary<string, string> Organics { get; init; }
    }
}