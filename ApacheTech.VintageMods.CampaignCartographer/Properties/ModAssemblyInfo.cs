using ApacheTech.VintageMods.Core.Annotation.Attributes;
using Vintagestory.API.Common;

// ReSharper disable StringLiteralTypo

[assembly: ModDependency("game", "1.16.3")]
[assembly: ModDependency("survival", "1.16.3")]

[assembly:ModInfo(
    "Campaign Cartographer",
    "campaigncartographer",
    Description = "Adds multiple map related features to the game, custom marker pins, GPS, automatic waypoint markers, and more.",
    Side = "Universal",
    Version = "2.2.1",
    RequiredOnClient = true,
    RequiredOnServer = false,
    NetworkVersion = "1.0.0",
    Website = "https://apachetech.co.uk",
    Contributors = new[] { "Apache", "Doombox", "Melchior", "Novocain", "egocarib", "Craluminum2413", "Aledark", "Th3Dilly" },
    Authors = new []{ "Apache" })]

[assembly: VintageModInfo(
    Side = EnumAppSide.Universal,
    ModId = "campaigncartographer",
    ModName = "Campaign Cartographer",
    RootDirectoryName = "CampaignCartographer",
    NetworkVersion = "1.0.0",
    Version = "2.2.1")]