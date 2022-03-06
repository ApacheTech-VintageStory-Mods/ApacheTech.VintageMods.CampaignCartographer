using System;
using System.Collections.Generic;
using System.Linq;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.DotNet;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Infrastructure.Dialogue
{
    public class AddEditWaypointDialogue : GenericDialogue
    {
        private readonly Waypoint _waypoint;
        private readonly WaypointTypeMode _mode;
        private readonly int _elementIndex;
        private readonly List<WaypointIconModel> _icons;

        public AddEditWaypointDialogue(ICoreClientAPI capi, Waypoint waypoint, WaypointTypeMode mode, int elementIndex) : base(capi)
        {
            _waypoint = waypoint;
            _mode = mode;
            _elementIndex = elementIndex;
            _icons = WaypointIconModel.GetVanillaIcons();

            var titlePrefix = _mode == WaypointTypeMode.Add ? "AddNew" : "Edit";
            Title = LangEx.FeatureString("WaypointManager.Dialogue", titlePrefix);
            Alignment = EnumDialogArea.CenterMiddle;
            Modal = true;
            ModalTransparency = .4f;
        }
        
        private GuiElementTextInput TitleTextBox => SingleComposer.GetTextInput("txtTitle");
        private GuiElementDropDown ColourComboBox => SingleComposer.GetDropDown("cbxColour");
        private GuiElementCustomDraw ColourPreviewBox => SingleComposer.GetCustomDraw("pbxColour");
        private GuiElementDropDown IconComboBox => SingleComposer.GetDropDown("cbxIcon");
        private GuiElementSwitch PinnedSwitch => SingleComposer.GetSwitch("btnPinned");

        public Action<Waypoint, int> OnOkAction { get; set; }
        public Action<Waypoint, int> OnDeleteAction { get; set; }

        #region Form Composition

        protected override void RefreshValues()
        {
            ApiEx.ClientMain.EnqueueMainThreadTask(() =>
            {
                var colour = NamedColour.FromArgb(_waypoint.Color);
                TitleTextBox.SetValue(_waypoint.Title);
                ColourComboBox.SetSelectedValue(colour);
                ColourPreviewBox.Redraw();
                IconComboBox.SetSelectedValue(_waypoint.Icon);
                PinnedSwitch.SetValue(_waypoint.Pinned);
            }, "");
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            var labelFont = CairoFont.WhiteSmallText();
            var textInputFont = CairoFont.WhiteDetailText();
            var topBounds = ElementBounds.FixedSize(400, 30);

            //
            // Title
            //

            var left = ElementBounds.FixedSize(100, 30).FixedUnder(topBounds, 10);
            var right = ElementBounds.FixedSize(270, 30).FixedUnder(topBounds, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "WaypointTitle"), labelFont, EnumTextOrientation.Right, left, "lblTitle")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "WaypointTitle.HoverText"), textInputFont, 260, left)
                .AddTextInput(right, OnTitleChanged, textInputFont, "txtTitle");

            //
            // Colour
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);
            var cbxColourBounds = right.FlatCopy().WithFixedWidth(230);
            var pbxColourBounds = right.FlatCopy().WithFixedWidth(30).FixedRightOf(cbxColourBounds, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Colour"), labelFont, EnumTextOrientation.Right, left, "lblColour")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Colour.HoverText"), textInputFont, 260, left)
                .AddDropDown(NamedColour.ValuesList(), NamedColour.NamesList(), 0,
                    OnColourValueChanged, cbxColourBounds, textInputFont, "cbxColour")
                .AddDynamicCustomDraw(pbxColourBounds, OnDrawColour, "pbxColour");

            //
            // Icon
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Icon"), labelFont, EnumTextOrientation.Right, left, "lblIcon")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Icon.HoverText"), textInputFont, 260, left)
                .AddDropDown(_icons.Select(p => p.Name).ToArray(), _icons.Select(p => p.Glyph).ToArray(), 0, OnIconChanged, right,
                    textInputFont, "cbxIcon");

            //
            // Pinned
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Pinned"), labelFont, EnumTextOrientation.Right, left, "lblPinned")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Pinned.HoverText"), textInputFont, 260, left)
                .AddSwitch(OnPinnedChanged, right, "btnPinned");

            //
            // Buttons
            //

            var controlRowBoundsLeftFixed = ElementBounds.FixedSize(100, 30).WithAlignment(EnumDialogArea.LeftFixed);
            var controlRowBoundsCentreFixed = ElementBounds.FixedSize(100, 30).WithAlignment(EnumDialogArea.CenterFixed);
            var controlRowBoundsRightFixed = ElementBounds.FixedSize(100, 30).WithAlignment(EnumDialogArea.RightFixed);

            composer
                .AddSmallButton(LangEx.GetCore("confirmation-cancel"), OnCancelButtonPressed, controlRowBoundsLeftFixed.FixedUnder(right, 10))
                .AddSmallButton(LangEx.GetCore("confirmation-ok"), OnOkButtonPressed, controlRowBoundsRightFixed.FixedUnder(right, 10));

            if (_mode == WaypointTypeMode.Add) return;
            composer
                .AddSmallButton(LangEx.GetCore("confirmation-delete"), OnDeleteButtonPressed, controlRowBoundsCentreFixed.FixedUnder(right, 10));
        }

        #endregion

        #region Control Event Handlers

        private void OnTitleChanged(string title)
        {
            _waypoint.Title = title;
        }

        private void OnColourValueChanged(string colour, bool selected)
        {
            if (!NamedColour.ValuesList().Contains(colour)) colour = NamedColour.Black;
            _waypoint.Color = colour.ToArgb();
            ColourPreviewBox.Redraw();
        }

        private void OnDrawColour(Context ctx, ImageSurface surface, ElementBounds currentBounds)
        {
            ctx.Rectangle(0.0, 0.0, GuiElement.scaled(25.0), GuiElement.scaled(25.0));
            ctx.SetSourceRGBA(ColorUtil.ToRGBADoubles(_waypoint.Color));
            ctx.FillPreserve();
            ctx.SetSourceRGBA(GuiStyle.DialogBorderColor);
            ctx.Stroke();
        }

        private void OnIconChanged(string icon, bool selected)
        {
            _waypoint.Icon = icon;
        }

        private void OnPinnedChanged(bool state)
        {
            _waypoint.Pinned = state;
        }

        private bool OnCancelButtonPressed()
        {
            return TryClose();
        }

        private bool OnOkButtonPressed()
        {
            OnOkAction?.Invoke(_waypoint, _elementIndex);
            return TryClose();
        }

        private bool OnDeleteButtonPressed()
        {
            OnDeleteAction?.Invoke(_waypoint, _elementIndex);
            return TryClose();
        }

        #endregion
    }
}
