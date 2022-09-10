using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Domain
{
    /// <summary>
    ///     Defines the information stored within a single GUI Cell Element.
    /// </summary>
    /// <seealso cref="SavegameCellEntry" />
    public class CellEntry<T> : SavegameCellEntry
    {
        /// <summary>
        ///     Gets the DTO model that defines the structure of the JSON import file.
        /// </summary>
        public T Model { get; init; }
    }
}