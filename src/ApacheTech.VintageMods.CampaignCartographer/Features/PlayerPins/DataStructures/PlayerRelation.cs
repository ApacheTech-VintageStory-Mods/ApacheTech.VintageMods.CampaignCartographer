namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.DataStructures
{
    /// <summary>
    ///     Determines the relationship between a given player entity, and the current client entity.
    /// </summary>
    public enum PlayerRelation
    {
        /// <summary>
        ///     Specifies that the given player is the client player.
        /// </summary>
        Self,

        /// <summary>
        ///     Specifies that the given player has been highlighted on the map.
        /// </summary>
        Friend,

        /// <summary>
        ///     Specifies that the given player is another player on the server.
        /// </summary>
        Others
    }
}