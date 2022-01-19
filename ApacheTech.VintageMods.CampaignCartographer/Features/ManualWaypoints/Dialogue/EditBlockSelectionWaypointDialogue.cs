using System;
using System.Collections.Generic;
using System.Linq;
using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions;
using ApacheTech.VintageMods.Core.Extensions.System;
using ApacheTech.VintageMods.Core.GameContent.AssetEnum;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Annotation;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Dialogue
{
    /// <summary>
    ///     GUI window that enables the player to be able to edit the template for the Block Selection Waypoints they add.
    /// </summary>
    /// <seealso cref="GenericDialogue" />
    public class EditBlockSelectionWaypointDialogue : GenericDialogue
    {
        private readonly BlockSelectionWaypointTemplate _waypoint;
        private readonly List<WaypointIconModel> _icons;
        
        /// <summary>
        /// 	Initialises a new instance of the <see cref="EditBlockSelectionWaypointDialogue"/> class.
        /// </summary>
        /// <param name="waypoint">The current template to edit.</param>
        [SidedConstructor(EnumAppSide.Client)]
        private EditBlockSelectionWaypointDialogue(BlockSelectionWaypointTemplate waypoint) : base(ApiEx.Client)
        {
            Title = LangEx.FeatureString("ManualWaypoints.Dialogue.BlockSelection", "Title");
            Alignment = EnumDialogArea.CenterMiddle;
            _waypoint = waypoint;
            _icons = WaypointIconModel.GetVanillaIcons();
        }

        /// <summary>
        /// 	Initialises a new instance of the <see cref="EditBlockSelectionWaypointDialogue"/> class.
        /// </summary>
        /// <param name="waypoint">The current template to edit.</param>
        public static EditBlockSelectionWaypointDialogue Create(BlockSelectionWaypointTemplate waypoint)
        {
            return new EditBlockSelectionWaypointDialogue(waypoint);
        }
        
        private GuiElementDropDown ColourComboBox => SingleComposer.GetDropDown("cbxColour");
        private GuiElementCustomDraw ColourPreviewBox => SingleComposer.GetCustomDraw("pbxColour");
        private GuiElementDropDown IconComboBox => SingleComposer.GetDropDown("cbxIcon");
        private GuiElementNumberInput HorizontalRadiusTextBox => SingleComposer.GetNumberInput("txtHorizontalRadius");
        private GuiElementNumberInput VerticalRadiusTextBox => SingleComposer.GetNumberInput("txtVerticalRadius");

        public Action<BlockSelectionWaypointTemplate> OnOkAction { get; set; }

        #region Form Composition

        protected override void RefreshValues()
        {
            ApiEx.ClientMain.EnqueueMainThreadTask(() =>
            {
                ColourComboBox.SetSelectedValue(_waypoint.Colour.ToLowerInvariant());
                ColourPreviewBox.Redraw();
                IconComboBox.SetSelectedValue(_waypoint.DisplayedIcon);
                HorizontalRadiusTextBox.SetValue(_waypoint.HorizontalCoverageRadius);
                VerticalRadiusTextBox.SetValue(_waypoint.VerticalCoverageRadius);
            }, "");
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            var labelFont = CairoFont.WhiteSmallText();
            var textInputFont = CairoFont.WhiteDetailText();
            var topBounds = ElementBounds.FixedSize(400, 30);

            //
            // Colour
            //

            var left = ElementBounds.FixedSize(100, 30).FixedUnder(topBounds, 10);
            var right = ElementBounds.FixedSize(270, 30).FixedUnder(topBounds, 10).FixedRightOf(left, 10);

            var cbxColourBounds = right.FlatCopy().WithFixedWidth(230);
            var pbxColourBounds = right.FlatCopy().WithFixedWidth(30).FixedRightOf(cbxColourBounds, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.BlockSelection", "Colour"), labelFont, EnumTextOrientation.Right, left, "lblColour")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.BlockSelection", "Colour.HoverText"), textInputFont, 260, left)
                .AddDropDown(NamedColour.ValuesList(), NamedColour.NamesList(), 0,
                    OnColourValueChanged, cbxColourBounds, textInputFont, "cbxColour")
                .AddDynamicCustomDraw(pbxColourBounds, OnDrawColour, "pbxColour");

            //
            // Icon
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.BlockSelection", "Icon"), labelFont, EnumTextOrientation.Right, left, "lblIcon")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.BlockSelection", "Icon.HoverText"), textInputFont, 260, left)
                .AddDropDown(_icons.Select(p => p.Name).ToArray(), _icons.Select(p => p.Glyph).ToArray(), 0, OnIconChanged, right,
                    textInputFont, "cbxIcon");

            //
            // Horizontal Radius
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.BlockSelection", "HCoverage"), labelFont, EnumTextOrientation.Right, left, "lblHorizontalRadius")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.BlockSelection", "HCoverage.HoverText"), textInputFont, 260, left)
                .AddNumberInput(right, OnHorizontalRadiusChanged, textInputFont, "txtHorizontalRadius");

            //
            // Vertical Radius
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("ManualWaypoints.Dialogue.BlockSelection", "VCoverage"), labelFont, EnumTextOrientation.Right, left, "lblVerticalRadius")
                .AddHoverText(LangEx.FeatureString("ManualWaypoints.Dialogue.BlockSelection", "VCoverage.HoverText"), textInputFont, 260, left)
                .AddNumberInput(right, OnVerticalRadiusChanged, textInputFont, "txtVerticalRadius");

            //
            // Buttons
            //

            var controlRowBoundsLeftFixed = ElementBounds.FixedSize(150, 30).WithAlignment(EnumDialogArea.LeftFixed);
            var controlRowBoundsRightFixed = ElementBounds.FixedSize(150, 30).WithAlignment(EnumDialogArea.RightFixed);

            composer
                .AddSmallButton(LangEx.GetCore("confirmation-cancel"), OnCancelButtonPressed, controlRowBoundsLeftFixed.FixedUnder(left, 10))
                .AddSmallButton(LangEx.GetCore("confirmation-ok"), OnOkButtonPressed, controlRowBoundsRightFixed.FixedUnder(right, 10));
        }

        #endregion

        #region Control Event Handlers

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

        private static int ParseRadiusValue(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 10;
            return int.TryParse(input.GetNumbers(), out var value)
                ? value : 10;
        }

        private void OnHorizontalRadiusChanged(string input)
        {
            var radius = ParseRadiusValue(input);
            _waypoint.HorizontalCoverageRadius = GameMath.Clamp(radius, 0, 50);

            if (!input.OnlyContainsNumbers() || radius is > 50 or < 0 || string.IsNullOrWhiteSpace(input))
            {
                HorizontalRadiusTextBox.SetValue(GameMath.Clamp(radius, 0, 50));
            }
        }

        private void OnVerticalRadiusChanged(string input)
        {
            var radius = ParseRadiusValue(input);
            _waypoint.VerticalCoverageRadius = GameMath.Clamp(radius, 0, 50);

            if (!input.OnlyContainsNumbers() || radius is > 50 or < 0 || string.IsNullOrWhiteSpace(input))
            {
                VerticalRadiusTextBox.SetValue(GameMath.Clamp(radius, 0, 50));
            }
        }

        private bool OnCancelButtonPressed()
        {
            return TryClose();
        }

        private bool OnOkButtonPressed()
        {
            OnOkAction?.Invoke(_waypoint);
            return TryClose();
        }

        #endregion
    }
}