# PvE Leaderboard
A server-side only mod that keeps track of PvE Kills, Death, K/D ratio and Lvl Kills of players and ranks them in multiple leaderboards.  
  
There are leaderboards for each enemy faction, blood type and an overall leaderboard.  
Additionally when killing an enemy, the level of that enemy is added to your "Lvl Kills" score.  
  
This mod also has the option to exclude low-level kills from counting towards the K/D of the leaderboard(s).  
There's also an option to prevent cheesing this restriction where the highest gear score (in the past X minutes) is used instead of the current gear score.  
  
Players can see their own stats and the leaderboard itself using a command (By default: `!pvestats [<faction>/<blood-type>/all/overall]`).  
The leaderboard shows up to 5 ranks at a time and allows players to input a page number so they can "browse" the leaderboard (By default: `!pvelb [<faction/blood-type>] [<pagenum>]`).
The Lvl Kills leaderboards can also be browsed using a command (By default: `!pvelklb [<faction/blood-type>] [<pagenum>]`).
  
<details>
<summary>Configuration Options</summary>

* Set a Level Difference at which the K/D isn't counting anymore of the leaderboard.
* Enable/disable usage of the anti-cheesing system (highest gear score tracking)
* Change the amount of time the highest gear score is remembered/tracked

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
* [ChangeLog](https://github.com/WhiteFang5/VMods/blob/master/CHANGELOG.md#pve-leaderboard)
