using System.Collections.Generic;
using System.IO;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Commands;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.DotNet;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.Core.Services.FileSystem.Extensions;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;

// ReSharper disable All

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Contains a GUI that can be used to control the settings for the feature.
    ///      • Add a waypoint at the player's current location, via a chat command.
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    public sealed class ManualWaypoints : ClientModSystem
    {
        public override void StartClientSide(ICoreClientAPI capi)
        {
            capi.RegisterCommand(ModServices.IOC.Resolve<ManualWaypointsChatCommand>());

            capi.Input.RegisterTransientGuiDialogueHotKey<ManualWaypointsMenuScreen>(LangEx.ModTitle(), GlKeys.F7);

            FluentChat.ClientCommand("wpsettings")
                .RegisterWith(capi)
                .HasDescription(LangEx.FeatureString("ManualWaypoints", "SettingsCommandDescription"))
                .HasDefaultHandler((_, _) => ModServices.IOC.Resolve<ManualWaypointsMenuScreen>().ToggleGui());

            UpdateWaypointTypesFromWorldFile();
        }

        private void UpdateWaypointTypesFromWorldFile()
        {
            var oldFile = new FileInfo(Path.Combine(ModPaths.ModDataWorldPath, "waypoint-types.json"));
            if (!oldFile.Exists) return;
            
            var newFile = ModServices.FileSystem.GetJsonFile("waypoint-types.json");
            var waypointTypes = new SortedDictionary<string, ManualWaypointTemplateModel>();
            
            waypointTypes.AddOrUpdateRange(newFile.ParseAsMany<ManualWaypointTemplateModel>(), w => w.Syntax);
            waypointTypes.AddOrUpdateRange(oldFile.ParseAsMany<ManualWaypointTemplateModel>(), w => w.Syntax);

            newFile.SaveFrom(waypointTypes.Values);
            oldFile.Delete();
        }
    }
}
