using ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Handlers;
using ApacheTech.VintageMods.Core.Abstractions.ChatCommandBrokers;
using ApacheTech.VintageMods.Core.Services;
using JetBrains.Annotations;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Broker
{
    /// <summary>
    ///     
    /// </summary>
    /// <seealso cref="ServerChatCommandBroker" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class GPSChatCommandBroker : ServerChatCommandBroker
    {
        /// <summary>
        /// 	Initialises a new instance of the <see cref="GPSChatCommandBroker"/> class.
        /// </summary>
        public GPSChatCommandBroker()
        {
            Options.Add("whisper", ModServices.IOC
                .Resolve<WhisperCommandHandler>()
                .OnServerWhisperCommandHandler);
        }
    }
}