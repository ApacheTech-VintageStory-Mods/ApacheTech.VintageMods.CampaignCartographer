using System;
using System.Collections.Generic;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.GameContent.GUI;
using Gantry.Core.GameContent.GUI.Helpers;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.ModMenu.Dialogue
{
    /// <summary>
    ///     User interface that acts as a hub, to bring together all features within the mod.
    /// </summary>
    /// <seealso cref="GenericDialogue" />
    [UsedImplicitly]
    public sealed class ModMenuDialogue : GenericDialogue
    {
        private float _row;
        private const float ButtonWidth = 400f;
        private const float HeightOffset = 200f;
        private readonly Dictionary<Type, string> _dialogues;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="ModMenuDialogue"/> class.
        /// </summary>
        /// <param name="capi">The client API.</param>
        /// <param name="system">The mod system that controls this dialogue window.</param>
        public ModMenuDialogue(ICoreClientAPI capi, ModMenu system) : base(capi)
        {
            Alignment = EnumDialogArea.CenterMiddle;
            Title = LangEx.ModTitle();
            ModalTransparency = 0f;
            ShowTitleBar = true;
            _dialogues = system.FeatureDialogues;
        }

        /// <summary>
        ///     Composes the header for the GUI.
        /// </summary>
        /// <param name="composer">The composer.</param>
        protected override void ComposeBody(GuiComposer composer)
        {
            var squareBounds = ElementBounds.FixedSize(EnumDialogArea.CenterTop, ButtonWidth, HeightOffset).WithFixedOffset(0, 30);
   
            composer
                .AddStaticImage(AssetLocation.Create("campaigncartographer:textures/dialogue/menu-logo.png"), squareBounds);

            foreach (var dialogue in _dialogues)
            {
                AddDialogueButton(composer, dialogue.Value, dialogue.Key);
            }
            IncrementRow(ref _row);
            AddButton(composer, LangEx.FeatureString("PredefinedWaypoints.Dialogue.MenuScreen", "Support"), OnDonateButtonPressed);
            AddButton(composer, Lang.Get("pause-back2game"), TryClose);
        }
        
        private void AddButton(GuiComposer composer, string langEntry, ActionConsumable onClick)
        {
            composer.AddSmallButton(langEntry, onClick, ButtonBounds(ref _row, ButtonWidth, HeightOffset));
        }

        private void AddDialogueButton(GuiComposer composer, string langEntry, Type dialogueType)
        {
            composer.AddSmallButton(langEntry, () => OpenDialogue(dialogueType), ButtonBounds(ref _row, ButtonWidth, HeightOffset));
        }

        private static void IncrementRow(ref float row) => row += 0.5f;

        private static ElementBounds ButtonBounds(ref float row, double width, double height)
        {
            IncrementRow(ref row);
            return ElementStdBounds
                .MenuButton(row, EnumDialogArea.LeftFixed)
                .WithFixedOffset(0, height)
                .WithFixedSize(width, 30);
        }

        private static bool OpenDialogue(Type type)
        {
            ((GuiDialog)IOC.Services.GetService(type)).Toggle();
            return true;
        }

        private static bool OnDonateButtonPressed()
        {
            var dialogue = IOC.Services.Resolve<SupportDialogue>();
            return dialogue.TryOpen();
        }
    }
}
