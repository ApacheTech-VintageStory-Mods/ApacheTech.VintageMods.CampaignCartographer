using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.System;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using ApacheTech.VintageMods.Core.GameContent.GUI;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Annotation;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue
{
    public class AddEditWaypointTypeDialogue : GenericDialogue
    {
        private readonly ManualWaypointTemplateModel _waypoint;
        private readonly WaypointTypeMode _mode;
        private readonly List<WaypointIconModel> _icons;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="AddEditWaypointTypeDialogue"/> class.
        /// </summary>
        /// <param name="capi">The capi.</param>
        /// <param name="waypoint">The waypoint.</param>
        /// <param name="mode"></param>
        [Obsolete("Use Factory Method: WaypointInfoDialogue.ShowDialogue()")]
        [SidedConstructor(EnumAppSide.Client)]
        public AddEditWaypointTypeDialogue(ICoreClientAPI capi, ManualWaypointTemplateModel waypoint, WaypointTypeMode mode) : base(capi)
        {
            _waypoint = waypoint;
            _mode = mode;
            _icons = WaypointIconModel.GetVanillaIcons();

            var titlePrefix = _mode == WaypointTypeMode.Add ? "AddNew" : "Edit";
            Title = LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", titlePrefix);
            Alignment = EnumDialogArea.CenterMiddle;
        }

        private GuiElementTextInput SyntaxTextBox => SingleComposer.GetTextInput("txtSyntax");
        private GuiElementTextInput TitleTextBox => SingleComposer.GetTextInput("txtTitle");
        private GuiElementTextInput DetailsTextBox => SingleComposer.GetTextInput("txtDetailText");
        private GuiElementDropDown ColourComboBox => SingleComposer.GetDropDown("cbxColour");
        private GuiElementCustomDraw ColourPreviewBox => SingleComposer.GetCustomDraw("pbxColour");
        private GuiElementDropDown IconComboBox => SingleComposer.GetDropDown("cbxIcon");
        private GuiElementSlider HorizontalRadiusTextBox => SingleComposer.GetSlider("txtHorizontalRadius");
        private GuiElementSlider VerticalRadiusTextBox => SingleComposer.GetSlider("txtVerticalRadius");
        private GuiElementSwitch PinnedSwitch => SingleComposer.GetSwitch("btnPinned");
        public Action<ManualWaypointTemplateModel> OnOkAction { get; set; }
        public Action<ManualWaypointTemplateModel> OnDeleteAction { get; set; }

        #region Form Composition

        protected override void RefreshValues()
        {
            ApiEx.ClientMain.EnqueueMainThreadTask(() =>
            {
                if (_mode == WaypointTypeMode.Add)
                {
                    SyntaxTextBox.SetValue(_waypoint.Syntax);
                }
                TitleTextBox.SetValue(_waypoint.Title);
                DetailsTextBox.SetValue(_waypoint.DetailText);
                ColourComboBox.SetSelectedValue(_waypoint.Colour.ToLowerInvariant());
                ColourPreviewBox.Redraw();
                IconComboBox.SetSelectedValue(_waypoint.DisplayedIcon);
                HorizontalRadiusTextBox.SetValues(_waypoint.HorizontalCoverageRadius, 0, 50, 1);
                VerticalRadiusTextBox.SetValues(_waypoint.VerticalCoverageRadius, 0, 50, 1);
                PinnedSwitch.SetValue(_waypoint.Pinned);
            }, "");
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            var labelFont = CairoFont.WhiteSmallText();
            var textInputFont = CairoFont.WhiteDetailText();
            var topBounds = ElementBounds.FixedSize(400, 30);

            //
            // Syntax
            //

            var left = ElementBounds.FixedSize(100, 30).FixedUnder(topBounds, 10);
            var right = ElementBounds.FixedSize(270, 30).FixedUnder(topBounds, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Syntax"), labelFont, EnumTextOrientation.Right, left.WithFixedOffset(0, 5), "lblSyntax")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Syntax.HoverText"), textInputFont, 260, left)
                .AddIf(_mode == WaypointTypeMode.Add)
                .AddTextInput(right, OnSyntaxChanged, textInputFont, "txtSyntax")
                .EndIf()
                .AddIf(_mode == WaypointTypeMode.Edit)
                .AddStaticText(_waypoint.Syntax, textInputFont, EnumTextOrientation.Left, right.WithFixedOffset(0, 5))
                .EndIf();

            //
            // Title
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "WaypointTitle"), labelFont, EnumTextOrientation.Right, left, "lblTitle")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "WaypointTitle.HoverText"), textInputFont, 260, left)
                .AddTextInput(right, OnTitleChanged, textInputFont, "txtTitle");

            //
            // Detail Text
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "DetailText"), labelFont, EnumTextOrientation.Right, left, "lblDetailText")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "DetailText.HoverText"), textInputFont, 260, left)
                .AddTextInput(right, OnDetailTextChanged, textInputFont, "txtDetailText");

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
            // Horizontal Radius
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "HCoverage"), labelFont, EnumTextOrientation.Right, left, "lblHorizontalRadius")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "HCoverage.HoverText"), textInputFont, 260, left)
                .AddSlider(OnHorizontalRadiusChanged, right.FlatCopy().WithFixedHeight(20), "txtHorizontalRadius");

            //
            // Vertical Radius
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "VCoverage"), labelFont, EnumTextOrientation.Right, left, "lblVerticalRadius")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "VCoverage.HoverText"), textInputFont, 260, left)
                .AddSlider(OnVerticalRadiusChanged, right.FlatCopy().WithFixedHeight(20), "txtVerticalRadius");

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

        private void OnSyntaxChanged(string syntax)
        {
            _waypoint.Syntax = syntax.ToLowerInvariant();
        }

        private void OnTitleChanged(string title)
        {
            _waypoint.Title = title;
        }

        private void OnDetailTextChanged(string detailText)
        {
            _waypoint.DetailText = detailText;
        }

        private void OnColourValueChanged(string colour, bool selected)
        {
            if (!NamedColour.ValuesList().Contains(colour)) colour = NamedColour.Black;
            _waypoint.Colour = colour;
            ColourPreviewBox.Redraw();
        }

        private void OnDrawColour(Context ctx, ImageSurface surface, ElementBounds currentBounds)
        {
            ctx.Rectangle(0.0, 0.0, GuiElement.scaled(25.0), GuiElement.scaled(25.0));
            ctx.SetSourceRGBA(ColorUtil.ToRGBADoubles(_waypoint.Colour.ColourValue()));
            ctx.FillPreserve();
            ctx.SetSourceRGBA(GuiStyle.DialogBorderColor);
            ctx.Stroke();
        }

        private void OnIconChanged(string icon, bool selected)
        {
            _waypoint.DisplayedIcon = icon;
            _waypoint.ServerIcon = icon;
        }

        private bool OnHorizontalRadiusChanged(int radius)
        {
            _waypoint.HorizontalCoverageRadius = radius;
            return true;
        }

        private bool OnVerticalRadiusChanged(int radius)
        {
            _waypoint.VerticalCoverageRadius = radius;
            return true;
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
            // ROADMAP: O/C issues with validation.
            var validationErrors = false;
            var message = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_waypoint.Syntax) || _waypoint.Syntax.Contains(" "))
            {
                message.AppendLine(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Syntax.Validation"));
                message.AppendLine();
                validationErrors = true;
            }

            if (_waypoint.HorizontalCoverageRadius < 0 || _waypoint.VerticalCoverageRadius < 0)
            {
                message.AppendLine(LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Coverage.Validation"));
                message.AppendLine();
                validationErrors = true;
            }

            if (validationErrors)
            {
                var title = LangEx.Get("ModTitle");
                MessageBox.Show(title, message.ToString());
                return false;
            }

            OnOkAction?.Invoke(_waypoint);
            return TryClose();
        }

        private bool OnDeleteButtonPressed()
        {
            var title = LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Delete.Title");
            var message = LangEx.FeatureString("ManualWaypoints.Dialogue.WaypointType", "Delete.Message");
            MessageBox.Show(title, message, ButtonLayout.OkCancel,
                () =>
                {
                    OnDeleteAction?.Invoke(_waypoint);
                    TryClose();
                });
            return true;
        }

        #endregion
    }
}