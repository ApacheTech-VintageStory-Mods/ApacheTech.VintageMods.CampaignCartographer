using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Packets;
using ApacheTech.VintageMods.Core.Abstractions.GUI;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions;
using ApacheTech.VintageMods.Core.Hosting.DependencyInjection.Annotation;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.Core.Services.FileSystem.FileAdaptors;
using Newtonsoft.Json;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.WaypointUtil.Dialogue.Exports
{
    public class WaypointExportConfirmationDialogue : GenericDialogue
    {
        private readonly WaypointFileModel _model;
        private string _fileName;
        private readonly string _defaultFileName = $"{DateTime.Now:yyyy.M.dd HH-mm-ss}.json";
        private readonly string _defaultName = LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "DefaultName");

        [SidedConstructor(EnumAppSide.Client)]
        public WaypointExportConfirmationDialogue(ICoreClientAPI capi, List<WaypointDto> waypoints) : base(capi)
        {
            Title = LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "ConfirmationTitle");
            Alignment = EnumDialogArea.CenterMiddle;
            Modal = true;
            _model = new WaypointFileModel
            {
                Waypoints = waypoints,
                Count = waypoints.Count
            };
        }

        public static void ShowDialogue(List<WaypointDto> waypoints)
        {
            ModServices.IOC.Resolve<ShowConfirmExportDialogue>()(waypoints).TryOpen();
        }

        #region Form Composition

        protected override void ComposeBody(GuiComposer composer)
        {
            var titleFont = CairoFont.WhiteSmallishText();
            var labelFont = CairoFont.WhiteSmallText();
            var textInputFont = CairoFont.WhiteDetailText().WithOrientation(EnumTextOrientation.Center);

            var titleText = LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "Title", _model.Count.FormatLargeNumber());
            var confirmButtonText = LangEx.GetCore("confirmation-export");
            var cancelButtonText = LangEx.GetCore("confirmation-cancel");

            //
            // Title
            //

            var titleBounds = ElementBounds.FixedSize(400, 30);

            composer
                .AddStaticText(titleText, titleFont, EnumTextOrientation.Center, titleBounds.WithFixedOffset(0, 30), "lblConfirmationMessage");

            //
            // Exports Name
            //

            var left = ElementBounds.FixedSize(100, 30).FixedUnder(titleBounds, 10);
            var right = ElementBounds.FixedSize(270, 30).FixedUnder(titleBounds, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "Name"), 
                    labelFont, EnumTextOrientation.Right, left.WithFixedOffset(0, 5), "lblName")
                .AddHoverText(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "Name.HoverText"), textInputFont, 260, left)
                .AddTextInput(right, OnNameTextChanged, textInputFont, "txtName");

            //
            // World Name
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "World"), 
                    labelFont, EnumTextOrientation.Right, left, "lblWorld")
                .AddHoverText(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "World.HoverText"), textInputFont, 260, left)
                .AddTextInput(right, OnWorldNameTextChanged, textInputFont, "txtWorld");

            //
            // File Name
            //

            left = ElementBounds.FixedSize(100, 30).FixedUnder(left, 10);
            right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

            composer
                .AddStaticText(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "FileName"), 
                    labelFont, EnumTextOrientation.Right, left, "lblFilename")
                .AddHoverText(LangEx.FeatureString("WaypointUtil.Dialogue.Exports", "FileName.HoverText"), textInputFont, 260, left)
                .AddTextInput(right, OnFileNameTextChanged, textInputFont, "txtFileName");

            //
            // Buttons
            //

            var controlRowBoundsLeftFixed = ElementBounds.FixedSize(150, 30).WithAlignment(EnumDialogArea.LeftFixed);
            var controlRowBoundsRightFixed = ElementBounds.FixedSize(150, 30).WithAlignment(EnumDialogArea.RightFixed);

            composer
                .AddSmallButton(cancelButtonText, OnCancelButtonPressed, controlRowBoundsLeftFixed.FixedUnder(left, 10))
                .AddSmallButton(confirmButtonText, OnExportButtonPressed, controlRowBoundsRightFixed.FixedUnder(right, 10));
        }

        protected override void RefreshValues()
        {
            Task.Factory.StartNew(async () =>
            {
                var worldName = await ModServices.IOC.Resolve<WorldNameRelay>().GetWorldNameAsync();
                ApiEx.ClientMain.EnqueueMainThreadTask(() =>
                {
                    var txtName = SingleComposer.GetTextInput("txtName");
                    txtName.SetPlaceHolderText(_defaultName);
                    txtName.SetValue(_defaultName);
                    _model.Name = _defaultName;

                    var txtWorld = SingleComposer.GetTextInput("txtWorld");
                    txtWorld.SetPlaceHolderText(worldName);
                    txtWorld.SetValue(worldName);
                    _model.World = worldName;

                    var txtFileName = SingleComposer.GetTextInput("txtFileName");
                    txtFileName.SetPlaceHolderText(_defaultFileName);
                    txtFileName.SetValue(_fileName.IfNullOrWhitespace(_defaultFileName));
                }, "");
            });
        }

        #endregion

        #region Control Event Handlers

        private void OnNameTextChanged(string name)
        {
            _model.Name = name;
        }

        private void OnWorldNameTextChanged(string worldName)
        {
            _model.World = worldName;
        }

        private void OnFileNameTextChanged(string fileName)
        {
            _fileName = fileName;
        }

        private bool OnCancelButtonPressed()
        {
            return TryClose();
        }

        // TODO: Business logic.
        // IO
        // Waypoint Management
        // Waypoint Exports
        private bool OnExportButtonPressed()
        {
            _fileName = _fileName.IfNullOrWhitespace(_defaultFileName);
            if (!_fileName.EndsWith(".json"))
            {
                _fileName += ".json";
            }
            var file = new JsonModFile(Path.Combine(ModPaths.ModDataWorldPath, "Exports", _fileName));
            _model.Name = _model.Name.IfNullOrWhitespace(_defaultName);
            _model.DateCreated = DateTime.Now;
            var json = JsonConvert.SerializeObject(_model, Formatting.Indented);
            file.SaveFrom(json);
            return TryClose();
        }

        #endregion
    }
}