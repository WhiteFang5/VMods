using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Unity.Entities;
using VMods.Shared;
using Wetstone.API;

namespace VMods.PvELeaderboard
{
	public static class PvELeaderboardSystem
	{
		#region Consts

		private const string PvELeaderboardFileName = "PvELeaderboard.json";

		private static readonly List<FactionEnum> TrackedFactions = new()
		{
			FactionEnum.Wolves,
			FactionEnum.Undead,
			FactionEnum.Militia,
			FactionEnum.ChurchOfLum,
			FactionEnum.Prisoners,
			FactionEnum.Bandits,
			FactionEnum.Bear,
			FactionEnum.Plants,
			FactionEnum.Harpy,
			FactionEnum.Critters,
			FactionEnum.Werewolves,
			FactionEnum.NatureSpirit,
			FactionEnum.Spiders,
			FactionEnum.VampireHunters,
			FactionEnum.Cursed,
			FactionEnum.Ashfolk,
			FactionEnum.Elementals,
		};

		private static readonly List<BloodType> TrackedBloodTypes = new()
		{
			BloodType.Creature,
			BloodType.Warrior,
			BloodType.Rogue,
			BloodType.Brute,
			BloodType.Scholar,
			BloodType.Worker,
			BloodType.VBlood,
		};

		#endregion

		#region Variables

		private static Dictionary<ulong, PvEStats> _pveStats;

		#endregion

		#region Public Methods

		public static void Initialize()
		{
			_pveStats = VModStorage.Load(PvELeaderboardFileName, () => new Dictionary<ulong, PvEStats>());

			VModStorage.SaveEvent += Save;
			VampireDownedHook.VampireDownedByMonsterEvent += OnVampireDowned;
			DeathHook.DeathEvent += OnDeath;
		}

		public static void Deinitialize()
		{
			DeathHook.DeathEvent -= OnDeath;
			VampireDownedHook.VampireDownedByMonsterEvent -= OnVampireDowned;
			VModStorage.SaveEvent -= Save;
		}

		public static void Save()
		{
			VModStorage.Save(PvELeaderboardFileName, _pveStats);
		}

		#endregion

		#region Private Methods

		private static void OnDeath(DeathEvent deathEvent)
		{
			var entityManager = VWorld.Server.EntityManager;

			if(!PvELeaderboardConfig.PvELeaderboardEnabled.Value ||
				!entityManager.HasComponent<PlayerCharacter>(deathEvent.Killer) ||
				!entityManager.HasComponent<UnitLevel>(deathEvent.Died) ||
				!entityManager.HasComponent<FactionReference>(deathEvent.Died))
			{
				return;
			}

			VModCharacter killerVModCharacter = new VModCharacter(deathEvent.Killer, entityManager);
			float killerLevel = HighestGearScoreSystem.GetCurrentOrHighestGearScore(killerVModCharacter);

			var victim = deathEvent.Died;
			var victimUnitLevel = entityManager.GetComponentData<UnitLevel>(victim);
			float victimLevel = victimUnitLevel.Level;
			var victimFaction = entityManager.GetComponentData<FactionReference>(victim).FactionGuid.Value.ToFactionEnum();
			BloodType? victimBloodType = null;
			if(entityManager.HasComponent<BloodConsumeSource>(victim))
			{
				var bloodconsumeSource = entityManager.GetComponentData<BloodConsumeSource>(victim);
				var bloodType = bloodconsumeSource.UnitBloodType.ToBloodType();
				if(bloodType.HasValue && TrackedBloodTypes.Contains(bloodType.Value))
				{
					victimBloodType = bloodType;
				}
			}

			if(!TrackedFactions.Exists(x => victimFaction.HasFlag(x)))
			{
				return;
			}

			var diff = killerLevel - victimLevel;
			if(diff >= PvELeaderboardConfig.PvELeaderboardLevelDifference.Value)
			{
				return;
			}

			if(!_pveStats.TryGetValue(killerVModCharacter.PlatformId, out var killerPvEStats))
			{
				killerPvEStats = new PvEStats();
				_pveStats.Add(killerVModCharacter.PlatformId, killerPvEStats);
			}

			killerPvEStats.AddKill((ulong)victimLevel, victimFaction, victimBloodType);
		}

		private static void OnVampireDowned(Entity killer, Entity victim)
		{
			if(!PvELeaderboardConfig.PvELeaderboardEnabled.Value)
			{
				return;
			}
			var entityManager = VWorld.Server.EntityManager;

			if(!entityManager.HasComponent<FactionReference>(killer))
			{
				return;
			}

			var killerUnitLevel = entityManager.GetComponentData<UnitLevel>(killer);
			float killerLevel = killerUnitLevel.Level;
			var killerFaction = entityManager.GetComponentData<FactionReference>(killer).FactionGuid.Value.ToFactionEnum();
			BloodType? killerBloodType = null;
			if(entityManager.HasComponent<BloodConsumeSource>(killer))
			{
				var bloodConsumeSource = entityManager.GetComponentData<BloodConsumeSource>(killer);
				var bloodType = bloodConsumeSource.UnitBloodType.ToBloodType();
				if(bloodType.HasValue && TrackedBloodTypes.Contains(bloodType.Value))
				{
					killerBloodType = bloodType;
				}
			}

			if(!TrackedFactions.Exists(x => killerFaction.HasFlag(x)))
			{
				return;
			}

			Entity victimUserEntity = entityManager.GetComponentData<PlayerCharacter>(victim).UserEntity._Entity;
			var victimUser = entityManager.GetComponentData<User>(victimUserEntity);
			ulong victimSteamID = victimUser.PlatformId;
			float victimLevel = HighestGearScoreSystem.GetCurrentOrHighestGearScore(new FromCharacter()
			{
				User = victimUserEntity,
				Character = victim,
			});

			var diff = killerLevel - victimLevel;
			if(diff >= PvELeaderboardConfig.PvELeaderboardLevelDifference.Value)
			{
				return;
			}

			if(!_pveStats.TryGetValue(victimSteamID, out var victimPvEStats))
			{
				victimPvEStats = new PvEStats();
				_pveStats.Add(victimSteamID, victimPvEStats);
			}

			victimPvEStats.AddDeath(killerFaction, killerBloodType);
		}

		[Command("pvestats,pve", "pvestats [<player-name>] [<faction>/<blood-type>/all/overall]", "Shows your current PvE stats (kills, deaths, K/D ratio & lvl kills) for the given faction, blood-type, the overall or all of them together.")]
		private static void OnPvEStatsCommand(Command command)
		{
			var entityManager = VWorld.Server.EntityManager;
			(var searchUsername, var vmodCharacter) = command.FindVModCharacter(argIdx: command.Args.Length - 2, entityManager: entityManager);

			if(vmodCharacter.HasValue)
			{
				var user = vmodCharacter.Value.User;
				if(!_pveStats.TryGetValue(user.PlatformId, out var pveStats))
				{
					pveStats = new PvEStats();
					_pveStats[user.PlatformId] = pveStats;
				}

				if(command.Args.Length >= 1)
				{
					var searchLeaderboard = command.Args[command.Args.Length switch
					{
						1 => 0,
						_ => 1,
					}];
					switch(searchLeaderboard)
					{
						case "all":
							{
								SendKDStats(pveStats.OverallKDStats, "Overall PvE", FactionEnum.None, null);
								foreach(var faction in TrackedFactions)
								{
									if(pveStats.FactionKDStats.TryGetValue(faction, out var kdStats))
									{
										SendKDStats(kdStats, $"{faction} faction", faction, null);
									}
								}
							}
							break;

						case "overall":
							SendKDStats(pveStats.OverallKDStats, "Overall PvE", FactionEnum.None, null);
							break;

						default:
							{
								if(Enum.TryParse<FactionEnum>(searchLeaderboard, true, out var faction))
								{
									if(pveStats.FactionKDStats.TryGetValue(faction, out var kdStats))
									{
										SendKDStats(kdStats, $"{faction}", faction, null);
									}
									else
									{
										command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername} is currently unranked for the {faction} leaderboard</color>");
									}
								}
								else if(Enum.TryParse<BloodType>(searchLeaderboard, true, out var bloodType))
								{
									if(pveStats.BloodTypeKDStats.TryGetValue(bloodType, out var kdStats))
									{
										SendKDStats(kdStats, $"{bloodType}", null, bloodType);
									}
									else
									{
										command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername} is currently unranked for the {bloodType} leaderboard</color>");
									}
								}
								else
								{
									command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid faction/blood-type option. Options are: {string.Join(", ", TrackedFactions)}, {string.Join(", ", TrackedBloodTypes)}, all, overall</color>");
								}
							}
							break;
					}
				}
				else
				{
					SendKDStats(pveStats.OverallKDStats, "Overall PvE", FactionEnum.None, null);
				}

				void SendKDStats(KDStats kdStats, string name, FactionEnum? lbFaction, BloodType? lbBloodType)
				{
					command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> {name} K/D: {kdStats.KDRatio} [{kdStats.Kills}/{kdStats.Deaths}] - Lvl Kills: {kdStats.LevelKills} - Rank {GetLeaderboard(lbFaction, lbBloodType).ToList().FindIndex(x => x.platformId == user.PlatformId) + 1} - Lvl Kills Rank {GetLvlKillsLeaderboard(lbFaction, lbBloodType).ToList().FindIndex(x => x.platformId == user.PlatformId) + 1}");
				}
			}
			command.Use();
		}

		[Command("pvelb,pveleaderboard", "pvelb [<faction/blood-type>] [<pagenum>]", "Shows the 5 players on the requested page of the given faction, blood-type or the overall PvE leaderboard (or top 5 if no page is given).")]
		private static void OnPvELeaderboardCommand(Command command)
		{
			OnPvELeaderboardCommand(command, GetLeaderboard, string.Empty);
		}

		[Command("pvelklb,pvelvlkillsleaderboard", "pvelklb [<faction/blood-type>] [<pagenum>]", "Shows the 5 players on the requested page of the given faction, blood-type or the overall PvE leaderboard (or top 5 if no page is given) sorted by the Lvl Kills stat.")]
		private static void OnPvELvlKillsLeaderboardCommand(Command command)
		{
			OnPvELeaderboardCommand(command, GetLvlKillsLeaderboard, " (Lvl Kills)");
		}

		private static void OnPvELeaderboardCommand(Command command, Func<FactionEnum?, BloodType?, IEnumerable<(ulong platformId, KDStats kdStats)>> getLeaderboardMethod, string leaderboardNameSuffix)
		{
			int argCount = command.Args.Length;
			int argIdx = 0;
			FactionEnum? lbFaction = null;
			BloodType? lbBloodType = null;
			string leaderboardName = null;
			if(command.Args.Length >= 1)
			{
				var firstArg = command.Args[argIdx];
				bool hasTwoArgs = argCount >= 2;
				if(hasTwoArgs || (argCount == 1 && !int.TryParse(firstArg, out _)))
				{
					if(Enum.TryParse<FactionEnum>(firstArg, true, out var faction))
					{
						lbFaction = faction;
						leaderboardName = faction.ToString();
					}
					else if(Enum.TryParse<BloodType>(firstArg, true, out var bloodType))
					{
						lbBloodType = bloodType;
						leaderboardName = bloodType.ToString();
					}
					else
					{
						command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid faction/blood-type option. Options are: {string.Join(", ", TrackedFactions)}, {string.Join(", ", TrackedBloodTypes)}</color>");
					}
					argIdx = Math.Min(argIdx + 1, command.Args.Length - 1);
				}
				else
				{
					lbFaction = FactionEnum.None;
					leaderboardName = "Overall";
				}
			}
			else
			{
				lbFaction = FactionEnum.None;
				leaderboardName = "Overall";
			}

			if(lbFaction.HasValue || lbBloodType.HasValue)
			{
				var leaderboard = getLeaderboardMethod(lbFaction, lbBloodType);

				int page = 0;
				if(argCount >= 1 && int.TryParse(command.Args[argIdx], out page))
				{
					page -= 1;
				}

				var recordsPerPage = 5;

				var maxPage = (int)Math.Ceiling(leaderboard.Count() / (double)recordsPerPage);
				page = Math.Min(maxPage - 1, page);

				var vmodCharacter = command.VModCharacter;
				var entityManager = VWorld.Server.EntityManager;
				var visibleLeaderboard = leaderboard.Skip(page * recordsPerPage).Take(recordsPerPage);
				vmodCharacter.SendSystemMessage($"===== {leaderboardName} PvE Leaderboard{leaderboardNameSuffix} =====");
				int rank = (page * recordsPerPage) + 1;
				foreach((var platformId, var kdStats) in visibleLeaderboard)
				{
					vmodCharacter.SendSystemMessage($"{rank}. <color=#ffffff>{Utils.GetCharacterName(platformId, entityManager)} : {kdStats.LevelKills} - K/D: {kdStats.KDRatio} [{kdStats.Kills}/{kdStats.Deaths}]</color>");
					rank++;
				}
				vmodCharacter.SendSystemMessage($"============ {page + 1}/{maxPage} ============");
			}
			command.Use();
		}

		private static IEnumerable<(ulong platformId, KDStats kdStats)> GetUnsortedLeaderboard(FactionEnum? faction, BloodType? bloodType)
		{
			return faction switch
			{
				null => bloodType switch
				{
					_ => _pveStats
						.Where(x => x.Value.BloodTypeKDStats.ContainsKey(bloodType.Value))
						.Select(x => (x.Key, x.Value.BloodTypeKDStats[bloodType.Value]))
				},
				FactionEnum.None => _pveStats
					.Select(x => (x.Key, x.Value.OverallKDStats)),
				_ => _pveStats
					.Where(x => x.Value.FactionKDStats.ContainsKey(faction.Value))
					.Select(x => (x.Key, x.Value.FactionKDStats[faction.Value])),
			};
		}

		private static IEnumerable<(ulong platformId, KDStats kdStats)> GetLeaderboard(FactionEnum? faction, BloodType? bloodType)
		{
			return GetUnsortedLeaderboard(faction, bloodType)
				.OrderByDescending(x => x.kdStats.KDRatio)
				.ThenByDescending(x => x.kdStats.Kills)
				.ThenBy(x => x.kdStats.Deaths)
				.ThenByDescending(x => x.kdStats.LevelKills);
		}

		private static IEnumerable<(ulong platformId, KDStats kdStats)> GetLvlKillsLeaderboard(FactionEnum? faction, BloodType? bloodType)
		{
			return GetUnsortedLeaderboard(faction, bloodType)
				.OrderByDescending(x => x.kdStats.LevelKills)
				.ThenByDescending(x => x.kdStats.KDRatio)
				.ThenByDescending(x => x.kdStats.Kills)
				.ThenBy(x => x.kdStats.Deaths);
		}

		#endregion

		#region Nested

		private class KDStats
		{
			#region Properties

			public ulong Kills { get; private set; }
			public ulong Deaths { get; private set; }
			public double KDRatio { get; private set; }

			public ulong LevelKills { get; private set; }

			#endregion

			#region Lifecycle

			[JsonConstructor]
			public KDStats(ulong kills, ulong deaths, double kdRatio, ulong levelKills)
			{
				(Kills, Deaths, KDRatio, LevelKills) = (kills, deaths, kdRatio, levelKills);

				CalcKDRatio();
			}

			public KDStats()
			{
				Kills = 0;
				Deaths = 0;
				KDRatio = 1d;
				LevelKills = 0;
			}

			#endregion

			#region Public Methods

			public void AddKill(ulong victimLevel)
			{
				Kills++;
				LevelKills += victimLevel;
				CalcKDRatio();
			}

			public void AddDeath()
			{
				Deaths++;
				CalcKDRatio();
			}

			#endregion

			#region Private Methods

			private void CalcKDRatio()
			{
				if(Deaths == 0)
				{
					KDRatio = Kills;
				}
				else
				{
					KDRatio = Kills / (double)Deaths;
				}
			}

			#endregion
		}

		private class PvEStats
		{
			#region Properties

			public KDStats OverallKDStats { get; private set; }

			public Dictionary<FactionEnum, KDStats> FactionKDStats { get; private set; }
			public Dictionary<BloodType, KDStats> BloodTypeKDStats { get; private set; }

			#endregion

			#region Lifecycle

			[JsonConstructor]
			public PvEStats(KDStats overallKDStats, Dictionary<FactionEnum, KDStats> factionKDStats, Dictionary<BloodType, KDStats> bloodTypeKDStats)
			{
				(OverallKDStats, FactionKDStats, BloodTypeKDStats) = (overallKDStats ?? new(), factionKDStats ?? new(), bloodTypeKDStats ?? new());
			}

			public PvEStats()
			{
				OverallKDStats = new();
				FactionKDStats = new();
				BloodTypeKDStats = new();
			}

			#endregion

			#region Public Methods

			public void AddKill(ulong victimLevel, FactionEnum victimFaction, BloodType? bloodType)
			{
				OverallKDStats.AddKill(victimLevel);
				GetOrCreateFactionKDStats(victimFaction).AddKill(victimLevel);
				GetOrCreateBloodTypeKDStats(bloodType)?.AddKill(victimLevel);
			}

			public void AddDeath(FactionEnum killerFaction, BloodType? killerBloodType)
			{
				OverallKDStats.AddDeath();
				GetOrCreateFactionKDStats(killerFaction).AddDeath();
				GetOrCreateBloodTypeKDStats(killerBloodType)?.AddDeath();
			}

			#endregion

			#region Private Methods

			private KDStats GetOrCreateFactionKDStats(FactionEnum faction)
			{
				if(!FactionKDStats.TryGetValue(faction, out var kdStats))
				{
					kdStats = new();
					FactionKDStats.Add(faction, kdStats);
				}
				return kdStats;
			}

			private KDStats GetOrCreateBloodTypeKDStats(BloodType? bloodType)
			{
				if(!bloodType.HasValue)
				{
					return null;
				}
				if(!BloodTypeKDStats.TryGetValue(bloodType.Value, out var kdStats))
				{
					kdStats = new();
					BloodTypeKDStats.Add(bloodType.Value, kdStats);
				}
				return kdStats;
			}

			#endregion
		}

		#endregion
	}
}
