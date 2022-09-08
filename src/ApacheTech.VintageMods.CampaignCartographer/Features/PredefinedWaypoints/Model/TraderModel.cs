using System.Collections.Generic;
using System.Linq;
using Gantry.Core.GameContent.AssetEnum;
using JetBrains.Annotations;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Model
{
    /// <summary>
    ///     Represents meta date information for trader waypoints.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class TraderModel
    {
        /// <summary>
        ///     Gets the internal code that identifies this trader type.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; }

        /// <summary>
        ///     Gets the colour to use for waypoints for this trader type.
        /// </summary>
        /// <value>The colour.</value>
        public string Colour { get; }

        /// <summary>
        /// 	Initialises a new instance of the <see cref="TraderModel"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="colour">The colour.</param>
        private TraderModel(string code, string colour)
        {
            Code = code;
            Colour = colour;
        }

        /// <summary>
        ///     Gets a list of all traders, and their associated waypoint colours.
        /// </summary>
        public static readonly List<TraderModel> Traders = new()
        {
            new TraderModel(TraderType.Artisan, NamedColour.Aqua),
            new TraderModel(TraderType.BuildingSupplies, NamedColour.Red),
            new TraderModel(TraderType.Clothing, NamedColour.Green),
            new TraderModel(TraderType.Commodities, NamedColour.Grey),
            new TraderModel(TraderType.Foods, "#C8C080"),
            new TraderModel(TraderType.Furniture, NamedColour.Orange),
            new TraderModel(TraderType.Luxuries, NamedColour.Blue),
            new TraderModel(TraderType.SurvivalGoods, NamedColour.Yellow),
            new TraderModel(TraderType.TreasureHunter, NamedColour.Purple),
        };

        /// <summary>
        ///     Gets the colour associated with the specified trader.
        /// </summary>
        /// <param name="trader">The trader to find the colour of.</param>
        /// <returns>A <see cref="string"/> representation of the colour to use for the waypoint of the trader.</returns>
        public static string GetColourFor(EntityTrader trader)
        {
            return Traders.SingleOrDefault(p =>
                    trader.Code.Path
                        .ToLowerInvariant()
                        .EndsWith(p.Code))
                ?.Colour ?? "White";
        }
    }
}