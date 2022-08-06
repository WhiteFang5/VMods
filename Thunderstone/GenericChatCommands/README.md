# Generic Chat Commands
A server-side only mod that adds a fair amount of generic chat commands (mostly for Mods & Admins only though) and a player chat muting system.

The player chat mute system can be used by Moderators & Admins to mute players (Admins, Super Admins and Moderators cannot be muted).
A Super Admin can grant Moderator or Admin provileges to other players (using one of the commands).

<details>
<summary>List of available player commands</summary>

* `!ping`: Tells you how much ping/latency you have.
* `!admin-level [<player-name>]`: Tells you the Admin Level of yourself (or the give player)

</details>

<details>
<summary>List of available Moderator commands</summary>

_Note: These commands can be made Admin-only through a config setting_
* `!mute <player-name> <number-of-minutes> [global/local]`: Mutes the given player for the given number of minutes in the given chat/channel (or all chats/channels when omitted) - commands can still be used by the muted player
* `!unmute <player-name>`: Unmutes the given player
* `!remaining-mute <player-name>`: Tells you how many more minutes the mute for the given player will last

</details>

<details>
<summary>List of available (Super)Admin commands</summary>

* [SuperAdmin] `!set-admin-level <player-name> <none/mod/admin/superadmin>`: Changes the given player's Admin Level to the given level
* `!ping [<player-name>]`: Tells you how much ping/latency you or the given player has
* `!rename [<current-player-name>] <new-player-name>`: Renames a given player (or yourself) to a new name
* `!nxtbm [server-wide]`: Tells you (or the entire server) when the next Blood Moon will appear
* `!skiptobm`: Skips time to the next Blood Moon
* `!buff [<player-name>] <prefab-GUID>`: Adds the buff defined by the prefab-GUID to yourself (or the given player)
* `!unbuff [<player-name>] <prefab-GUID>`: Removes the buff defined by the prefab-GUID to yourself (or the given player)
* `!health [<player-name>] <percentage>`: Sets the Health of yourself (or the given player) to the given percentage
* `!complete-all-achievements [<player-name>]`: Completes all achievements for yourself (or the given player)
* `!unlock-all-research [<player-name>]`: Unlocks all research for yourself (or the given player)
* `!unlock-all-v-blood [<player-name>] <all/ability/passive/shapeshift>`: Unlocks all V-Blood Abilities/Passives/Shapshifts or all three of these for yourself (or the given player)
* `!spawn-npc <npc-name/prefab-GUID> [<amount>] [<life-time>]`: Spawns the given amount of npcs based on their name or prefab-GUID, and they'll stay alive of the given amount of time (or untill killed when the life-time argument is omitted
* `!set-blood [<player-name>] <blood-type> <blood-quality> [<gain-amount>]`: Sets your (or the given player's) blood type to the specified blood-type and blood-quality, and optionally adds a given amount of blood (in Litres)
* `!blood-potion <blood-type> <blood-quality>`: Creates a Blood Potion with the given Blood Type and Blood Quality
* [SuperAdmin] `!global-... [on/off]`: A set of commands that change the settings **server-wide** (i.e. for everyone!) - Note: these might be dangerous! so use them carefully
  * `sun-damage`
  * `durability-loss`
  * `blood-drain`
  * `cooldowns`
  * `build-costs`
  * `all-progression-unlocked`
  * `play-invul`
  * `day-night-cycle`: This pauses the Day/Night cycle completely (time stops moving forward)
  * `npc-movement`
  * `building-area-restrictions`: Be careful using this one, it might cause ruins, vegitation and others objects to spawn in player's bases.
  * `all-waypoints-unlocked`
  * `aggro`
  * `death-sequence-instead-of-ragdolls`
  * `drops`: Be extra careful using this one, it'll remove all current drops on the floor AND anything, anyone drops to the floor will be deleted from the game too!
  * `tutorial-popups`
  * `building-placement-restrictions`: Be careful using this one, it might cause ruins, vegitation and others objects to spawn in player's bases.
  * `3d-height`: Be careful using this one, it might cause clipping through objects and/or the world which results in players getting stuck.
  * `tile-collision`: Be careful using this one, it might cause clipping through objects and/or the world which results in players getting stuck.
  * `dynamic-collision`: Be careful using this one, it might cause clipping through objects and/or the world which results in players getting stuck.
  * `building-replacement`: Be careful using this one, it might cause ruins, vegitation and others objects to spawn in player's bases.
  * `dynamic-clouds`
  * `hit-effects`
  * `high-castle-roofs`
  * `feed-at-any-hp`: Allows you to feed on npcs, regardless of their hp (i.e. they no longer have to be low health to feed)
  * `linn-castle-roofs`
  * `free-building-placement`
  * `building-floor-territory`
  * `building-debugging`
  * `bat-sun-damage`
  * `castle-heart-blood-ess`
  * `castle-limits`: This allows anyone in the server to place more than the server-config defined limit of castle hearts

</details>

<details>
<summary>Configuration Options</summary>

* Enable/disable server-wide announcing when a player is renamed
* Enable/disable server-wide announcing when time is being skipped to the next blood moon
* Enable/disable the option to allow server-wide announcing of the time until next blood moon
* Enable/disable server-wide announcing when any of the `global-...` options are changed
* Enable/disable server-wide announcing when a player's privileges have been changed
* Enable/disable of the entire mute system
* Enable/disable the ability for players with the Moderator privilege to mute/unmute other players
* Enable/disable server-wide announcing when a player gets muted
* Enable/disable server-wide announcing when a player gets unmuted

</details>

## How to manually install
* Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/)
* Install [Wetstone](https://v-rising.thunderstore.io/package/molenzwiebel/Wetstone/)
* (Locally hosted games only) Install [ServerLaunchFix](https://v-rising.thunderstore.io/package/Mythic/ServerLaunchFix/)
* Extract the Vmods._mod-name_.dll
* Move the desired mod(s) to the `[VRising (server) folder]/BepInEx/WetstonePlugins/`
* Launch the server (or game) to auto-generate the config files
* Edit the configs as you desire (found in `[VRising (server) folder]/BepInEx/config/`)
* Reload the mods using the Wetstone commands (by default F6 for client-side mods, and/or `!reload` for server-side mods)
  * If this doesn't work, or isn't enabled, restart the server/game

## Commands
Most of the VMods come with a set of commands that can be used. To see the available commands, by default a player or admin can use `!help`.  
Normal players won't see the Admin-only commands listed.  
The prefix (`!`) can be changed on a per-mod basis.  
To prevent spam/abuse there's also a command cooldown for non-admins, this value can also be tweaked on a per-mod basis.  
Commands can also be disabled completely on a per-mod basis.

## More Details
* [ChangeLog](https://github.com/WhiteFang5/VMods/blob/master/CHANGELOG.md#generic-chat-commands)
