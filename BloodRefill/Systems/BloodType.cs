using ProjectM;
using System;
using System.Collections.Generic;

namespace VMods.BloodRefill
{
	public enum BloodType
	{
		Frailed = -899826404,
		Creature = -77658840,
		Warrior = -1094467405,
		Rogue = 793735874,
		Brute = 581377887,
		Scholar = -586506765,
		Worker = -540707191,
		VBlood = 1557174542,
	}

	public static class BloodTypeExtensions
	{
		#region Consts

		public static readonly Dictionary<BloodType, PrefabGUID> BloodTypeToPrefabGUIDMapping = new()
		{
			[BloodType.Creature] = new PrefabGUID(1897056612),
			[BloodType.Warrior] = new PrefabGUID(-1128238456),
			[BloodType.Rogue] = new PrefabGUID(-1030822544),
			[BloodType.Brute] = new PrefabGUID(-1464869978),
			[BloodType.Scholar] = new PrefabGUID(-700632469),
			[BloodType.Worker] = new PrefabGUID(-1342764880),
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

		#endregion
	}
}
