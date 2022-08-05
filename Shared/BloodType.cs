using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using Wetstone.API;

namespace VMods.Shared
{
	public enum BloodType
	{
		Frailed = -899826404,//(UnitBloodType) BloodType_None
		Creature = -77658840,//(UnitBloodType) BloodType_Creature
		Warrior = -1094467405,//(UnitBloodType) BloodType_Warrior
		Rogue = 793735874,//(UnitBloodType) BloodType_Rogue
		Brute = 581377887,//(UnitBloodType) BloodType_Brute
		Scholar = -586506765,//(UnitBloodType) BloodType_Scholar
		Worker = -540707191,//(UnitBloodType) BloodType_Worker
		VBlood = 1557174542,//(UnitBloodType) BloodType_VBlood
	}

	public static class BloodTypeExtensions
	{
		#region Consts

		public static readonly Dictionary<BloodType, PrefabGUID> BloodTypeToPrefabGUIDMapping = new()
		{
			[BloodType.Creature] = new PrefabGUID(1897056612),//CHAR_Wildlife_Deer
			[BloodType.Warrior] = new PrefabGUID(-1128238456),//CHAR_Bandit_Bomber
			[BloodType.Rogue] = new PrefabGUID(-1030822544),//CHAR_Bandit_Deadeye
			[BloodType.Brute] = new PrefabGUID(-1464869978),//CHAR_Town_Cleric
			[BloodType.Scholar] = new PrefabGUID(-700632469),//CHAR_Farmlands_Nun
			[BloodType.Worker] = new PrefabGUID(-1342764880),//CHAR_Farmlands_Farmer
		};

		#endregion

		#region Public Methods

		public static bool ParseBloodType(this PrefabGUID prefabGUID, out BloodType bloodType)
		{
			int guidHash = prefabGUID.GuidHash;
			if(!Enum.IsDefined(typeof(BloodType), guidHash))
			{
				bloodType = BloodType.Frailed;
				return false;
			}
			bloodType = (BloodType)guidHash;
			return true;
		}

		public static BloodType? ToBloodType(this PrefabGUID prefabGUID)
		{
			int guidHash = prefabGUID.GuidHash;
			if(!Enum.IsDefined(typeof(BloodType), guidHash))
			{
				return null;
			}
			return (BloodType)guidHash;
		}

		public static PrefabGUID ToPrefabGUID(this BloodType bloodType) => BloodTypeToPrefabGUIDMapping[bloodType];

		public static void ApplyToPlayer(this BloodType bloodType, User user, float quality, int addAmount)
		{
			ChangeBloodDebugEvent bloodChangeEvent = new()
			{
				Source = bloodType.ToPrefabGUID(),
				Quality = quality,
				Amount = addAmount,
			};
			VWorld.Server.GetExistingSystem<DebugEventsSystem>().ChangeBloodEvent(user.Index, ref bloodChangeEvent);
		}

		#endregion
	}
}
