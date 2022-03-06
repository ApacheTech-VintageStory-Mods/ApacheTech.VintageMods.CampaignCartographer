using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Enums;
using ApacheTech.VintageMods.Core.Abstractions.Features;
using ApacheTech.VintageMods.Core.Extensions;
using ApacheTech.VintageMods.Core.Extensions.DotNet;
using ApacheTech.VintageMods.Core.Services.HarmonyPatching.Annotations;
using Cairo;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using Color = System.Drawing.Color;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins.Patches
{
    [HarmonySidedPatch(EnumAppSide.Client)]
    public class PlayerMapLayerPatches : WorldSettingsConsumer<PlayerPinsSettings>
    {
        private static ICoreClientAPI _capi;
        private static IWorldMapManager _mapSink;
        private static IDictionary<IPlayer, EntityMapComponent> PlayerPins { get; set; }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerMapLayer), MethodType.Constructor, typeof(ICoreAPI), typeof(IWorldMapManager))]
        public static void Patch_PlayerMapLayer_Constructor_Postfix(ICoreAPI api, IWorldMapManager mapsink)
        {
            _capi = (ICoreClientAPI)api;
            _mapSink = mapsink;
            PlayerPins = new Dictionary<IPlayer, EntityMapComponent>();
            _capi.Event.LeaveWorld += DisposeComponents;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMapLayer), "OnLoaded")]
        public static bool Patch_PlayerMapLayer_Event_OnLoaded_Prefix()
        {
            if (_capi is null) return false;
            _capi.Event.PlayerEntitySpawn += OnPlayerSpawn;
            _capi.Event.PlayerEntityDespawn += OnPlayerDespawn;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMapLayer), "OnMapOpenedClient")]
        public static bool Patch_PlayerMapLayer_OnMapOpenedClient_Prefix()
        {
            PlayerPins.Purge();
            foreach (var player in _capi.World.AllOnlinePlayers)
            {
                if (player.Entity == null)
                {
                    _capi.World.Logger.Warning("Can't add player {0} to world map, missing entity :<", player.PlayerUID);
                }
                else if (!_capi.World.Config.GetBool("mapHideOtherPlayers") || player.PlayerUID == _capi.World.Player.PlayerUID)
                {
                    AddPlayerToMap(player);
                }
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMapLayer), "Render")]
        public static bool Patch_PlayerMapLayer_Render_Prefix(GuiElementMap mapElem, float dt)
        {
            foreach (var pin in PlayerPins)
            {
                pin.Value.Render(mapElem, dt);
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMapLayer), "OnMouseMoveClient")]
        public static bool Patch_PlayerMapLayer_OnMouseMoveClient_Prefix(MouseEvent args, GuiElementMap mapElem, StringBuilder hoverText)
        {
            foreach (var pin in PlayerPins)
            {
                pin.Value.OnMouseMove(args, mapElem, hoverText);
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMapLayer), "OnMouseUpClient")]
        public static bool Patch_PlayerMapLayer_OnMouseUpClient_Prefix(MouseEvent args, GuiElementMap mapElem)
        {
            foreach (var pin in PlayerPins)
            {
                pin.Value.OnMouseUpOnElement(args, mapElem);
            }
            return false;
        }

        private static void OnPlayerDespawn(IPlayer player)
        {
            if (!PlayerPins.TryGetValue(player, out var entityMapComponent)) return;
            entityMapComponent.Dispose();
            PlayerPins.Remove(player);
        }

        private static void OnPlayerSpawn(IPlayer player)
        {
            if (_capi.World.Config.GetBool("mapHideOtherPlayers") && player.PlayerUID != _capi.World.Player.PlayerUID) return;

            if (!_mapSink.IsOpened || PlayerPins.ContainsKey(player)) return;

            AddPlayerToMap(player);
        }

        private static void AddPlayerToMap(IPlayer player)
        {
            var textureType = player.PlayerUID == _capi.World.Player.PlayerUID
                ? TextureType.Self
                : Settings.Friends.Values.Contains(player.PlayerUID) ? TextureType.Friends : TextureType.Others;
            
            var texture = textureType switch
            {
                TextureType.Self => LoadTexture(Settings.SelfColour, Settings.SelfScale),
                TextureType.Friends => LoadTexture(Settings.FriendColour, Settings.FriendScale),
                TextureType.Others => LoadTexture(Settings.OthersColour, Settings.OthersScale),
                _ => throw new ArgumentOutOfRangeException(nameof(textureType), textureType, "Cannot add player to map.")
            };

            var comp = new EntityMapComponent(_capi, texture, player.Entity);
            PlayerPins.Add(player, comp);
        }

        private static LoadedTexture LoadTexture(Color colour, int scale)
        {
            scale += 16;

            var rgba = colour.Normalise();
            var outline = new[] { 0d, 0d, 0d, rgba[3] };

            var imageSurface = new ImageSurface(Format.Argb32, scale, scale);
            var context = new Context(imageSurface);

            context.SetSourceRGBA(0.0, 0.0, 0.0, 0.0);
            context.Paint();
            _capi.Gui.Icons.DrawMapPlayer(context, 0, 0, scale, scale, outline, rgba);
            var texture = _capi.Gui.LoadCairoTexture(imageSurface, false);
            var loadedTexture = new LoadedTexture(_capi, texture, scale, scale);

            // Explicitly disposing the context and surface, after each use.
            context.Dispose();
            imageSurface.Dispose();

            return loadedTexture;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMapLayer), "Dispose")]
        public static bool Patch_PlayerMapLayer_Dispose_Prefix()
        {
            DisposeComponents();
            return true;
        }

        private static void DisposeComponents()
        {
            foreach (var comp in PlayerPins.Values)
            {
                comp.Texture.Dispose();
            }
            PlayerPins.Purge();
        }

        ~PlayerMapLayerPatches()
        {
            DisposeComponents();
        }
    }
}