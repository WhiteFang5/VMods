# VMods
A selection of Mods for V-Rising

## General Mod info (Applies to most mods)
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
