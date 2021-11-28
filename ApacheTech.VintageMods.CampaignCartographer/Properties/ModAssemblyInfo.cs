using ApacheTech.VintageMods.Core.Annotation.Attributes;
using Vintagestory.API.Common;

[assembly: ModDependency("game", "1.15.9")]

[assembly:ModInfo(
    "Campaign Cartographer",
    "wpex",
    Description = "Adds multiple Cartography related features to the game, such as custom waypoint icons, GPS, auto waypoint markers, and more.",
    Side = "Universal",
    Version = "2.0.0",
    RequiredOnClient = false,
    RequiredOnServer = false,
    NetworkVersion = "1.0.0",
    Website = "https://apachetech.co.uk",
    Contributors = new[] { "ApacheTech Solutions" },
    Authors = new []{ "ApacheTech Solutions" })]

[assembly: VintageModInfo(
    ModId = "wpex",
    ModName = "Campaign Cartographer",
    RootDirectoryName = "CampaignCartographer",
    NetworkVersion = "1.0.0",
    Version = "2.0.0")]