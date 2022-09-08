using System.Collections.Generic;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointManager.Dialogue.Exports
{
    /// <summary>
    ///     Proxy method delegate, to allow unique parameters to be passed through the IOC container.
    ///     Creates an instance of the <see cref="WaypointExportConfirmationDialogue"/> class, and displays it.
    /// </summary>
    /// <param name="waypoints">The waypoints to pass to the form.</param>
    /// <returns>An instance of <see cref="WaypointExportConfirmationDialogue"/>, created via the IOC container.</returns>
    public delegate WaypointExportConfirmationDialogue ShowConfirmExportDialogue(List<PositionedWaypointTemplate> waypoints);
}