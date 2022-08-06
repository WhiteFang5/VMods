using ProjectM;
using ProjectM.Network;
using ProjectM.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using VMods.Shared;
using Wetstone.API;
using AdminLevel = VMods.Shared.CommandAttribute.AdminLevel;

namespace VMods.GenericChatCommands
{
	public static class GenericChatCommandsSystem
	{
		#region Consts

		private static readonly Dictionary<DebugSettingType, (string commandName, bool reverseOnOff)> DebugSettingTypeToCommandNameMapping = new()
		{
			[DebugSettingType.SunDamageDisabled] = ("sun-damage", true),
			[DebugSettingType.DurabilityDisabled] = ("durability-loss", true),
			[DebugSettingType.BloodDrainDisabled] = ("blood-drain", true),
			[DebugSettingType.CooldownsDisabled] = ("cooldowns", true),
			[DebugSettingType.BuildCostsDisabled] = ("build-costs", true),
			[DebugSettingType.AllProgressionUnlocked] = ("all-progression-unlocked", false),
			[DebugSettingType.PlayerInvulernabilityEnabled] = ("play-invul", false),
			[DebugSettingType.DayNightCycleDisabled] = ("day-night-cycle", true),
			[DebugSettingType.NPCsDisabled] = ("npc-movement", true),
			[DebugSettingType.BuildingAreaRestrictionDisabled] = ("building-area-restrictions", true),
			[DebugSettingType.AllWaypointsUnlocked] = ("all-waypoints-unlocked", false),
			[DebugSettingType.AggroDisabled] = ("aggro", true),
			[DebugSettingType.UseDeathSequencesInsteadOfRagdolls] = ("death-sequence-instead-of-ragdolls", false),
			[DebugSettingType.DropsDisabled] = ("drops", true),
			[DebugSettingType.TutorialPopupsDisabled] = ("tutorial-popups", true),
			[DebugSettingType.BuildingPlacementRestrictionsDisabled] = ("building-placement-restrictions", true),
			[DebugSettingType.Use3DHeight] = ("3d-height", false),
			[DebugSettingType.TileCollisionDisabled] = ("tile-collision", true),
			[DebugSettingType.DynamicCollisionDisabled] = ("dynamic-collision", true),
			[DebugSettingType.BuildingReplacementDisabled] = ("building-replacement", true),
			[DebugSettingType.DynamicCloudsDisabled] = ("dynamic-clouds", true),
			[DebugSettingType.HitEffectsDisabled] = ("hit-effects", true),
			[DebugSettingType.HighCastleRoofsEnabled] = ("high-castle-roofs", false),
			[DebugSettingType.FeedWoundedRequirementDisabled] = ("feed-at-any-hp", true),
			[DebugSettingType.LinnCastleRoofsEnabled] = ("linn-castle-roofs", false),
			[DebugSettingType.FreeBuildingPlacementEnabled] = ("free-building-placement", false),
			[DebugSettingType.BuildingFloorTerritoryDisabled] = ("building-floor-territory", true),
			[DebugSettingType.BuildingEnableDebugging] = ("building-debugging", false),
			[DebugSettingType.UseSunblockerChecksForFly] = ("bat-sun-damage", true),
			[DebugSettingType.CastleHeartBloodEssenceDisabled] = ("castle-heart-blood-ess", true),
			[DebugSettingType.CastleLimitsDisabled] = ("castle-limits", true),
		};

		#endregion

		#region Variables

		private static readonly List<string> _registeredCommands = new();

		#endregion

		#region Public Methods

		public static void Initialize()
		{
			var debugSettingTypes = (DebugSettingType[])Enum.GetValues(typeof(DebugSettingType));
			foreach(var debugSettingType in debugSettingTypes)
			{
				if(!DebugSettingTypeToCommandNameMapping.TryGetValue(debugSettingType, out var commandData))
				{
					Utils.Logger.LogWarning($"Missing Command Name for '{debugSettingType}'.");
					continue;
				}

				var id = Guid.NewGuid().ToString();
				var name = debugSettingType.ToString();
				var commandName = $"global-{commandData.commandName}";
				var commandAttribute = new CommandAttribute(commandName, $"{commandName} [on/off]", $"Turns the '{commandData.commandName}' setting on/off <color=#ff0000>Server-Wide</color>", AdminLevel.SuperAdmin);
				CommandSystem.RegisterCommand(id, command => OnDebugSettingCommand(command, debugSettingType), commandAttribute);
				_registeredCommands.Add(id);
			}
		}

		public static void Deinitialize()
		{
			_registeredCommands.ForEach(x => CommandSystem.UnregisterCommand(x));
		}

		#endregion

		#region Private Methods

		private static void OnDebugSettingCommand(Command command, DebugSettingType debugSettingType)
		{
			if(command.Args.Length >= 1)
			{
				var server = VWorld.Server;
				var entityManager = server.EntityManager;
				var toggleValue = command.Args[0];
				var commandData = DebugSettingTypeToCommandNameMapping[debugSettingType];

				switch(toggleValue)
				{
					case "on":
					case "true":
					case "1":
						SetDebugSetting(true);
						break;

					case "off":
					case "false":
					case "0":
						SetDebugSetting(false);
						break;

					default:
						command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid toggle options. Options are: on, off</color>");
						break;
				}

				// Nested Method(s)
				void SetDebugSetting(bool enabled)
				{
					var enabledName = enabled ? "on" : "off";
					Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has turned the '{debugSettingType}' setting {enabledName}.");

					SetDebugSettingEvent setDebugSettingEvent = new()
					{
						SettingType = debugSettingType,
						Value = commandData.reverseOnOff ? !enabled : enabled,
					};
					server.GetExistingSystem<DebugEventsSystem>().SetDebugSetting(command.VModCharacter.User.Index, ref setDebugSettingEvent);

					if(GenericChatCommandsConfig.GenericChatCommandsAnnounceGlobalCommandChanges.Value)
					{
						ServerChatUtils.SendSystemMessageToAllClients(entityManager, $"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has been turned {commandData.commandName} <color=#{(enabled ? "00ff00" : "ff0000")}>{enabledName}</color>");
					}
					else
					{
						command.VModCharacter.SendSystemMessage($"<color=#ffffff>{commandData.commandName}</color> has been turned <color=#{(enabled ? "00ff00" : "ff0000")}>{enabledName}</color>");
					}
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}

		[Command("set-admin-level,set-admin-lvl,set-adminlvl,setadminlevel,setadminlvl", "set-admin-level <player-name> <none/mod/admin/superadmin>", "Changes the given player's Admin Level to the given level", AdminLevel.SuperAdmin)]
		private static void OnSetAdminLevelCommand(Command command)
		{
			if(command.Args.Length >= 2)
			{
				var server = VWorld.Server;
				var entityManager = server.EntityManager;
				(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);
				
				if(vmodCharacter.HasValue)
				{
					AdminLevel? newAdminLevel = null;
					switch(command.Args[1].ToLowerInvariant())
					{
						case "none":
							newAdminLevel = AdminLevel.None;
							break;

						case "mod":
						case "moderator":
							newAdminLevel = AdminLevel.Moderator;
							break;

						case "admin":
							newAdminLevel = AdminLevel.Admin;
							break;

						case "superadmin":
						case "super-admin":
							newAdminLevel = AdminLevel.SuperAdmin;
							break;

						default:
							command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid admin-level. Options are: none, mod, admin, superadmin</color>");
							break;
					}

					if(newAdminLevel.HasValue)
					{
						Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has changed the Admin Level of {searchUsername} from {vmodCharacter.Value.AdminLevel} to {newAdminLevel.Value}");

						var entity = entityManager.CreateEntity(
							ComponentType.ReadOnly<FromCharacter>(),
							ComponentType.ReadOnly<SetUserAdminLevelAdminEvent>()
						);
						entityManager.SetComponentData(entity, command.VModCharacter.FromCharacter);
						entityManager.SetComponentData(entity, new SetUserAdminLevelAdminEvent()
						{
							UserNetworkId = entityManager.GetComponentData<NetworkId>(vmodCharacter.Value.FromCharacter.User),
							AdminLevel = newAdminLevel.Value.ToAdminLevel(),
						});

						if(GenericChatCommandsConfig.GenericChatCommandsAnnounceAdminLevelChanges.Value)
						{
							string message = newAdminLevel.Value switch
							{
								AdminLevel.None => $"Vampire <color=#ffffff>{searchUsername}</color> had his/her <color=#00ff00>{vmodCharacter.Value.AdminLevel}</color> privileges <color=#ff0000>revoked</color>",
								_ => $"Vampire <color=#ffffff>{searchUsername}</color> has been granted <color=#00ff00>{newAdminLevel.Value}</color> privileges!",
							};
							ServerChatUtils.SendSystemMessageToAllClients(entityManager, message);
						}
						else
						{
							if(vmodCharacter.Value != command.VModCharacter)
							{
								string message = newAdminLevel.Value switch
								{
									AdminLevel.None => $"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has <color=#ff0000>revoked</color> your <color=#00ff00>{vmodCharacter.Value.AdminLevel}</color> privileges",
									_ => $"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has granted you <color=#00ff00>{newAdminLevel.Value}</color> privileges!",
								};
								vmodCharacter.Value.SendSystemMessage(message);
							}
							// No need to send it to the command sender because the game itself already tells you what happend.
						}
					}
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}

		[Command("admin-level,get-admin-level,get-admin-lvl,get-adminlvl,getadminevel,adminlevel,adminlvl", "admin-level [<player-name>]", "Tells you the Admin Level of yourself (or the give player)")]
		private static void OnGetAdminLevelCommand(Command command)
		{
			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);
				
			if(vmodCharacter.HasValue)
			{
				var adminLevel = vmodCharacter.Value.AdminLevel.ToAdminLevel();
				string message = adminLevel switch
				{
					AdminLevel.None => $"Vampire <color=#ffffff>{searchUsername}</color> has no special privileges",
					_ => $"Vampire <color=#ffffff>{searchUsername}</color> has <color=#00ff00>{adminLevel}</color> privileges",
				};
				command.VModCharacter.SendSystemMessage(message);
			}
			command.Use();
		}

		[Command("rename", "rename [<current-player-name>] <new-player-name>", "Renames a given player (or yourself) to a new name", AdminLevel.Admin)]
		private static void OnRenameCommand(Command command)
		{
			if(command.Args.Length >= 1)
			{
				var server = VWorld.Server;
				var entityManager = server.EntityManager;
				(var searchUsername, var vmodCharacter) = command.FindVModCharacter(argIdx: command.Args.Length - 2, entityManager: entityManager);
				var newUsername = command.Args.Length switch
				{
					1 => command.Args[0],
					_ => command.Args[1],
				};

				if(vmodCharacter.HasValue)
				{
					Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has renamed {searchUsername} to {newUsername}");

					server.GetExistingSystem<DebugEventsSystem>().RenameUser(vmodCharacter.Value.FromCharacter, new RenameUserDebugEvent()
					{
						NewName = newUsername,
						Target = entityManager.GetComponentData<NetworkId>(vmodCharacter.Value.FromCharacter.User),
					});

					string message = $"Vampire <color=#ffffff>{searchUsername}</color> is now known as <color=#00ff00>{newUsername}</color>!";
					if(GenericChatCommandsConfig.GenericChatCommandsAnnounceRename.Value)
					{
						ServerChatUtils.SendSystemMessageToAllClients(entityManager, message);
					}
					else
					{
						if(vmodCharacter.Value != command.VModCharacter)
						{
							vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has renamed you to <color=#00ff00>{newUsername}</color>");
						}
						command.VModCharacter.SendSystemMessage(message);
					}
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}

		[Command("nxtbm,nextbm,nextbloodmoon", "nxtbm [server-wide]", "Tells you (or the entire server) when the next Blood Moon will appear", AdminLevel.Admin)]
		private static void OnNextBloodMoonWhenCommand(Command command)
		{
			bool serverWide = command.Args.Length >= 1 && command.Args[0] == "server-wide";

			var server = VWorld.Server;
			var dayNightCycle = server.GetExistingSystem<ServerScriptMapper>()._DayNightCycleAccessor.GetSingleton();
			string message;
			if(dayNightCycle.IsBloodMoonDay())
			{
				message = $"<color=#ffffff>Today</color> is <color=#ff0000>Blood Moon</color> day!";
			}
			else
			{
				var totalDays = dayNightCycle.TotalDays();
				var nextBloodMoonDay = dayNightCycle.NextBloodMoonDay;
				var remainingDays = nextBloodMoonDay - totalDays;
				if(remainingDays >= 1)
				{
					message = $"Next <color=#ff0000>Blood Moon</color> will be in <color=#ffffff>{nextBloodMoonDay - totalDays}</color> days";
				}
				else if(remainingDays == 0)
				{
					message = $"Next <color=#ff0000>Blood Moon</color> will be <color=#ffffff>later today</color>";
				}
				else
				{
					message = $"The <color=#ff0000>Blood Moon</color> is still thinking about when to appear again...";
				}
			}
			if(serverWide && GenericChatCommandsConfig.GenericChatCommandsAllowNextBloodMoonServerWideAnnouncing.Value)
			{
				Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has announced the time until the next Blood Moon server-wide.");
				ServerChatUtils.SendSystemMessageToAllClients(server.EntityManager, message);
			}
			else
			{
				command.VModCharacter.SendSystemMessage(message);
			}
			command.Use();
		}

		[Command("skiptobm,skiptobloodmoon", "skiptobm", "Skips time to the next Blood Moon", AdminLevel.Admin)]
		private static void OnSkipToBloodMoonCommand(Command command)
		{
			var server = VWorld.Server;
			server.GetExistingSystem<DebugEventsSystem>().JumpToNextBloodMoon();

			Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has skipped time to the next Blood Moon.");

			var message = "Time has been skipped to the next <color=#ff0000>Blood Moon</color>!";
			if(GenericChatCommandsConfig.GenericChatCommandsAnnounceBloodMoonSkip.Value)
			{
				ServerChatUtils.SendSystemMessageToAllClients(server.EntityManager, message);
			}
			else
			{
				command.VModCharacter.SendSystemMessage(message);
			}
			command.Use();
		}

		[Command("buff", "buff [<player-name>] <buff-id>", "Adds the buff defined by the buff-id to yourself (or the given player)", AdminLevel.Admin)]
		private static void OnBuffCommand(Command command)
		{
			if(command.Args.Length >= 1)
			{
				var server = VWorld.Server;
				var entityManager = server.EntityManager;
				(var searchUsername, var vmodCharacter) = command.FindVModCharacter(argIdx: command.Args.Length - 2, entityManager: entityManager);
				if(vmodCharacter.HasValue)
				{
					if(int.TryParse(command.Args.Length switch
					{
						1 => command.Args[0],
						_ => command.Args[1],
					}, out int guidHash))
					{
						var buffGUID = new PrefabGUID(guidHash);
						var prefabNameLookupMap = server.GetExistingSystem<PrefabCollectionSystem>().PrefabNameLookupMap;
						if(prefabNameLookupMap.ContainsKey(buffGUID))
						{
							var buffName = prefabNameLookupMap[buffGUID];

							Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has applied the {buffName} ({guidHash}) to {searchUsername}");

							vmodCharacter.Value.ApplyBuff(buffGUID);

							if(vmodCharacter.Value != command.VModCharacter)
							{
								vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.Character}</color> has buffed you with <color=#00ff00>{buffName}</color>");
							}
							command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> has been buffed with <color=#00ff00>{buffName}</color> ({guidHash})");
						}
						else
						{
							command.VModCharacter.SendSystemMessage($"<color=#ff0000>Buff ID <color=#ffffff>{guidHash}</color> does not exist.</color>");
						}
					}
					else
					{
						CommandSystem.SendInvalidCommandMessage(command, true);
					}
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}

		[Command("unbuff", "unbuff [<player-name>] <prefab-GUID>", "Remove the buff defined by the prefab-GUID to yourself (or the given player)", AdminLevel.Admin)]
		private static void OnUnBuffCommand(Command command)
		{
			if(command.Args.Length >= 1)
			{
				var server = VWorld.Server;
				var entityManager = server.EntityManager;
				(var searchUsername, var vmodCharacter) = command.FindVModCharacter(argIdx: command.Args.Length - 2, entityManager: entityManager);
				if(vmodCharacter.HasValue)
				{
					if(int.TryParse(command.Args.Length switch
					{
						1 => command.Args[0],
						_ => command.Args[1],
					}, out int guidHash))
					{
						var buffGUID = new PrefabGUID(guidHash);
						var prefabNameLookupMap = server.GetExistingSystem<PrefabCollectionSystem>().PrefabNameLookupMap;
						var buffName = prefabNameLookupMap[buffGUID];

						Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has removed the {buffName} ({guidHash}) to {searchUsername}");

						vmodCharacter.Value.RemoveBuff(buffGUID);

						if(vmodCharacter.Value != command.VModCharacter)
						{
							vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has removed your <color=#00ff00>{buffName}</color> buff");
						}
						command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> has been un-buffed from <color=#00ff00>{buffName}</color> ({guidHash})");
					}
					else
					{
						CommandSystem.SendInvalidCommandMessage(command, true);
					}
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}

		[Command("ping", "ping [<player-name>]", "Tells you how much ping/latency you (<color=#ff0000>[ADMIN]</color> or the given player) has")]
		private static void OnPingCommand(Command command)
		{
			var entityManager = VWorld.Server.EntityManager;
			(var searchUsername, var vmodCharacter) = command.FindVModCharacter(argIdx: command.VModCharacter.IsAdmin ? command.Args.Length - 1 : -1, entityManager: entityManager);
			if(vmodCharacter.HasValue)
			{
				var latency = entityManager.GetComponentData<Latency>(vmodCharacter.Value.FromCharacter.User);
				if(searchUsername == command.VModCharacter.CharacterName)
				{
					command.VModCharacter.SendSystemMessage($"You have <color=#ffffff>{latency.Value}ms</color> ping");
				}
				else
				{
					command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> has <color=#ffffff>{latency.Value}ms</color> ping");
				}
			}
			command.Use();
		}

		[Command("health,hp", "health [<player-name>] <percentage>", "Sets the Health of yourself (or the given player) to the given percentage", AdminLevel.Admin)]
		private static void OnHealthCommand(Command command)
		{
			if(command.Args.Length >= 1)
			{
				var server = VWorld.Server;
				var entityManager = server.EntityManager;
				(var searchUsername, var vmodCharacter) = command.FindVModCharacter(argIdx: command.Args.Length - 2, entityManager: entityManager);
				if(vmodCharacter.HasValue)
				{
					if(int.TryParse((command.Args.Length switch
					{
						1 => command.Args[0],
						_ => command.Args[1],
					}).Replace("%", string.Empty), out int percentage) && percentage >= 0 && percentage <= 100)
					{
						Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has set the health of {searchUsername} to {percentage}%");

						var healthData = entityManager.GetComponentData<Health>(vmodCharacter.Value.FromCharacter.Character);

						var changeHealthEvent = new ChangeHealthDebugEvent()
						{
							Amount = (int)((healthData.MaxHealth / 100f * percentage) - healthData.Value),
						};
						server.GetExistingSystem<DebugEventsSystem>().ChangeHealthEvent(command.VModCharacter.User.Index, ref changeHealthEvent);

						if(vmodCharacter.Value != command.VModCharacter)
						{
							vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has changed your health to <color=#00ff00>{percentage}%</color>");
						}
						command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> has had his health changed to <color=#00ff00>{percentage}%</color>");
					}
					else
					{
						CommandSystem.SendInvalidCommandMessage(command, true);
					}
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}

		[Command("complete-all-achievements,complete-achievements,completeallachievements,completeachievements", "complete-all-achievements [<player-name>]", "Completes all achievements for yourself (or the given player)", AdminLevel.Admin)]
		private static void OnCompletedAllAchievementsCommand(Command command)
		{
			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);
			if(vmodCharacter.HasValue)
			{
				Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has set all achievements completed for {searchUsername}");

				server.GetExistingSystem<DebugEventsSystem>().CompleteAllAchievements(command.VModCharacter.FromCharacter);

				if(vmodCharacter.Value != command.VModCharacter)
				{
					vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has completed all achievements for you");
				}
				command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> now has all achievements completed");
			}
			command.Use();
		}

		[Command("unlock-all-research,unlock-research,unlockallresearch,unlockresearch", "unlock-all-research [<player-name>]", "Unlocks all research for yourself (or the given player)", AdminLevel.Admin)]
		private static void OnUnlockAllResearchCommand(Command command)
		{
			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);
			if(vmodCharacter.HasValue)
			{
				Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has unlocked all research for {searchUsername}");

				server.GetExistingSystem<DebugEventsSystem>().UnlockAllResearch(command.VModCharacter.FromCharacter);

				if(vmodCharacter.Value != command.VModCharacter)
				{
					vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has unlocked all research for you");
				}
				command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> now has all research unlocked");
			}
			command.Use();
		}

		[Command("unlock-all-v-blood,unlock-all-vblood,unlock-v-blood,unlock-vblood,unlockallvblood,unlockvblood", "unlock-all-v-blood [<player-name>] <all/ability/passive/shapeshift>", "Unlocks all V-Blood Abilities/Passives/Shapshifts or all three of these for yourself (or the given player)", AdminLevel.Admin)]
		private static void OnUnlockAllVBloodCommand(Command command)
		{
			if(command.Args.Length >= 1)
			{
				var server = VWorld.Server;
				var entityManager = server.EntityManager;
				(var searchUsername, var vmodCharacter) = command.FindVModCharacter(argIdx: command.Args.Length - 2, entityManager: entityManager);
				if(vmodCharacter.HasValue)
				{
					string unlockOption = command.Args.Length switch
					{
						1 => command.Args[0],
						_ => command.Args[1],
					};
					switch(unlockOption)
					{
						case "all":
							server.GetExistingSystem<DebugEventsSystem>().UnlockAllVBloods(command.VModCharacter.FromCharacter);
							unlockOption = "abilities, passives & shapeshifts";
							break;

						case "ability":
						case "abilities":
							server.GetExistingSystem<DebugEventsSystem>().UnlockVBloodFeatures(command.VModCharacter.FromCharacter, DebugEventsSystem.VBloodFeatureType.Ability);
							unlockOption = "abilities";
							break;

						case "passive":
						case "passives":
							server.GetExistingSystem<DebugEventsSystem>().UnlockVBloodFeatures(command.VModCharacter.FromCharacter, DebugEventsSystem.VBloodFeatureType.Passive);
							unlockOption = "passives";
							break;

						case "shapeshift":
						case "shapeshifts":
							server.GetExistingSystem<DebugEventsSystem>().UnlockVBloodFeatures(command.VModCharacter.FromCharacter, DebugEventsSystem.VBloodFeatureType.Shapeshift);
							unlockOption = "shapeshifts";
							break;

						default:
							command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid unlock options. Options are: all, ability, passive, shapeshift</color>");
							break;
					}


					Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has unlocked all V-Blood {unlockOption} for {searchUsername}");

					if(vmodCharacter.Value != command.VModCharacter)
					{
						vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has unlocked all V-Blood {unlockOption} for you");
					}
					command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> now has all V-Blood {unlockOption} unlocked");
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}

		[Command("spawn-npc,spawnnpc,spn", "spawnnpc <npc-name/prefab-GUID> [<amount>] [<life-time>]", "Spawns the given amount of npcs based on their name or prefab-GUID, and they'll stay alive of the given amount of time (or untill killed when the life-time argument is omitted)", AdminLevel.Admin)]
		private static void OnSpawnNpcCommand(Command command)
		{
			if(command.Args.Length >= 1)
			{
				var server = VWorld.Server;
				var entityManager = server.EntityManager;
				var prefabCollection = server.GetExistingSystem<PrefabCollectionSystem>();

				string npcNameOrGUID = command.Args[0];
				string npcName = string.Empty;
				PrefabGUID npcGUID = PrefabGUID.Empty;
				if(int.TryParse(npcNameOrGUID, out var prefabGUIDHash))
				{
					var guid = new PrefabGUID(prefabGUIDHash);
					if(prefabCollection.PrefabGuidLookupMap.ContainsKey(guid))
					{
						npcName = prefabCollection.PrefabGuidLookupMap[guid].ToString();
						npcGUID = guid;
					}
				}
				else
				{
					FixedString128 prefabName = new(npcNameOrGUID);
					if(prefabCollection.PrefabNameToPrefabGuidLookupMap.ContainsKey(prefabName))
					{
						npcGUID = prefabCollection.PrefabNameToPrefabGuidLookupMap[prefabName];
						npcName = npcNameOrGUID;
					}
				}

				if(npcGUID != PrefabGUID.Empty)
				{
					if(npcName.StartsWith("CHAR_"))
					{
						if(int.TryParse(command.Args.Length switch
						{
							1 => "1",
							_ => command.Args[1],
						}, out var amount) && amount >= 0)
						{
							if(int.TryParse(command.Args.Length switch
							{
								1 => "-1",
								2 => "-1",
								_ => command.Args[2]
							}, out int lifeTime))
							{
								lifeTime = Math.Max(lifeTime, -1);

								Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has spawned {amount} {npcName} (Life-time: {lifeTime}s)");

								var translation = entityManager.GetComponentData<Translation>(command.VModCharacter.FromCharacter.User);

								server.GetExistingSystem<UnitSpawnerUpdateSystem>().SpawnUnit(Entity.Null, npcGUID, translation.Value, amount, 1f, 2f, lifeTime);

								command.VModCharacter.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has spawned <color=#00ff00>{amount}</color> <color=#ffffff>{npcName}</color>{(lifeTime == -1 ? "" : $" (Life-time: {lifeTime}s)")}");
							}
							else
							{
								command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid life-time for the NPC(s)</color>");
							}
						}
						else
						{
							command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid amount of NPC(s) to spawn</color>");
						}
					}
					else
					{
						command.VModCharacter.SendSystemMessage($"<color=#ff0000>'{npcNameOrGUID}' is a valid name/GUID, but it isn't an NPC (and thus cannot be spawned)</color>");
					}
				}
				else
				{
					command.VModCharacter.SendSystemMessage($"<color=#ff0000>Unknown NPC name or GUID '{npcNameOrGUID}'</color>");
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}

		[Command("set-blood,setblood", "set-blood [<player-name>] <blood-type> <blood-quality> [<gain-amount>]", "Sets your (or the given player's) blood type to the specified blood-type and blood-quality, and optionally adds a given amount of blood (in Litres)", AdminLevel.Admin)]
		private static void OnSetBloodCommand(Command command)
		{
			var argCount = command.Args.Length;
			if(argCount >= 2)
			{
				bool isFirstArgBloodType = Enum.TryParse(command.Args[0].ToLowerInvariant(), true, out BloodType _);

				var entityManager = VWorld.Server.EntityManager;
				int argIdx = argCount switch
				{
					3 when isFirstArgBloodType => -1,
					3 => 0,
					4 => 0,
					_ => -1,
				};
				(var searchUsername, var vmodCharacter) = command.FindVModCharacter(argIdx, entityManager: entityManager);

				if(vmodCharacter.HasValue)
				{
					argIdx++;
					var searchBloodType = command.Args[argIdx];
					var validBloodTypes = BloodTypeExtensions.BloodTypeToPrefabGUIDMapping.Keys.ToList();
					if(Enum.TryParse(searchBloodType.ToLowerInvariant(), true, out BloodType bloodType) && validBloodTypes.Contains(bloodType))
					{
						argIdx++;
						var searchBloodQuality = command.Args[argIdx];
						if(int.TryParse(searchBloodQuality.Replace("%", string.Empty), out var bloodQuality) && bloodQuality >= 1 && bloodQuality <= 100)
						{
							float? addBloodAmount = null;
							if((argCount >= 3 && isFirstArgBloodType) || argCount >= 4)
							{
								argIdx++;
								var searchLitres = command.Args[argIdx];
								if(float.TryParse(searchLitres.Replace("L", string.Empty), out float parsedAddBloodAmount) && parsedAddBloodAmount >= -10f && parsedAddBloodAmount <= 10f)
								{
									addBloodAmount = parsedAddBloodAmount;
								}
								else
								{
									command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid gain-amount '{searchBloodQuality}'. Should be between -10 and 10</color>");
								}
							}
							else
							{
								addBloodAmount = 10f;
							}

							if(addBloodAmount.HasValue)
							{
								Utils.Logger.LogMessage($"{command.VModCharacter.Character} has changed blood type of {searchUsername} to {bloodQuality}% {searchBloodType} and added {addBloodAmount.Value}L");

								bloodType.ApplyToPlayer(vmodCharacter.Value.User, bloodQuality, (int)(addBloodAmount.Value * 10f));
								if(vmodCharacter.Value != command.VModCharacter)
								{
									vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.Character}</color> has changed your blood type to <color=#ff0000>{bloodQuality}%</color> <color=#ffffff>{searchBloodType}</color> and added <color=#ff0000>{addBloodAmount.Value}L</color>");
								}
								command.VModCharacter.SendSystemMessage($"Changed blood type of <color=#ffffff>{searchUsername}</color> to <color=#ff0000>{bloodQuality}%</color> <color=#ffffff>{searchBloodType}</color> and added <color=#ff0000>{addBloodAmount.Value}L</color>");
							}
						}
						else
						{
							command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid blood-quality '{searchBloodQuality}'. Should be between 1 and 100</color>");
						}
					}
					else
					{
						command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid blood-type '{searchBloodType}'. Options are: {string.Join(", ", validBloodTypes.Select(x => x.ToString()))}</color>");
					}
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}


		[Command("blood-potion,bloodpotion,bloodpot", "blood-potion <blood-type> <blood-quality>", "Creates a Blood Potion with the given Blood Type and Blood Quality", AdminLevel.Admin)]
		private static void OnBloodPotionCommand(Command command)
		{
			if(command.Args.Length >= 2)
			{
				var searchBloodType = command.Args[0];
				var validBloodTypes = BloodTypeExtensions.BloodTypeToPrefabGUIDMapping.Keys.ToList();
				if(Enum.TryParse(searchBloodType.ToLowerInvariant(), true, out BloodType bloodType) && validBloodTypes.Contains(bloodType))
				{
					var searchBloodQuality = command.Args[1];
					if(int.TryParse(searchBloodQuality.Replace("%", string.Empty), out var bloodQuality) && bloodQuality >= 1 && bloodQuality <= 100)
					{
						var server = VWorld.Server;
						var entityManager = server.EntityManager;
						var itemHashLookupMap = server.GetExistingSystem<ServerScriptMapper>()._GameDataSystem.ItemHashLookupMap;

						if(Utils.TryGiveItem(entityManager, itemHashLookupMap, command.VModCharacter.FromCharacter.Character, Utils.BloodPotion, 1, out _, out var itemEntity) && itemEntity != Entity.Null && entityManager.HasComponent<StoredBlood>(itemEntity))
						{
							Utils.Logger.LogMessage($"{command.VModCharacter.Character} has created a {bloodQuality}% {searchBloodType} blood potion");

							var storedBlood = entityManager.GetComponentData<StoredBlood>(itemEntity);
							storedBlood.BloodType = new PrefabGUID((int)bloodType);
							storedBlood.BloodQuality = bloodQuality;
							entityManager.SetComponentData(itemEntity, storedBlood);

							command.VModCharacter.SendSystemMessage($"You received a <color=#ff0000>{bloodQuality}%</color> <color=#ffffff>{searchBloodType}</color> Blood Potion");
						}
					}
					else
					{
						command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid blood-quality '{searchBloodQuality}'. Should be between 1 and 100</color>");
					}
				}
				else
				{
					command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid blood-type '{searchBloodType}'. Options are: {string.Join(", ", validBloodTypes.Select(x => x.ToString()))}</color>");
				}
			}
			else
			{
				CommandSystem.SendInvalidCommandMessage(command);
			}
			command.Use();
		}

		#endregion
	}
}
