using ApacheTech.VintageMods.Core.Abstractions.ModSystems;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints
{
    public sealed class SurfaceDeposits : ClientModSystem
    {
        // Setup:
        //      Collate a list of all loose surface deposits.
        //      Map each deposit type to a WaypointInfoModel.

        // Usage:
        //      Patch when player "picks up" the deposit.
        //      Patch when player "breaks" the deposit.
        //      If no waypoint in range, make waypoint.


    }
}
