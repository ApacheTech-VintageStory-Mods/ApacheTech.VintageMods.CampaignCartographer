using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public class ClientEventManagerPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClientEventManager), "TriggerNewServerChatLine")]
        [HarmonyPriority(Priority.First)]
        public static bool Patch_ClientEventManger_TriggerNewServerChatLine_Prefix(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return true;

            var waypointAddedText = Lang.Get("Ok, waypoint nr. {0} added", 0);
            var isWaypointAddedMessage = message.StartsWith(waypointAddedText.Substring(0, 11));

            var waypointDeletedText = Lang.Get("Ok, deleted waypoint.");
            var isWaypointDeletedMessage = message.StartsWith(waypointDeletedText.Substring(0, 11));
            
            return !(isWaypointAddedMessage || isWaypointDeletedMessage);
        }
    }
}
