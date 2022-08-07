using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Wetstone.API;
using AdminLevel = VMods.Shared.CommandAttribute.AdminLevel;

namespace VMods.Shared
{
	public static class HighestGearScoreSystem
	{
		#region Variables

		private static Dictionary<ulong, GearScoreData> _gearScoreData;

		#endregion

		#region Properties

		private static string HighestGearScoreFileName => $"{Utils.PluginName}-HighestGearScore.json";

		#endregion

		#region Public Methods

		public static void Initialize()
		{
			_gearScoreData = VModStorage.Load(HighestGearScoreFileName, () => new Dictionary<ulong, GearScoreData>());

			VModStorage.SaveEvent += Save;
			EquipmentHooks.EquipmentChangedEvent += OnEquipmentChanged;
			VampireDownedHook.VampireDownedByVampireEvent += OnVampireDowned;
		}

		public static void Deinitialize()
		{
			VampireDownedHook.VampireDownedByVampireEvent -= OnVampireDowned;
			EquipmentHooks.EquipmentChangedEvent -= OnEquipmentChanged;
			VModStorage.SaveEvent -= Save;
		}

		public static void Save()
		{
			PruneHighestGearScores();

			VModStorage.Save(HighestGearScoreFileName, _gearScoreData);
		}

		public static float GetCurrentOrHighestGearScore(VModCharacter vmodCharacter)
		{
			return GetCurrentOrHighestGearScore(vmodCharacter.FromCharacter);
		}

		public static float GetCurrentOrHighestGearScore(FromCharacter fromCharacter)
		{
			var entityManager = VWorld.Server.EntityManager;
			var currentGS = GetCurrentGearScore(fromCharacter, entityManager);
			if(HighestGearScoreSystemConfig.HighestGearScoreSystemEnabled.Value)
			{
				PruneHighestGearScores();

				var user = entityManager.GetComponentData<User>(fromCharacter.User);
				if(_gearScoreData.TryGetValue(user.PlatformId, out var gearScoreData))
				{
					return Math.Max(currentGS, gearScoreData.HighestGearScore);
				}
			}
			return currentGS;
		}

		public static float GetCurrentGearScore(FromCharacter fromCharacter, EntityManager entityManager)
		{
			return GetCurrentGearScore(fromCharacter.Character, entityManager);
		}

		public static float GetCurrentGearScore(VModCharacter vmodCharacter, EntityManager entityManager)
		{
			return GetCurrentGearScore(vmodCharacter.FromCharacter.Character, entityManager);
		}

		public static float GetCurrentGearScore(Entity characterEntity, EntityManager entityManager)
		{
			var equipment = entityManager.GetComponentData<Equipment>(characterEntity);
			return equipment.ArmorLevel + equipment.WeaponLevel + equipment.SpellLevel;
		}

		#endregion

		#region Private Methods

		private static void PruneHighestGearScores()
		{
			var now = DateTime.UtcNow;
			var keys = _gearScoreData.Keys.ToList();
			var duration = HighestGearScoreSystemConfig.HighestGearScoreDuration.Value;
			foreach(var key in keys)
			{
				var gearScoreData = _gearScoreData[key];
				if(now.Subtract(gearScoreData.LastUpdated).TotalSeconds > duration)
				{
					_gearScoreData.Remove(key);
				}
			}
		}

		private static void OnEquipmentChanged(FromCharacter fromCharacter)
		{
			if(!HighestGearScoreSystemConfig.HighestGearScoreSystemEnabled.Value)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var user = entityManager.GetComponentData<User>(fromCharacter.User);
			if(!_gearScoreData.TryGetValue(user.PlatformId, out var gearScoreData))
			{
				gearScoreData = new GearScoreData();
				_gearScoreData.Add(user.PlatformId, gearScoreData);
			}

			if(DateTime.UtcNow.Subtract(gearScoreData.LastUpdated).TotalSeconds >= 30f)
			{
				gearScoreData.HighestGearScore = 0f;
			}

			float gearScore = GetCurrentGearScore(fromCharacter.Character, entityManager);
			gearScoreData.HighestGearScore = Math.Max(gearScoreData.HighestGearScore, gearScore);
			gearScoreData.LastUpdated = DateTime.UtcNow;

#if DEBUG
			//var message = $"Highest Gearscore Updated: {gearScoreData.HighestGearScore} (Current: {gearScore})";
			//Utils.Logger.LogMessage(message);
			//user.SendSystemMessage($"Highest Gearscore Updated: {gearScoreData.HighestGearScore} (Current: {gearScore})");
#endif
		}

		private static void OnVampireDowned(Entity killer, Entity victim)
		{
			var entityManager = VWorld.Server.EntityManager;
			var victimCharacter = entityManager.GetComponentData<PlayerCharacter>(victim);
			var victumUserEntity = victimCharacter.UserEntity._Entity;
			var victumUser = entityManager.GetComponentData<User>(victumUserEntity);

			_gearScoreData.Remove(victumUser.PlatformId);
		}

		[Command("highestgs,hgs,higs,highgs,highestgearscore", "highestgs [<player-name>]", "Tells you what the highest gear score is for the given player (or yourself when noplayername is given)", AdminLevel.Admin)]
		private static void OnHighestGearScoreCommand(Command command)
		{
			var entityManager = VWorld.Server.EntityManager;
			(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);

			if(vmodCharacter.HasValue)
			{
				var user = vmodCharacter.Value.User;
				if(_gearScoreData.TryGetValue(user.PlatformId, out var gearScoreData))
				{
					TimeSpan diff = DateTime.UtcNow.Subtract(gearScoreData.LastUpdated);
					command.VModCharacter.SendSystemMessage($"[{Utils.PluginName}] The Highest Gear Score for <color=#ffffff>{searchUsername}</color> (Lv: {GetCurrentGearScore(vmodCharacter.Value, entityManager)}) was <color=#00ff00>{gearScoreData.HighestGearScore}</color> (Last updated {diff.ToAgoString()} ago).");
				}
				else
				{
					command.VModCharacter.SendSystemMessage($"[{Utils.PluginName}] No Highest Gear Score is recorded for <color=#ffffff>{searchUsername}</color> (Lv: {GetCurrentGearScore(vmodCharacter.Value, entityManager)}).");
				}
			}
		}

		[Command("clearhgs,resethgs,clearhighestgearscore,resethighestgearscore", "clearhgs [<player-name>]", "Removes the current Highest Gear Score record for the given player (or yourself when noplayername is given)", AdminLevel.Admin)]
		private static void OnResetHighestGearScoreCommand(Command command)
		{
			var entityManager = VWorld.Server.EntityManager;
			(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);

			if(vmodCharacter.HasValue)
			{
				var user = vmodCharacter.Value.User;
				_gearScoreData.Remove(user.PlatformId);
				command.VModCharacter.SendSystemMessage($"[{Utils.PluginName}] Removed the Highest Gear Score record for <color=#ffffff>{searchUsername}</color>.");
			}
		}

		#endregion

		#region Nested

		private class GearScoreData
		{
			public float HighestGearScore { get; set; }
			public DateTime LastUpdated { get; set; }
		}

		#endregion
	}
}
