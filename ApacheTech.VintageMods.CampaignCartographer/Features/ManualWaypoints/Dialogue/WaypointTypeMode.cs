namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue
{
    /// <summary>
    ///     Determines the mod to open the <see cref="AddEditWaypointTypeDialogue"/> form in.
    /// </summary>
    public enum WaypointTypeMode
    {
        /// <summary>
        ///     Displays the form in Add mode, allowing the user to add a new waypoint type.
        /// </summary>
        Add,

        /// <summary>
        ///     Displays the form in Edit mode, allowing the user to edit or delete an existing waypoint type.
        /// </summary>
        Edit
    }
}