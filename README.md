# Campaign Cartographer

Adds multiple Cartography related features to the game, such as custom waypoint icons, GPS, auto waypoint markers, and more.

This mod can be installed as a client-side only mod, which will allow you access to most of the features of the mod. If the mod is also installed on the server, extra features will be available to you. These features have been highlighted below.

**SERVER OWNERS:** If you install this mod on your server, it will remain as an optional mod for your clients. They can choose whether or not to have it installed on their client, and it will not cause any issues logging on, if they don't have it installed.

## Features:
  
 - ### **Manual Waypoint Addition (.wp)**
 
    Quickly and easily add waypoints at your current position, via the chat window. There are over 150 pre-defined waypoints for many different block types, and areas of interest.

     - Add a waypoint at the player's current location, via a chat command. ***`(.wp)`***
     - Add a waypoint to a translocator, within five blocks of the player. ***`(.wptl)`***
     - Add a waypoint to a teleporter block, within five blocks of the player. ***`(.wptp)`***
     - Add a waypoint to a trader, within five blocks of the player. ***`(.wpt)`***
     - Add a waypoint for the block the player is currently targetting. ***`(.wps)`***
     - Toggle the Manual Waypoints settings window. ***`(.wpSettings)`***
        - **[TODO]** Change the waypoint settings for BlockSelection waypoints.
        - **[TODO]** Change the default fallback waypoint settings.
        - **[TODO]** Add new pre-defined waypoints.
        - **[TODO]** Edit existing pre-defined waypoints.
        - **[TODO]** Remove existing pre-defined waypoints.

 - ### **Automatic Waypoint Addition** 
 
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

 - ### **Centre Map**
 
    Allows you to centre the mini-map, and world map on any location you wish.

     - Re-centre the map on the current player. ***`(.cm)`***
     - Re-centre the map on any specific X, Z coordinates. ***`(.cm pos)`***
     - Re-centre the map on any specific online player. ***`(.cm player)`***
     - Re-centre the map on world spawn. ***`(.cm spawn)`***
     - Re-centre the map on a specific waypoint. ***`(.cm wp)`***
     - Re-centre the map on player's spawn point. ***`(.cm home)`*** **(Requires Server Installation)**

 - ### **Global Positioning System (.gps)**
 
    Display and share your current location within the world.
  
     - Display your current XYZ coordinates. ***`(.gps)`***
     - Copy your current XYZ coordinates to clipboard. ***`(.gps copy)`***
     - Send your current XYZ coordinates as a chat message to the current chat group. ***`(.gps chat)`***
     - Whisper your current XYZ coordinates to a single player. ***`(.gps to)`*** **(Requires Server Installation)**
     - **[TODO]**  **Admin:** Change permissions to whisper other members of the server. ***`(/gpsAdmin)`*** **(Requires Server Installation)**

 - ### **Customisable Player Map Pins**
 
    Change the colour and scale of player pins on the world map.
 
     - Add other players as friends, to distinguish their player pins on the map. ***`(.friend add)`***
     - Remove a player as a friend. ***`(.friend remove)`***
     - Toggle the Player Pins settings window. ***`(.playerPins)`***
         - Change settings for your own player pin.
         - Change settings for the player pins of friends on the server.
         - Change settings for the player pins of other people on the server.

 - ### **Waypoint Utilities**
 
    A selection of utility commands that makes working with waypoints a lot easier.

     - Purge all waypoints. ***`(.wpUtil purge-all)`***
     - Purge all waypoints within a given radius of the player. ***`(.wpUtil purge-nearby)`***
     - Purge all waypoints with a specified icon. ***`(.wpUtil purge-icon)`***
     - Purge all waypoints with a specified colour. ***`(.wpUtil purge-colour)`***
     - Purge all waypoints where the title starts with a specified string. ***`(.wpUtil purge-title)`***
     - Toggle the Waypoint Export window. ***`(.wpUtil export)`***
         - **[TODO]**  Choose a location to save the file to.
         - **[TODO]**  Choose a file name.
         - **[TODO]**  Export waypoints to file.
     - Toggle the Waypoint Imports window. ***`(.wpUtil import)`***
         - **[TODO]**  Choose a file to import waypoints from.
         - **[TODO]**  Choose import type (purge/replace/append).
         - **[TODO]**  Import waypoints from file.

## Planned Future Features:

These are features that have not yet been implemented, but are planned for release within future updates of the Mod. Please give any feedback, and potential feature requests at the <a href="https://github.com/ApacheTechSolutions/ApacheTech.VintageMods.CampaignCartographer/issues" target="_blank">Official Issue Tracker</a>.

 - ### **"Home Location" Waypoints (/wpHome)** **(Requires Server Installation)**
 
    Each player can set one location on the server as their home. This location will be saved to the server, and a waypoint will be made at that position. This waypoint will then be shared with other clients, automatically, or by request.
 
     - **Client:** Add a "Home Location" waypoint. ***`(/wpHome add)`***
     - **Client:** Remove a "Home Location" waypoint. ***`(/wpHome remove)`***
     - **Client:** Request the "Home Location" of another player. ***`(/wpHome get)`***
     - **Client:** Request all "Home Location" from the server. ***`(/wpHome get-all)`***
     - **Client:** Enable automatic updates from the server. ***`(/wpHome enable)`***
     - **Client:** Disable automatic updates from the server. ***`(/wpHome disable)`***
     - **Admin:** Manually send updated list to client. ***`(/wpHomeAdmin send)`***
     - **Admin:** Manually broadcast updated list to all clients. ***`(/wpHomeAdmin broadcast)`***

 - ### **"Public Location" Waypoints (/wpPublic)** **(Requires Server Installation)**
 
    Public locations are places such as *World Spawn*, *New Player Areas*, *Public Forge*, *Shopping Districts*, *Portal Nexus*, or other server-wide points of interest that an Admin can add to a list of waypoints that can be broadcast to players as they log in.
 
     - **Client:** Enable automatic updates from the server. ***`(/wpPublic enable)`***
     - **Client:** Disable automatic updates from the server. ***`(/wpPublic disable)`***
     - **Client:** Request an updated list of "Public Locations". ***`(/wpPublic update)`***
     - **Admin:** Add a "Public Location" waypoint. ***`(/wpPublicAdmin add)`***
     - **Admin:** Remove a "Public Location" waypoint. ***`(/wpPublicAdmin remove)`***
     - **Admin:** Manually send updated list to client. ***`(/wpPublicAdmin send)`***
     - **Admin:** Manually broadcast updated list to all clients. ***`(/wpPublicAdmin broadcast)`***

 - ### **New Waypoint Icons**

     - Adds many new waypoint icons that can be used to mark locations on the map.

## Screenshots

**Automatic Waypoints GUI**

![Automatic Waypoints GUI Screenshot](https://i.imgur.com/YXfbKEA.png)

**Own Player Pins GUI**

![Own Player Pins GUI Screenshot](https://i.imgur.com/uVmXnSx.png)

**Friends Player Pins GUI**

![Friends Player Pins Player Pins GUI Screenshot](https://i.imgur.com/3FTOTZ0.png)

**Other Player Pins GUI**

![Other Player Pins GUI Screenshot](https://i.imgur.com/heSBu9o.png)

## Client-Side Commands:

| Command               | Description |
| ---                   | --- |
| **.wp**               | Add a waypoint at the player's current location, via a chat command. |
| **.wptl**             | Adds a waypoint to a translocator, within five blocks of the player. |
| **.wptp**             | Adds a waypoint to a teleporter block, within five blocks of the player. |
| **.wpt**              | Adds a waypoint to a trader, within five blocks of the player. |
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
| **/gpsAdmin**         | Toggle the GPS Admin settings window. |
| **/wpHome**           | Synchronise player home location waypoints with the server. |
| **/wpHomeAdmin**      | Toggle the Home Locations Admin settings window. |
| **/wpPublic**         | Synchronise server-wide public location waypoints with the server. |
| **/wpPublicAdmin**    | Toggle the Public Locations Admin settings window. |

## Acknowledgements:

Thank you to the following people:

 - **Doombox:** Original creator of the Customisable Player Pins feature.
 - **egocarib:** Huge inspiration for much of the Auto Waypoints feature.
 - **Melchior:** For assistance with ProtoBuf, and overwriting vanilla classes.
 - **Craluminum2413:** For assistance with JSON patching.
 - **Novocain:** Original creator of some of the back-end reflection helpers.
 - **Tyron:** For refactoring some of the API to make this mod easier to make.
