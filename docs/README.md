# Campaign Cartographer

Adds multiple Cartography related features to the game, such as custom player pins, GPS, auto waypoint markers, and more.

This mod can be installed as a client-side only mod, which will allow you access to most of the features of the mod. If the mod is also installed on the server, extra features will be available to you. These features have been highlighted below.

**SERVER OWNERS:** If you install this mod on your server, it will remain as an optional mod for your clients. They can choose whether or not to have it installed on their client, and it will not cause any issues logging on, if they don't have it installed.

## Features:
  
 - ### **Manual Waypoint Addition *`(.wp)`***
 
    Quickly and easily add waypoints at your current position, via the chat window. There are over 130 pre-defined waypoints for many different block types, and areas of interest.

     - Add a waypoint at the player's current location, via a chat command. ***`(.wp)`***
     - Add a waypoint to a translocator, within five blocks of the player. ***`(.wptl)`***
     - Add a waypoint to a teleporter block, within five blocks of the player. ***`(.wptp)`***
     - Add a waypoint to a trader, within five blocks of the player. ***`(.wpt)`***
     - Add a waypoint for the block the player is currently targetting. ***`(.wps)`***
     - Toggle the Manual Waypoints settings window. ***`(.wpSettings)`***
        - Change the default fallback waypoint settings.
        - Change the waypoint settings for BlockSelection waypoints.
        - Add new pre-defined waypoint types.
        - Edit existing pre-defined waypoint types.
        - Remove existing pre-defined waypoint types.

 - ### **Automatic Waypoint Addition *`(.wpAuto)`*** 
 
    Make exploration even more rewarding, by documenting your journeys as you travel. From rock strata, to traders, to essential foodstuff; waypoints are added as you interact with the world.

     - Automatically add waypoints for Broken Translocators, as the player steps on them.
     - Automatically add waypoints for Repaired Translocators, as the player travels between them.
     - Automatically add waypoints for Teleporters, as the player travels between them.
     - Automatically add waypoints for Traders, as the player interacts with them.
     - Automatically add waypoints for mine-able Ores, when the player collects surface deposits.
     - Automatically add waypoints for Rock types, when the player collects loose stones.
     - Automatically add waypoints for Berries, Mushrooms, and Resin, when the player harvests them.
     - Toggle the Auto Waypoints settings window.  ***`(.wpAuto)`***
         - Enable / Disable automatic waypoint placements for each type.

 - ### **Centre Map *`(.cm)`***
 
    Allows you to centre the mini-map, and world map on any location you wish.

     - Re-centre the map on the current player. ***`(.cm)`***
     - Re-centre the map on any specific X, Z coordinates. ***`(.cm pos)`***
     - Re-centre the map on any specific online player. ***`(.cm player)`***
     - Re-centre the map on world spawn. ***`(.cm spawn)`***
     - Re-centre the map on a specific waypoint. ***`(.cm wp)`***
     - Re-centre the map on player's spawn point. ***`(.cm home)`*** **(Requires Server Installation)**

 - ### **Global Positioning System *`(.gps)`***
 
    Display and share your current location within the world.
  
     - Display your current XYZ coordinates. ***`(.gps)`***
     - Copy your current XYZ coordinates to clipboard. ***`(.gps copy)`***
     - Send your current XYZ coordinates as a chat message to the current chat group. ***`(.gps chat)`***
     - Whisper your current XYZ coordinates to a single player. Disabled by default. ***`(.gps to)`*** **(Requires Server Installation)**
     - **Admin:** Change permissions to whisper other members of the server. ***`(/gpsAdmin)`*** **(Requires Server Installation)**
         - Enable whispers between players. ***`(/gpsAdmin enable-whispers)`***
         - Disable whispers between players. ***`(/gpsAdmin disable-whispers)`***

 - ### **Customisable Player Map Pins *`(.playerPins)`***
 
    Change the colour and scale of player pins on the world map.
 
     - Add other players as friends, to distinguish their player pins on the map. ***`(.friend add)`***
     - Remove a player as a friend. ***`(.friend remove)`***
     - Toggle the Player Pins settings window. ***`(.playerPins)`***
         - Change settings for your own player pin.
         - Change settings for the player pins of friends on the server.
         - Change settings for the player pins of other people on the server.

 - ### **Waypoint Utilities *`(.wpUtil)`***
 
    A selection of utility commands that makes working with waypoints a lot easier.

     - All purge commands will need confirmation. ***`(.wpUtil confirm)`***
     - Purge all waypoints. ***`(.wpUtil purge-all)`***
     - Purge all waypoints within a given radius of the player. ***`(.wpUtil purge-nearby)`***
     - Purge all waypoints with a specified icon. ***`(.wpUtil purge-icon)`***
     - Purge all waypoints with a specified colour. ***`(.wpUtil purge-colour)`***
     - Purge all waypoints where the title starts with a specified string. ***`(.wpUtil purge-title)`***
     - Toggle the Waypoint Export window. ***`(.wpUtil export)`***
         - Choose which waypoints to export.
         - Export waypoints to file.
     - Toggle the Waypoint Imports window. ***`(.wpUtil import)`***
         - Import waypoints from file(s).

## Client-Side Commands:

| Command               | Description |
| ---                   | --- |
| **.wp**               | Add a waypoint at the player's current location, via a chat command. |
| **.wptl**             | Adds a waypoint to a translocator, within five blocks of the player. |
| **.wptp**             | Adds a waypoint to a teleporter block, within five blocks of the player. |
| **.wpt**              | Adds a waypoint to a trader, within five blocks of the player. |
| **.wps**              | Adds a waypoint to the block the player is currently targetting. |
| **.wpUtil**           | Various utilities for managing waypoints, en-masse. |
| **.wpSettings**       | Toggle the Manual Waypoints settings window. |
| **.wpAuto**           | Toggle the Auto Waypoints settings window. |
| **.cm**               | Centre the mini-map, and world map on a specific location. |
| **.gps**              | Display and broadcast your current location. |
| **.playerPins**       | Toggle the Player Pins settings window. |
| **.friend**           | Distinguish a player's marker pin on the map. |

## Server-Side Commands:

| Command               | Description |
| ---                   | --- |
| **/gpsAdmin**         | Change GPS settings for the server. |

## Acknowledgements:

Thank you to the following people:

 - **Doombox:** Original creator of the Customisable Player Pins feature.
 - **egocarib:** Huge inspiration for much of the Auto Waypoints feature.
 - **Melchior:** For assistance with ProtoBuf, and overwriting vanilla classes.
 - **Craluminum2413:** Translation into Russian, and Ukrainian.
 - **Aledark:** Translation into French.
 - **Novocain:** Original creator of some of the back-end reflection helpers, and ClientSideEverywhere hack.
 - **Tyron:** For refactoring some of the API to make this mod easier to make.
