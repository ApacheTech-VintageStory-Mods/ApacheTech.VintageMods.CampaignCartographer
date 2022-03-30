using System.Collections.Generic;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Registration;
using ApacheTech.VintageMods.Core.Services;

namespace ApacheTech.VintageMods.CampaignCartographer.Infrastructure.Extensions
{
    public sealed class ModMenuDialogue
    {
        public Dictionary<GenericDialogue, string> FeatureDialogues { get; } = new();
    }

    public sealed class ModMenuProgram : ClientFeatureRegistrar
    {
        public override void ConfigureClientModServices(IServiceCollection services)
        {
            services.RegisterSingleton<ModMenuDialogue>();
        }
    }

    public static class ModMenuExtensions
    {
        public static void RegisterModMenuFeatureDialogue<TDialogue>(string title) where TDialogue : GenericDialogue
        {
            var dialogue = (TDialogue)ModServices.IOC.GetService(typeof(TDialogue));
            ModServices.IOC.Resolve<ModMenuDialogue>().FeatureDialogues.Add(dialogue, title);
        }
    }
}
