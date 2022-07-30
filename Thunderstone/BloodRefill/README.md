# Blood Refill
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
* [ChangeLog](https://github.com/WhiteFang5/VMods/blob/master/CHANGELOG.md#blood-refill)
