using System.Diagnostics.CodeAnalysis;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.GameContent.GUI;
using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.ModMenu.Extensions
{
    /// <summary>
    ///     Extension methods to aid registration of hub dialogues.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ModMenuExtensions
    {
        /// <summary>
        ///     Adds an accessibility hub dialogue.
        /// </summary>
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension Method")]
        public static void AddModMenuDialogue<T>(this ICoreClientAPI capi, string title) where T : GenericDialogue
        {
            IOC.Services.Resolve<ModMenu>().FeatureDialogues
                .AddIfNotPresent(typeof(T), LangEx.FeatureString($"{title}.Dialogue", "Title"));
        }
    }
}