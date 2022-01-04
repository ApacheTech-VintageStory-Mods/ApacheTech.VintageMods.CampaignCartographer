using ApacheTech.VintageMods.Core.Annotation.Attributes;
using Vintagestory.API.Common;

// ReSharper disable StringLiteralTypo

[assembly: ModDependency("game", "1.16.0-rc.2")]

[assembly:ModInfo(
    "Campaign Cartographer",
    "campaigncartographer",
    Description = "Adds multiple Cartography related features to the game, such as custom waypoint icons, GPS, auto waypoint markers, and more.",
    Side = "Universal",
    Version = "2.0.0-rc.2",
    RequiredOnClient = false,
    RequiredOnServer = false,
    NetworkVersion = "1.0.0",
    Website = "https://apachetech.co.uk",
    Contributors = new[] { "ApacheTech Solutions", "Doombox", "Melchior" },
    Authors = new []{ "ApacheTech Solutions" })]

[assembly: VintageModInfo(
    Side = EnumAppSide.Universal,
    ModId = "campaigncartographer",
    ModName = "Campaign Cartographer",
    RootDirectoryName = "CampaignCartographer",
    NetworkVersion = "1.0.0",
    Version = "2.0.0")]