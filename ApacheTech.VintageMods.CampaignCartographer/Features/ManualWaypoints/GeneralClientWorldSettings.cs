namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints
{
    /// <summary>
    ///     General Per-World Settings for the Client.
    /// </summary>
    public class GeneralClientWorldSettings
    {
        /// <summary>
        ///     Gets or sets a value indicating whether or not this is the first time the user has run this mod on this server.
        /// </summary>
        /// <value><c>true</c> if it's the first time the mod has been run; otherwise, <c>false</c>.</value>
        public bool FirstRun { get; set; }
    }
}