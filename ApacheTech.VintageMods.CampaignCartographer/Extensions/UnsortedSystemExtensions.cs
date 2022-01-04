using System.Collections.Generic;
using ApacheTech.Common.Extensions.Harmony;

namespace ApacheTech.VintageMods.CampaignCartographer.Extensions
{
    public static class UnsortedSystemExtensions
    {
        public static void Empty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            foreach (var entry in dictionary)
            {
                entry.GetMethod("Dispose")?.Invoke(entry, null);
            }
            dictionary.Clear();
        }
    }
}