using Gantry.Core;
using JetBrains.Annotations;
using Vintagestory.API.MathTools;

namespace ApacheTech.VintageMods.CampaignCartographer.Domain
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class NativeWaypointCommands
    {
        public static void Add(BlockPos position, string icon, bool pinned, string colour, string title) =>
            ApiEx.Client.SendChatMessage($"/waypoint addati {icon} {position.X} {position.Y} {position.Z} {pinned} {colour} {title}");

        public static void Modify(int index, string icon, bool pinned, string colour, string title) =>
            ApiEx.Client.SendChatMessage($"/waypoint modify {index} {colour} {icon} {pinned} {title}");

        public static void Remove(int index) =>
            ApiEx.Client.SendChatMessage($"/waypoint remove {index}");
    }
}