using Vintagestory.API.Common;

// ReSharper disable StringLiteralTypo

[assembly: ModDependency("game", "1.17.1")]
[assembly: ModDependency("survival", "1.17.1")]

[assembly:ModInfo(
    "Campaign Cartographer",
    "campaigncartographer",
    Description = "Adds multiple map related features to the game, custom marker pins, GPS, automatic waypoint markers, and more.",
    Side = "Universal",
    Version = "3.0.0",
    RequiredOnClient = false,
    RequiredOnServer = false,
    NetworkVersion = "1.0.0",
    Website = "https://apachetech.co.uk",
    Contributors = new[] { "Apache", "Doombox", "Melchior", "Novocain", "egocarib", "Craluminum2413", "Aledark", "Th3Dilly" },
    Authors = new []{ "ApacheTech Solutions" })]