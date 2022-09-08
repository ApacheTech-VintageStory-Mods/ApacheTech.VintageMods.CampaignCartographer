using System;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Services.WaypointTemplates.DataStructures;
using Cairo;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PredefinedWaypoints.Dialogue.PredefinedWaypoints
{
    /// <summary>
    ///     A cell displayed within the cell list on the <see cref="PredefinedWaypointsDialogue"/> screen.
    /// </summary>
    /// <seealso cref="GuiElementTextBase" />
    /// <seealso cref="IGuiElementCell" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PredefinedWaypointsGuiCell : GuiElementTextBase, IGuiElementCell
    {
        public PredefinedWaypointsCellEntry Cell { get; }

        private LoadedTexture _cellTexture;
        private int _switchOnTextureId;
        private int _leftHighlightTextureId;
        private int _rightHighlightTextureId;

        private double _titleTextHeight;
        private const double UnscaledSwitchSize = 25.0;
        private const double UnscaledSwitchPadding = 4.0;
        private const double UnscaledRightBoxWidth = 40.0;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="PredefinedWaypointsGuiCell" /> class.
        /// </summary>
        /// <param name="capi">The capi.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="bounds">The bounds.</param>
        public PredefinedWaypointsGuiCell(ICoreClientAPI capi, PredefinedWaypointsCellEntry cell, ElementBounds bounds) : base(capi, "", null, bounds)
        {
            Cell = cell;
            Bounds = bounds.WithFixedHeight(30);
            _cellTexture = new LoadedTexture(capi);
            Cell.TitleFont ??= CairoFont.WhiteSmallishText();
            Cell.RightTopOffY = 3f;

            if (Cell.DetailTextFont != null) return;
            Cell.DetailTextFont = CairoFont.WhiteSmallText();
            Cell.DetailTextFont.Color[3] *= 0.6;
        }

        public PredefinedWaypointTemplate Model => Cell.Model;

        private void GenerateEnabledTexture()
        {
            var size = scaled(UnscaledSwitchSize - 2.0 * UnscaledSwitchPadding);
            using var imageSurface = new ImageSurface(0, (int)size, (int)size);
            using var context = genContext(imageSurface);
            RoundRectangle(context, 0.0, 0.0, size, size, 2.0);
            fillWithPattern(api, context, waterTextureName);
            generateTexture(imageSurface, ref _switchOnTextureId);
        }

        private void PaintIcon(Context context, double x, double y, double squareSize)
        {
            var padding = scaled(5);
            context.SetSourceRGBA(0.0, 0.0, 0.0, 0.5);
            RoundRectangle(context, x, y, squareSize, squareSize, 3.0);
            context.Fill();
            var colour = Model.Colour.ToColour().ToNormalisedRgba();
            api.Gui.Icons.DrawIcon(context, $"wp{Model.DisplayedIcon.UcFirst()}", x + padding, y + padding, (int)squareSize - 2 * padding, (int)squareSize - 2 * padding, colour);
        }

        private void Compose()
        {
            ComposeHover(true, ref _leftHighlightTextureId);
            ComposeHover(false, ref _rightHighlightTextureId);
            GenerateEnabledTexture();
            using var imageSurface = new ImageSurface(0, Bounds.OuterWidthInt, Bounds.OuterHeightInt);
            using var context = new Context(imageSurface);
            var num = scaled(UnscaledRightBoxWidth);
            Bounds.CalcWorldBounds();

            // Form
            const double brightness = 1.2;
            RoundRectangle(context, 0.0, 0.0, Bounds.OuterWidth, Bounds.OuterHeight, 0.0);
            context.SetSourceRGBA(GuiStyle.DialogDefaultBgColor[0] * brightness, GuiStyle.DialogDefaultBgColor[1] * brightness, GuiStyle.DialogDefaultBgColor[2] * brightness, 1);
            context.Paint();

            // Icon
            const int squareSize = 30;
            const int outerBounds = squareSize + 15;
            PaintIcon(context, Bounds.absPaddingX + 5, Bounds.absPaddingY + 5, squareSize);

            // Main Title.
            Font = Cell.TitleFont;
            _titleTextHeight = textUtil.AutobreakAndDrawMultilineTextAt(context, Font, Cell.Title, Bounds.absPaddingX + outerBounds, Bounds.absPaddingY + scaled(Cell.RightTopOffY * 2), Bounds.InnerWidth - outerBounds);

            // Detail Text.
            Font = Cell.DetailTextFont.WithLineHeightMultiplier(1.5);
            textUtil.AutobreakAndDrawMultilineTextAt(context, Font, Cell.DetailText, Bounds.absPaddingX + squareSize + 10, Bounds.absPaddingY + _titleTextHeight + Bounds.absPaddingY + 5, Bounds.InnerWidth);

            // Top Right Text:
            var textExtents = Font.GetTextExtents(Cell.RightTopText);
            textUtil.AutobreakAndDrawMultilineTextAt(context, Font, Cell.RightTopText,
                Bounds.absPaddingX + Bounds.InnerWidth - textExtents.Width - num - scaled(10.0), Bounds.absPaddingY + scaled(Cell.RightTopOffY), textExtents.Width + 1.0, EnumTextOrientation.Right);

            context.Operator = Operator.Add;
            EmbossRoundRectangleElement(context, 0.0, 0.0, Bounds.OuterWidth, Bounds.OuterHeight, false, 4, 0);

            context.SetSourceRGBA(0.0, 0.0, 0.0, 0.5);
            RoundRectangle(context, 0.0, 0.0, Bounds.OuterWidth, Bounds.OuterHeight, 1.0);
            context.Fill();

            var scaledSwitchSize = scaled(UnscaledSwitchSize);
            var scaledSwitchPadding = scaled(UnscaledSwitchPadding);
            var x = Bounds.absPaddingX + Bounds.InnerWidth - scaled(0.0) - scaledSwitchSize - scaledSwitchPadding;
            var y = Bounds.absPaddingY + Bounds.absPaddingY;
            context.SetSourceRGBA(0.0, 0.0, 0.0, 0.2);
            RoundRectangle(context, x, y, scaledSwitchSize, scaledSwitchSize, 3.0);
            context.Fill();
            EmbossRoundRectangleElement(context, x, y, scaledSwitchSize, scaledSwitchSize, true, 1, 2);
            generateTexture(imageSurface, ref _cellTexture);
        }

        private void ComposeHover(bool left, ref int textureId)
        {
            var imageSurface = new ImageSurface(0, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            var context = genContext(imageSurface);
            var num = scaled(UnscaledRightBoxWidth);
            if (left)
            {
                context.NewPath();
                context.LineTo(0.0, 0.0);
                context.LineTo(Bounds.InnerWidth - num, 0.0);
                context.LineTo(Bounds.InnerWidth - num, Bounds.OuterHeight);
                context.LineTo(0.0, Bounds.OuterHeight);
                context.ClosePath();
            }
            else
            {
                context.NewPath();
                context.LineTo(Bounds.InnerWidth - num, 0.0);
                context.LineTo(Bounds.OuterWidth, 0.0);
                context.LineTo(Bounds.OuterWidth, Bounds.OuterHeight);
                context.LineTo(Bounds.InnerWidth - num, Bounds.OuterHeight);
                context.ClosePath();
            }
            context.SetSourceRGBA(0.0, 0.0, 0.0, 0.15);
            context.Fill();
            generateTexture(imageSurface, ref textureId);
            context.Dispose();
            imageSurface.Dispose();
        }

        public void OnRenderInteractiveElements(ICoreClientAPI capi, float deltaTime)
        {
            if (_cellTexture.TextureId == 0)
            {
                Compose();
            }
            api.Render.Render2DTexturePremultipliedAlpha(_cellTexture.TextureId, (int)Bounds.absX, (int)Bounds.absY, Bounds.OuterWidthInt, Bounds.OuterHeightInt);
            var mouseX = api.Input.MouseX;
            var mouseY = api.Input.MouseY;
            var vec2d = Bounds.PositionInside(mouseX, mouseY);
            if (vec2d != null)
            {
                api.Render.Render2DTexturePremultipliedAlpha(
                    vec2d.X > Bounds.InnerWidth - scaled(GuiElementMainMenuCell.unscaledRightBoxWidth)
                        ? _rightHighlightTextureId
                        : _leftHighlightTextureId, (int)Bounds.absX, (int)Bounds.absY, Bounds.OuterWidth,
                    Bounds.OuterHeight);
            }
            if (On)
            {
                var num = scaled(UnscaledSwitchSize - 2.0 * UnscaledSwitchPadding);
                var num2 = scaled(UnscaledSwitchPadding);
                var posX = Bounds.renderX + Bounds.InnerWidth - num + num2 - scaled(5.0);
                var posY = Bounds.renderY + scaled(8.0) + num2;
                api.Render.Render2DTexturePremultipliedAlpha(_switchOnTextureId, posX, posY, (int)num, (int)num);
                return;
            }
            api.Render.Render2DTexturePremultipliedAlpha(_rightHighlightTextureId, (int)Bounds.renderX, (int)Bounds.renderY, Bounds.OuterWidth, Bounds.OuterHeight);
            api.Render.Render2DTexturePremultipliedAlpha(_leftHighlightTextureId, (int)Bounds.renderX, (int)Bounds.renderY, Bounds.OuterWidth, Bounds.OuterHeight);
        }

        public void UpdateCellHeight()
        {
            Bounds.CalcWorldBounds();
            var paddingY = Bounds.absPaddingY / RuntimeEnv.GUIScale;
            var width = Bounds.InnerWidth;

            var innerWidth = (int)(Bounds.InnerHeight - Bounds.absPaddingY * 2.0 - 10.0);
            width -= innerWidth + 10;

            Font = Cell.TitleFont;
            text = Cell.Title;

            _titleTextHeight = textUtil.GetMultilineTextHeight(Font, Cell.Title, width) / RuntimeEnv.GUIScale;
            Font = Cell.DetailTextFont;
            text = Cell.DetailText;
            var num4 = textUtil.GetMultilineTextHeight(Font, Cell.DetailText, width) / RuntimeEnv.GUIScale;

            Bounds.fixedHeight = paddingY + _titleTextHeight + paddingY + num4 + paddingY;
            if (Bounds.fixedHeight > 50.0)
            {
                Bounds.fixedHeight = 50.0;
            }
        }

        public void OnMouseDownOnElement(MouseEvent args, int elementIndex)
        {
        }

        public void OnMouseMoveOnElement(MouseEvent args, int elementIndex)
        {
        }

        public void OnMouseUpOnElement(MouseEvent args, int elementIndex)
        {
            var mouseX = api.Input.MouseX;
            var mouseY = api.Input.MouseY;
            var vec2d = Bounds.PositionInside(mouseX, mouseY);
            api.Gui.PlaySound("menubutton_press");
            if (vec2d.X > Bounds.InnerWidth - scaled(GuiElementMainMenuCell.unscaledRightBoxWidth))
            {
                OnMouseDownOnCellRight?.Invoke(elementIndex);
                args.Handled = true;
                return;
            }
            OnMouseDownOnCellLeft?.Invoke(elementIndex);
            args.Handled = true;
        }

        public new ElementBounds Bounds { get; }

        public Action<int> OnMouseDownOnCellLeft { private get; init; }

        public Action<int> OnMouseDownOnCellRight { private get; init; }

        public bool On { get; set; } = true;

        public override void Dispose()
        {
            _cellTexture?.Dispose();
            api.Render.GLDeleteTexture(_leftHighlightTextureId);
            api.Render.GLDeleteTexture(_rightHighlightTextureId);
            api.Render.GLDeleteTexture(_switchOnTextureId);
            base.Dispose();
        }
    }
}