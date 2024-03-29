﻿using HarmonyLib;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Patches
{
    public partial class WaypointUtilPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClientEventManager), "TriggerNewServerChatLine")]
        [HarmonyPriority(Priority.First)]
        public static bool Patch_ClientEventManger_TriggerNewServerChatLine_Prefix(string message)
        {
            if (!Settings.SilenceFeedback) return true;
            if (string.IsNullOrWhiteSpace(message)) return true;

            var waypointAddedText = Lang.Get("Ok, waypoint nr. {0} added", 0);
            var isWaypointAddedMessage = message.StartsWith(waypointAddedText.Substring(0, 11));

            var waypointDeletedText = Lang.Get("Ok, deleted waypoint.");
            var isWaypointDeletedMessage = message.StartsWith(waypointDeletedText.Substring(0, 11));
            
            return !(isWaypointAddedMessage || isWaypointDeletedMessage);
        }
    }
}
