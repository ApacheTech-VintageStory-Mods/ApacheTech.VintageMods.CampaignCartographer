using ApacheTech.VintageMods.Core.Annotation.Attributes;
using Vintagestory.API.Common;

[assembly: ModDependency("game", "1.15.7")]

[assembly:ModInfo(
    "[VintageMods] - Waypoint Extensions",
    "wpex",
    Description = "Quickly, and easily add waypoint markers at your current position.",
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
    ModName = "[VintageMods] - Waypoint Extensions",
    RootDirectoryName = "WaypointExtensions",
    NetworkVersion = "1.0.0",
    Version = "2.0.0")]