namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints
{
    /// <summary>
    ///     General Global Settings for the Client.
    /// </summary>
    public class GeneralClientGlobalSettings
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to always load default waypoints.
        /// </summary>
        /// <value><c>true</c> if default waypoints should always be loaded; otherwise, <c>false</c>.</value>
        public bool AlwaysLoadDefaultWaypoints { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to never load default waypoints.
        /// </summary>
        /// <value><c>true</c> if default waypoints should never be loaded; otherwise, <c>false</c>.</value>
        public bool NeverLoadDefaultWaypoints { get; set; }
    }
}