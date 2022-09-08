using System;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class GpsHyperLink
    {
        public BlockPos Position { get; set; }

        public GpsHyperLink(LinkTextComponent link)
        {
            if (!link.Href.StartsWith("gps://"))
            {
                throw new ArgumentException("GPS Link protocol not recognised.");
            }
            var data = link.Href.Replace("gps://", "");
            var array = data.Split('=');
            Position = new BlockPos(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
        }

        public GpsHyperLink(BlockPos position)
        {
            Position = position;
        }

        public string ToHyperlink()
        {
            var path = $"{Position.X}={Position.Y}={Position.Z}";
            return $"<a href=\"gps://{path}\">{LangEx.FeatureString("GPS", "ShowOnMap")}</a>";
        }

        public static void Execute(LinkTextComponent link)
        {
            new GpsHyperLink(link).Execute();
        }

        public void Execute()
        {
            var mapManager = IOC.Services.Resolve<WorldMapManager>();

            if (mapManager.worldMapDlg is null ||
                !mapManager.worldMapDlg.IsOpened() ||
                mapManager.worldMapDlg.IsOpened() && mapManager.worldMapDlg.DialogType == EnumDialogType.HUD)
            {
                mapManager.ToggleMap(EnumDialogType.Dialog);
            }

            var map = mapManager.worldMapDlg?.SingleComposer.GetElement("mapElem") as GuiElementMap;
            map?.CenterMapTo(Position);
        }
    }
}