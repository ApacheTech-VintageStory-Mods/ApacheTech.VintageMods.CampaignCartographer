using System;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets;
using ApacheTech.VintageMods.Core.Extensions.System;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Exports
{
    /// <summary>
    ///     A cell displayed within the cell list on the <see cref="WaypointExportDialogue"/> screen.
    /// </summary>
    /// <seealso cref="GuiElementTextBase" />
    /// <seealso cref="IGuiElementCell" />
    public class WaypointExportGuiCell : GuiElementTextBase, IGuiElementCell
    {
        private readonly WaypointExportCellEntry _cell;

        private LoadedTexture _cellTexture;
        private int _switchOnTextureId;
        private int _leftHighlightTextureId;
        private int _rightHighlightTextureId;
        
        private double _titleTextHeight;
        private const double UnscaledSwitchSize = 25.0;
        private const double UnscaledSwitchPadding = 4.0;
        private const double UnscaledRightBoxWidth = 40.0;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="WaypointExportGuiCell" /> class.
        /// </summary>
        /// <param name="capi">The capi.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="bounds">The bounds.</param>
        public WaypointExportGuiCell(ICoreClientAPI capi, WaypointExportCellEntry cell, ElementBounds bounds) : base(capi, "", null, bounds)
        {
            _cell = cell;
            Bounds = bounds;
            _cellTexture = new LoadedTexture(capi);
            _cell.TitleFont ??= CairoFont.WhiteSmallishText();
            if (_cell.DetailTextFont != null) return;
            _cell.DetailTextFont = CairoFont.WhiteSmallText();
            _cell.DetailTextFont.Color[3] *= 0.6;
        }
        public WaypointDto Waypoint => _cell.Waypoint;

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
            var colour = Waypoint.Colour.ToColour().Normalise();
            api.Gui.Icons.DrawIcon(context, $"wp{Waypoint.DisplayedIcon.UcFirst()}", x + padding, y + padding, (int)squareSize - 2 * padding, (int)squareSize - 2 * padding, colour);
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
            var squareSize = (Bounds.InnerHeight - Bounds.absPaddingY * 2.0 - 10.0);
            var outerBounds = squareSize + 15;
            PaintIcon(context, Bounds.absPaddingX + 5, Bounds.absPaddingY + 5, squareSize);

            // Main Title.
            Font = _cell.TitleFont;
            _titleTextHeight = textUtil.AutobreakAndDrawMultilineTextAt(context, Font, _cell.Title, Bounds.absPaddingX + outerBounds, Bounds.absPaddingY, Bounds.InnerWidth - outerBounds);

            // Detail Text.
            Font = _cell.DetailTextFont;
            textUtil.AutobreakAndDrawMultilineTextAt(context, Font, _cell.DetailText, Bounds.absPaddingX + outerBounds, Bounds.absPaddingY + _titleTextHeight + Bounds.absPaddingY, Bounds.InnerWidth - outerBounds);

            // Top Right Text: Location of Waypoint, relative to spawn.
            var textExtents = Font.GetTextExtents(_cell.RightTopText);
            textUtil.AutobreakAndDrawMultilineTextAt(context, Font, _cell.RightTopText,
                Bounds.absPaddingX + Bounds.InnerWidth - textExtents.Width - num - scaled(10.0), Bounds.absPaddingY + scaled(_cell.RightTopOffY), textExtents.Width + 1.0, EnumTextOrientation.Right);

            context.Operator = Operator.Add;
            EmbossRoundRectangleElement(context, 0.0, 0.0, Bounds.OuterWidth, Bounds.OuterHeight, false, 4, 0);
            if (string.IsNullOrWhiteSpace(Waypoint.DetailText))
            {
                context.SetSourceRGBA(0.0, 0.0, 0.0, 0.5);
                RoundRectangle(context, 0.0, 0.0, Bounds.OuterWidth, Bounds.OuterHeight, 1.0);
                context.Fill();
            }
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
            using var imageSurface = new ImageSurface(0, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            using var context = genContext(imageSurface);
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

            Font = _cell.TitleFont;
            text = _cell.Title;

            _titleTextHeight = textUtil.GetMultilineTextHeight(Font, _cell.Title, width) / RuntimeEnv.GUIScale;
            Font = _cell.DetailTextFont;
            text = _cell.DetailText;
            var num4 = textUtil.GetMultilineTextHeight(Font, _cell.DetailText, width) / RuntimeEnv.GUIScale;

            Bounds.fixedHeight = paddingY + _titleTextHeight + paddingY + num4 + paddingY;
            if (Bounds.fixedHeight < 50.0)
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
