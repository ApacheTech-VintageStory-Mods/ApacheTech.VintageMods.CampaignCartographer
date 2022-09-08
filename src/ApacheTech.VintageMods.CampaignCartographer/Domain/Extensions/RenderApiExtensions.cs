using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.Extensions;
using Gantry.Core.GameContent.AssetEnum;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class RenderApiExtensions
    {
        public static MeshRef Get2dLineMesh(this IRenderAPI api, Waypoint startPoint, Waypoint endPoint)
        {
            var rgbaStart = ColorUtil
                .Int2Hex(startPoint.Color)
                .ToColour()
                .ToNormalisedRgba()
                .ToRgbaVec4F();

            var rgbaEnd = ColorUtil
                .Int2Hex(endPoint.Color)
                .ToColour()
                .ToNormalisedRgba()
                .ToRgbaVec4F();

            var mapManager = IOC.Services.Resolve<WorldMapManager>();

            // Coordinate Translation.
            var startViewPos = Vec2f.Zero;
            var endViewPos = Vec2f.Zero;
            mapManager.TranslateWorldPosToViewPos(startPoint.Position, ref startViewPos);
            mapManager.TranslateWorldPosToViewPos(endPoint.Position, ref endViewPos);

            var mesh = new MeshData();
            mesh.SetXyz(new[] { startViewPos.X, startViewPos.Y, 0, endViewPos.X, endViewPos.Y, 0 });
            mesh.SetVerticesCount(2);
            mesh.SetIndices(new[] { 0, 1 });
            mesh.SetIndicesCount(2);
            mesh.SetRgba(new[]
            {
                (byte)rgbaStart[0], (byte)rgbaStart[1], (byte)rgbaStart[2], (byte)rgbaStart[3],
                (byte)rgbaEnd[0], (byte)rgbaEnd[1], (byte)rgbaEnd[2], (byte)rgbaEnd[3]
            });
            mesh.SetMode(EnumDrawMode.Lines);
            return api.UploadMesh(mesh);
        }

        public static MeshRef Get2dLineMesh(this IRenderAPI api, Vec2f startPos, Vec2f endPos, NamedColour colour)
        {
            var rgba = colour.ToString().ToColour().ToNormalisedRgba();
            var mesh = new MeshData();
            mesh.SetXyz(new[] { startPos.X, startPos.Y, 0, endPos.X, endPos.Y, 0 });
            mesh.SetVerticesCount(2);
            mesh.SetIndices(new[] { 0, 1 });
            mesh.SetIndicesCount(2);
            mesh.SetRgba(new[]
            {
                (byte)rgba[0], (byte)rgba[1], (byte)rgba[2], (byte)rgba[3],
                (byte)rgba[0], (byte)rgba[1], (byte)rgba[2], (byte)rgba[3]
            });
            mesh.SetMode(EnumDrawMode.Lines);
            return api.UploadMesh(mesh);
        }

        public static void Render2DLine(this IRenderAPI api, MeshRef mesh)
        {
            var clientMain = ApiEx.ClientMain;
            var guiShaderProg = ShaderPrograms.Gui;

            // Shader Uniforms.
            guiShaderProg.ExtraGlow = 0;
            guiShaderProg.ApplyColor = 1;
            guiShaderProg.NoTexture = 1f;
            guiShaderProg.OverlayOpacity = 0f;
            guiShaderProg.NormalShaded = 0;

            // Render Line.
            clientMain.GlPushMatrix();
            guiShaderProg.ProjectionMatrix = clientMain.CurrentProjectionMatrix;
            guiShaderProg.ModelViewMatrix = clientMain.CurrentModelViewMatrix;
            api.RenderMesh(mesh);
            clientMain.GlPopMatrix();
        }
    }
}