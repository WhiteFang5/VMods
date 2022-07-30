using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Unity.Entities;
using VMods.Shared;
using Wetstone.API;

namespace VMods.PvPLeaderboard
{
	public static class PvPLeaderboardSystem
	{
		#region Consts

		private const string PvPPunishmentFileName = "PvPLeaderboard.json";

		#endregion

		#region Variables

		private static Dictionary<ulong, PvPStats> _pvpStats;

		#endregion

		#region Properties

		private static IEnumerable<KeyValuePair<ulong, PvPStats>> PvPLeaderboard => _pvpStats.OrderByDescending(x => x.Value.KDRatio).ThenByDescending(x => x.Value.Kills).ThenBy(x => x.Value.Deaths);

		#endregion

		#region Public Methods

		public static void Initialize()
		{
			_pvpStats = VModStorage.Load(PvPPunishmentFileName, () => new Dictionary<ulong, PvPStats>());

			VModStorage.SaveEvent += Save;
			VampireDownedHook.VampireDownedEvent += OnVampireDowned;
		}

		public static void Deinitialize()
		{
			VampireDownedHook.VampireDownedEvent -= OnVampireDowned;
			VModStorage.SaveEvent -= Save;
		}

		public static void Save()
		{
			VModStorage.Save(PvPPunishmentFileName, _pvpStats);
		}

		#endregion

		#region Private Methods

		private static void OnVampireDowned(Entity killer, Entity victim)
		{
			if(!PvPLeaderboardConfig.PvPLeaderboardEnabled.Value)
			{
				return;
			}
			var entityManager = VWorld.Server.EntityManager;

			Entity killerUserEntity = entityManager.GetComponentData<PlayerCharacter>(killer).UserEntity._Entity;
			var killerUser = entityManager.GetComponentData<User>(killerUserEntity);
			ulong killerSteamID = killerUser.PlatformId;
			float killerLevel = HighestGearScoreSystem.GetCurrentOrHighestGearScore(new FromCharacter()
			{
				User = killerUserEntity,
				Character = killer,
			});

			Entity victimUserEntity = entityManager.GetComponentData<PlayerCharacter>(victim).UserEntity._Entity;
			var victimUser = entityManager.GetComponentData<User>(victimUserEntity);
			ulong victimSteamID = victimUser.PlatformId;
			float victimLevel = HighestGearScoreSystem.GetCurrentOrHighestGearScore(new FromCharacter()
			{
				User = victimUserEntity,
				Character = victim,
			});

			var diff = killerLevel - victimLevel;

#if DEBUG
			var msg = $"{killerUser.CharacterName} ({killerSteamID}) [Lv: {killerLevel}] killed {victimUser.CharacterName} ({victimSteamID}) [Lv: {victimLevel}]. - Diff: {diff}";
			Utils.Logger.LogMessage(msg);
			//Utils.SendMessage(killerUserEntity, msg, ServerChatMessageType.System);
			//Utils.SendMessage(victimUserEntity, msg, ServerChatMessageType.System);
#endif

			if(diff >= PvPLeaderboardConfig.PvPLeaderboardLevelDifference.Value)
			{
				Utils.Logger.LogMessage($"Vampire {killerUser.CharacterName} (Lv: {killerLevel}; Current Lv: {HighestGearScoreSystem.GetCurrentGearScore(killer, entityManager)}) has grief-killed {victimUser.CharacterName} (Lv {victimLevel}; Current Lv: {HighestGearScoreSystem.GetCurrentGearScore(killer, entityManager)})!");
				if(PvPLeaderboardConfig.PvPLeaderboardAnnounceLowLevelKill.Value)
				{
					ServerChatUtils.SendSystemMessageToAllClients(entityManager, $"Vampire <color=#ffffff>{killerUser.CharacterName}</color> (Lv {killerLevel}) has grief-killed <color=#ffffff>{victimUser.CharacterName}</color> (Lv {victimLevel})!");
				}
				return;
			}

			if(!_pvpStats.TryGetValue(killerSteamID, out var killerPvPStats))
			{
				killerPvPStats = new PvPStats();
				_pvpStats.Add(killerSteamID, killerPvPStats);
			}
			if(!_pvpStats.TryGetValue(victimSteamID, out var victimPvPStats))
			{
				victimPvPStats = new PvPStats();
				_pvpStats.Add(victimSteamID, victimPvPStats);
			}

			killerPvPStats.AddKill();
			victimPvPStats.AddDeath();

			if(PvPLeaderboardConfig.PvPLeaderboardAnnounceKill.Value)
			{
				ServerChatUtils.SendSystemMessageToAllClients(entityManager, $"Vampire \"{killerUser.CharacterName}\" has killed \"{victimUser.CharacterName}\"!");
			}
		}

		[Command("pvpstats,pvp", "pvpstats", "Shows your current pvp stats (kills, deaths & K/D ratio).")]
		private static void OnPvPStatsCommand(Command command)
		{
			var entityManager = VWorld.Server.EntityManager;
			(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);

			if(vmodCharacter.HasValue)
			{
				var user = vmodCharacter.Value.User;
				if(!_pvpStats.TryGetValue(user.PlatformId, out var pvpStats))
				{
					pvpStats = new PvPStats();
					_pvpStats[user.PlatformId] = pvpStats;
				}
				command.User.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> K/D: {pvpStats.KDRatio} [{pvpStats.Kills}/{pvpStats.Deaths}] - Rank {PvPLeaderboard.ToList().FindIndex(x => x.Key == user.PlatformId) + 1}");
			}
			command.Use();
		}

		[Command("pvplb,pvpleaderboard", "pvplb [<pagenum>]", "Shows the 5 players on the requested page of the leaderboard (or top 5 if no page is given).")]
		private static void OnPvPLeaderboardCommand(Command command)
		{
			int page = 0;
			if(command.Args.Length >= 1 && int.TryParse(command.Args[0], out page))
			{
				page -= 1;
			}

			var recordsPerPage = 5;

			var maxPage = (int)Math.Ceiling(_pvpStats.Count / (double)recordsPerPage);
			page = Math.Min(maxPage - 1, page);

			var user = command.User;
			var entityManager = VWorld.Server.EntityManager;
			var leaderboard = PvPLeaderboard.Skip(page * recordsPerPage).Take(recordsPerPage);
			user.SendSystemMessage("========== PvP Leaderboard ==========");
			int rank = (page * recordsPerPage) + 1;
			foreach((var platformId, var pvpStats) in leaderboard)
			{
				user.SendSystemMessage($"{rank}. <color=#ffffff>{Utils.GetCharacterName(platformId, entityManager)} : {pvpStats.KDRatio} [{pvpStats.Kills}/{pvpStats.Deaths}]</color>");
				rank++;
			}
			user.SendSystemMessage($"=============== {page + 1}/{maxPage} ===============");

			command.Use();
		}

		#endregion

		#region Nested

		private class PvPStats
		{
			#region Properties

			public int Kills { get; private set; }
			public int Deaths { get; private set; }
			public double KDRatio { get; private set; }

			#endregion

			#region Lifecycle

			[JsonConstructor]
			public PvPStats(int kills, int deaths, double kdRatio)
			{
				(Kills, Deaths, KDRatio) = (kills, deaths, kdRatio);

				CalcKDRatio();
			}

			public PvPStats()
			{
				Kills = 0;
				Deaths = 0;
				KDRatio = 1d;
			}

			#endregion

			#region Public Methods

			public void AddKill()
			{
				Kills++;
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

		#endregion
	}
}
