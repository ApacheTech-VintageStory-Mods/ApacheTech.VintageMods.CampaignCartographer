using System.Linq;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Extensions
{
    public static class WorldMapManagerExtensions
    {
        /// <summary>
        ///     Returns the map layer used for rendering player pins.
        /// </summary>
        /// <param name="mapManager">The <see cref="WorldMapManager" /> instance that this method was called from.</param>
        public static PlayerPinsMapLayer PlayerMapLayer(this WorldMapManager mapManager)
        {
            return mapManager.MapLayers.OfType<PlayerPinsMapLayer>().First();
        }
    }
}