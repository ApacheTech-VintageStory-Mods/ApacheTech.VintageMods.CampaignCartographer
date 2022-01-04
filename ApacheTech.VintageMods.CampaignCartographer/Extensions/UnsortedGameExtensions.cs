using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Extensions
{
    public static class UnsortedGameExtensions
    {
        public static void Delete(this LoadedTexture texture)
        {
            ApiEx.Client.Gui.DeleteTexture(texture.TextureId);
        }
    }
}