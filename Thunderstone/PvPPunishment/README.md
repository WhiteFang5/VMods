# PvP Punishment
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
* [ChangeLog](https://github.com/WhiteFang5/VMods/blob/master/CHANGELOG.md#pvp-punishment)
