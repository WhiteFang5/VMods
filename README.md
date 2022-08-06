# VMods
VMods is a selection of Mods for V-Rising made using a common/shared codebase that all follow the same coding principals and in-game usage.

## List of VMods
* [Blood Refill](#blood-refill)
* [Recover Empty Containers](#recover-empty-containers)
* [Resource Stash Withdrawal](#resource-stash-withdrawal)
* [PvP Leaderboard](#pvp-leaderboard)
* [PvP Punishment](#pvp-punishment)
* [Chest PvP Protection](#chest-pvp-protection)
* [Generic Chat Commands](#generic-chat-commands)

## General Mod info (Applies to most mods)
### How to manually install
* Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/)
* Install [Wetstone](https://v-rising.thunderstore.io/package/molenzwiebel/Wetstone/)
* (Locally hosted games only) Install [ServerLaunchFix](https://v-rising.thunderstore.io/package/Mythic/ServerLaunchFix/)
* Extract the Vmods._mod-name_.dll
* Move the desired mod(s) to the `[VRising (server) folder]/BepInEx/WetstonePlugins/`
* Launch the server (or game) to auto-generate the config files
* Edit the configs as you desire (found in `[VRising (server) folder]/BepInEx/config/`)
* Reload the mods using the Wetstone commands (by default F6 for client-side mods, and/or `!reload` for server-side mods)
  * If this doesn't work, or isn't enabled, restart the server/game

### Commands
Most of the mods come with a set of commands that can be used. To see the available commands, by default a player or admin can use `!help`.  
Normal players won't see the Admin-only commands listed.  
The prefix (`!`) can be changed on a per-mod basis.  
To prevent spam/abuse there's also a command cooldown for non-admins, this value can also be tweaked on a per-mod basis.  
Commands can also be disabled completely on a per-mod basis.

## Blood Refill
A server-side only mod that allows players to refill their blood pool.  
  
When feed-killing an enemy, you'll be able to regain some blood.  
The amount of blood regained is based on the level difference, blood type and blood quality of the killed enemy with V-Bloods refilling your blood pool for a much larger amount.

<details>
<summary>Configuration Options</summary>

* Enable/disable requiring feed-killing (when disabled, any kill grants some blood).
* Choose the amount of blood gained on a 'regular refill' (i.e. a refill without any level, blood type or quality punishments applied)
* A multiplier to reduce the amount of gained blood when feeding on an enemy of a different blood type. (blood dilution)
* The ability to disable different blood type refilling (i.e. a 0 multiplier for different blood types)
* Switch between having V-Blood act as diluted or pure blood, or have V-Blood completely refill your blood pool
* The options to make refilling random between 0.1L and the calculated amount (which then acts as a max refill amount)
* A global refill multiplier (applied after picking a random refill value)

</details>

## Recover Empty Containers
A server-side only mod that allows players to recover empty containers.  
  
When a player drinks a potion or brew, an empty container (glass bottle or canteen) is given back to the player (or dropped on the floor when the player's inventory is full)

## Resource Stash Withdrawal
A server & client side mod that allows players to withdraw items for a recipe directly from their stash.  
This mod also adds the stash count of items to the tooltips (given that you're in/near your base)  
  
When a player is at a crafting or other workstation, he/she can click on the recipe or an individual component of a recipe with their middle-mouse button to withdraw the missing item(s) directly from their stash.  
(The withdraw the full amount, CTRL+Middle Mouse Button can be used)  
  
Note: Players without the client-side mod can still join and play the server, but won't be able to make use of this feature.  
  
## PvP Leaderboard
A server-side only mod that keeps track of Kills, Death and the K/D ratio of players and ranks them in a leaderboard.  
  
This mod also has the option to exclude low-level (or grief-kills) from counting towards the K/D of the leaderboard.
There's also an option to prevent cheesing this restriction where the highest gear score (in the past X minutes) is used instead of the current gear score.  
Both legitimate and grief-kills can be announced server-wide.  
  
Players can see their own stats and the leaderboard itself using a command  (By default: `!pvpstats`).  
The leaderboard shows up to 5 ranks at a time and allows players to input a page number so they can "browse" the leaderboard (By default: `!pvplb [<page-number>]`).
  
<details>
<summary>Configuration Options</summary>

* Enable/disable announcing of legitimate kills
* Enable/disable announcing of grief-kills
* Set a Level Difference at which the K/D isn't counting anymore of the leaderboard.
* Enable/disable usage of the anti-cheesing system (highest gear score tracking)
* Change the amount of time the highest gear score is remembered/tracked

</details>

## PvP Punishment
A server-side only mod that punishes low-level kills.  
  
This mod also has the option prevent cheesing the low-level restriction where the highest gear score (in the past X minutes) is used instead of the current gear score.  
Both the amount of offenses before being punishes as well as the punishment itself can be tweaked.

<details>
<summary>Configuration Options</summary>

* Set a Level Difference at which an offense is being recorded
* Enable/disable usage of the anti-cheesing system (highest gear score tracking)
* Change the amount of offenses a player can make before actually being punished
* Change the offense cooldown time before the offense counter resets
* Change the duration of the punishment
* Change the following for the actual punishment:
  * % reduced Movement Speed
  * % reduced Max Health
  * % reduced Physical Resistance
  * % reduced Spell Resistance
  * amount of reduced Fire Resistance
  * amount of reduced Holy Resistance
  * amount of reduced Sun Resistance
  * amount of reduced Silver Resistance
  * % of reduced Physical Power
  * % of reduced Spell Power

</details>

## Chest PvP Protection
A server-side only mod that prevents looting of enemy player chests/workstations by players with the PvP Protection buff.  
  
Besides looting, it'll also prevent moving, sorting, swapping and merging ("compilsively count") of items within those chests/workstations.

## Generic Chat Commands
A server-side only mod that adds a fair amount of generic chat commands (mostly for Mods & Admins only though) and a player chat muting system.

The player chat mute system can be used by Moderators & Admins to mute players (Admins, Super Admins and Moderators cannot be muted).
A Super Admin can grant Moderator or Admin privileges to other players (using one of the commands).

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
