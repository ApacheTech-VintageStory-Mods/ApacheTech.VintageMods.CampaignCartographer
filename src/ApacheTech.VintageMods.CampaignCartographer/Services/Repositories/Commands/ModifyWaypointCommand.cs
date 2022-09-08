using Gantry.Core;
using Gantry.Core.Contracts;
using JetBrains.Annotations;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.Repositories.Commands
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ModifyWaypointCommand : ICommand
    {
        private readonly string _command;

        public ModifyWaypointCommand(int index, string icon, bool pinned, string colour, string title)
        {
            _command = $"/waypoint modify {index} {colour} {icon} {pinned} {title}";
        }

        public void Execute()
        {
            ApiEx.Client.SendChatMessage(_command);
        }
    }
}