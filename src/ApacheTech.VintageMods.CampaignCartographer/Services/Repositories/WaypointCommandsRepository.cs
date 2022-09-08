using System.Collections.Generic;
using System.Threading.Tasks;
using ApacheTech.VintageMods.CampaignCartographer.Services.Repositories.Commands;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.Extensions;
using Gantry.Core;
using Gantry.Core.Contracts;
using Gantry.Core.Extensions.GameContent;
using Gantry.Services.HarmonyPatches.Annotations;
using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.Repositories
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class WaypointCommandsRepository
    {
        private readonly WorldMapManager _mapManager;
        
        private static bool _processing;
        private List<Waypoint> SavedWaypoints => _mapManager.WaypointMapLayer().ownWaypoints;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WaypointMapLayer), nameof(WaypointMapLayer.OnDataFromServer))]
        private static void Patch_WaypointMapLayer_OnDataFromServer_Postfix()
        {
            _processing = false;
        }

        public WaypointCommandsRepository(WorldMapManager mapManager)
        {
            _mapManager = mapManager;
        }

        private IEnumerable<int> Filter(Func<Waypoint, bool> filter)
        {
            var list = new List<int>();
            for (var i = 0; i < SavedWaypoints.Count; i++)
            {
                if (!filter(SavedWaypoints[i])) continue;
                list.Add(i);
            }
            list.Reverse();
            return list;
        }

        private void ProcessMassRemoval(Func<Waypoint, bool> filter)
        {
            var list = Filter(filter);
            var commands = new Queue<ICommand>();
            foreach (var waypoint in list) commands.Enqueue(new RemoveWaypointCommand(waypoint));
            ProcessQueue(commands);
        }

        private void ProcessMassAddition(IEnumerable<PositionedWaypointTemplate> waypoints)
        {
            var commands = new Queue<ICommand>();
            foreach (var waypoint in waypoints) 
                commands.Enqueue(new AddWaypointCommand(
                    waypoint.Position.AsBlockPos, 
                    waypoint.ServerIcon, 
                    waypoint.Pinned, 
                    waypoint.Colour, 
                    waypoint.Title));
            ProcessQueue(commands);
        }

        private async void ProcessQueue(Queue<ICommand> commands)
        {
            while (commands.Count > 0)
            {
                while (_processing) await Task.Delay(20);
                _processing = true;
                commands
                    .Dequeue()
                    .Execute();
            }
        }

        public void RemoveAll()
        {
            ProcessMassRemoval(_ => true);
        }

        public void RemoveAllWaypointsAtPosition(BlockPos pos)
        {
            ProcessMassRemoval(w => w.IsInHorizontalRangeOf(pos, 0));
        }

        public void RemoveNearPlayer(int radius)
        {
            ProcessMassRemoval(w => w.IsInHorizontalRangeOf(
                ApiEx.Client.World.Player.Entity.Pos.AsBlockPos, radius));
        }

        public void RemoveByIcon(string icon)
        {
            ProcessMassRemoval(w => w.Icon == icon);
        }

        public void RemoveByColour(string colour)
        {
            ProcessMassRemoval(w => w.Color == colour.ToInt());
        }

        public void RemoveByTitle(string partialTitle)
        {
            ProcessMassRemoval(w => w.Title.StartsWith(partialTitle));
        }

        public void AddWaypoints<T>(IEnumerable<T> waypoints) where T: PositionedWaypointTemplate
        {
            ProcessMassAddition(waypoints);
        }
    }
}
