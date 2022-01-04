using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApacheTech.VintageMods.CampaignCartographer.Extensions;
using ApacheTech.VintageMods.Core.Extensions.System;
using ApacheTech.VintageMods.Core.Services;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using Color = System.Drawing.Color;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins
{
    /// <summary>
    ///     A replacement for the game's vanilla <see cref="PlayerMapLayer"/> class.
    /// </summary>
    /// <seealso cref="MarkerMapLayer" />
    /// <seealso cref="IDisposable" />
    public class PlayerPinsMapLayer : MarkerMapLayer, IDisposable
    {
        private readonly ICoreClientAPI _capi;

        private readonly PlayerPinsSettings _settings;

        private IDictionary<IPlayer, EntityMapComponent> PlayerPins { get; }

        private Dictionary<string, LoadedTexture> PlayerPinTextures { get; }

        /// <summary>
        /// 	Initialises a new instance of the <see cref="PlayerPinsMapLayer"/> class.
        /// </summary>
        public PlayerPinsMapLayer(ICoreAPI api, IWorldMapManager mapManager) : base(api, mapManager)
        {
            _capi = api as ICoreClientAPI;
            _settings = ModServices.IOC.Resolve<PlayerPinsSettings>();
            PlayerPins = new Dictionary<IPlayer, EntityMapComponent>();
            PlayerPinTextures = new Dictionary<string, LoadedTexture>();
        }

        public override string Title => "Players";

        public override EnumMapAppSide DataSide => EnumMapAppSide.Client;

        public override void OnLoaded()
        {
            if (_capi == null) return;
            _capi.Event.PlayerEntitySpawn += OnPlayerDespawn;
            _capi.Event.PlayerEntityDespawn += OnPlayerSpawn;
        }

        private void OnPlayerDespawn(IPlayer player)
        {
            if (!PlayerPins.TryGetValue(player, out var entityMapComponent)) return;
            entityMapComponent.Dispose();
            PlayerPins.Remove(player);
        }

        private void OnPlayerSpawn(IPlayer player)
        {
            if (_capi.World.Config.GetBool("mapHideOtherPlayers") &&
                player.PlayerUID != _capi.World.Player.PlayerUID) return;
            if (!mapSink.IsOpened || PlayerPins.ContainsKey(player)) return;
            var value = new EntityMapComponent(_capi, PlayerPinTextures["others"], player.Entity);
            PlayerPins[player] = value;
        }

        public override void Render(GuiElementMap mapElem, float dt)
        {
            foreach (var pin in PlayerPins)
            {
                pin.Value.Render(mapElem, dt);
            }
        }

        public override void OnMouseMoveClient(MouseEvent args, GuiElementMap mapElem, StringBuilder hoverText)
        {
            foreach (var pin in PlayerPins)
            {
                pin.Value.OnMouseMove(args, mapElem, hoverText);
            }
        }

        public override void OnMouseUpClient(MouseEvent args, GuiElementMap mapElem)
        {
            foreach (var pin in PlayerPins)
            {
                pin.Value.OnMouseUpOnElement(args, mapElem);
            }
        }

        public override void Dispose()
        {
            PlayerPins.Empty();
            PlayerPinTextures.Empty();
        }

        public override void OnMapOpenedClient()
        {
            PlayerPinTextures.Empty();
            PlayerPins.Empty();
            PlayerPinTextures["Self"] = LoadTexture(_settings.SelfColour, _settings.SelfScale);
            PlayerPinTextures["Friend"] = LoadTexture(_settings.FriendColour, _settings.FriendScale);
            PlayerPinTextures["Others"] = LoadTexture(_settings.OthersColour, _settings.OthersScale);
            foreach (var player in _capi.World.AllOnlinePlayers)
            {
                if (player.Entity == null)
                {
                    _capi.World.Logger.Warning("Can't add player {0} to world map, missing entity :<", player.PlayerUID);
                }
                else if (!_capi.World.Config.GetBool("mapHideOtherPlayers") || player.PlayerUID == _capi.World.Player.PlayerUID)
                {
                    var textureType = player.PlayerUID == _capi.World.Player.PlayerUID ? "Self" :
                        _settings.Friends.Values.Contains(player.PlayerUID) ? "Friend" : "Others";
                    var comp = new EntityMapComponent(_capi, PlayerPinTextures[textureType], player.Entity);
                    PlayerPins.Add(player, comp);
                }
            }
        }

        private ImageSurface GetTexture(Color colour, int scale)
        {
            scale += 16;
            var rgba = colour.Normalise();
            var outline = new[] { 0d, 0d, 0d, rgba[3] };
            var surface = new ImageSurface(Format.Argb32, scale, scale);
            var ctx = new Context(surface);
            ctx.SetSourceRGBA(0.0, 0.0, 0.0, 0.0);
            ctx.Paint();
            _capi.Gui.Icons.DrawMapPlayer(ctx, 0, 0, scale, scale, outline, rgba);
            return surface;
        }

        private LoadedTexture LoadTexture(Color colour, int scale)
        {
            var surface = GetTexture(colour, scale);
            var texture = _capi.Gui.LoadCairoTexture(surface, false);
            scale += 16;
            return new LoadedTexture(_capi, texture, scale, scale);
        }
    }
}