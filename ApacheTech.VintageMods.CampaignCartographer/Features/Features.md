# FEATURES:

 - Global Positioning System (.gps)
  
     - Display your current XYZ coordinates.
     - Copy your current XYZ coordinates to clipboard.
     - Send your current XYZ coordinates as a chat message to the current chat group.
     - Whisper your current XYZ coordinates to a single player (server settings permitting).
     - Server: Enable/Disable permissions to whisper to other members of the server.
  
 - Manual Waypoint Addition (.wp)

     - Contains a GUI that can be used to control the settings for the feature.
     - Add a waypoint at the player's current location, via a chat command.

 - Automatic Waypoints

     - Contains a GUI that can be used to control the settings for the feature.
     - Enable / Disable all automatic waypoint placements.
     - Automatically add waypoints for Broken Translocators, as the steps on them.
     - Automatically add waypoints for Repaired Translocators, as the player travels between them.
     - Automatically add waypoints for Teleporters, as the player travels between them.
     - Automatically add waypoints for Traders, as the player interacts with them.
     - Automatically add waypoints for Meteors, when the player shift-right-clicks on Meteoric Iron Blocks.
     - Server: Send Teleporter information to clients, when creating Teleporter waypoints.

 - New Waypoint Icons

     - Adds many new waypoint icons that can be used to mark locations on the map.

 - Uninstall (.wpUninstall | /wpUninstall)

     - Removes all files and folders related to the mod, from the data directory.
     - Restores all custom waypoint icons to the deafult `dot` icon.
     - Server: Exports all home location waypoints to file.
     - Server: Exports saved waypoints database to file.
     - Client: Exports waypoints database to file.

 - Centre Map (.cm)

     - Re-centre the map on any specific X, Z coordinates.
     - Re-centre the map on any specific logged-in player.
     - Re-centre the map on the current player.
     - Re-centre the map on world spawn.

 - Waypoint Management

     - Contains a GUI that can be used to control the settings for the feature.
     - Export all waypoints.
     - Import waypoints from file.
     - Remove all waypoints within a given radius of the player, after confirmation.
     - Remove all waypoints with a specified icon, after confirmation.
     - Remove all waypoints with a specified colour, after confirmation.
     - Remove all waypoints where the title starts with a specified string, after confirmation.
     
 - Waypoint Home Locations (.wpHome)
 
     - Enable / Disable automatic home waypoint location updates from the server.
     - Set a home waypoint location on the server.
     - Get a home waypoint location from the server, for a player that has previously registered their home location.
     - Server: Broadcast an updated list of home waypoint locations to all clients.
     - Server: Broadcast a set of default waypoints to all clients, as they log in.

# CLIENT-SIDE COMMANDS:

 - .wp
 - .wpConfig
 - .wpAuto
 - .wpUtil
 - .cm
 - .gps [broadcast|clipboard|whisper]
 - .wpUninstall

# SERVER-SIDE COMMANDS:

 - /wpHome [set|update|remove|playerName]
 - /wpConfig
 - /wpBroadcast
 - /wpUninstall