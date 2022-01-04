using System.Collections.Generic;
using System.Linq;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using Vintagestory.GameContent;

// ReSharper disable MemberCanBePrivate.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model
{
    public sealed class TraderModel
    {
        public string Code { get; }
        public string Colour { get; }
        
        private TraderModel(string code, string colour)
        {
            Code = code;
            Colour = colour;
        }
        
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