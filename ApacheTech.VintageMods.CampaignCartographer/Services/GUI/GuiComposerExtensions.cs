using ApacheTech.VintageMods.Core.Services;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace ApacheTech.VintageMods.CampaignCartographer.Services.GUI
{
    public static class GuiComposerExtensions
    {
        /// <summary>
        /// The default bounds for a menu button.
        /// </summary>
        /// <returns>The default bounds for a menu button.</returns>

        public static ElementBounds DefaultButtonBounds()
        {
            return ElementBounds.Fixed(0.0, 0.0, 0.0, 40.0).WithFixedPadding(0.0, 3.0);
        }

        /// <summary>
        /// The default bounds for a menu button.
        /// </summary>
        /// <returns>The default bounds for a menu button.</returns>

        public static ElementBounds DefaultButtonBounds(this GuiCompositeSettings _) => DefaultButtonBounds();

        public static void RegisterGuiDialogueHotKey<TDialogue>(
            this IInputAPI api, string displayText, GlKeys hotKey, HotkeyType hotKeyType)
            where TDialogue : GuiDialog
        {
            var dialogue = ModServices.IOC.Resolve<TDialogue>();
            api.RegisterHotKey(dialogue.ToggleKeyCombinationCode, displayText, hotKey, hotKeyType);
            api.SetHotKeyHandler(dialogue.ToggleKeyCombinationCode, _ => ToggleGui(dialogue));
        }

        private static bool ToggleGui<TDialogue>(TDialogue dialogue) where TDialogue : GuiDialog
        {
            if (dialogue.IsOpened())
            {
                dialogue.TryClose();
                return true;
            }
            dialogue.TryOpen();
            return true;
        }
    }
}