using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VMods.Shared
{
	public static class FactionEnumExtensions
	{
		#region Consts

		private static readonly Dictionary<PrefabGUID, FactionEnum> PrefabGUIDToFactionEnumMapping = new()
		{
			[new PrefabGUID(-1632475814)] = FactionEnum.Ashfolk,
			[new PrefabGUID(-413163549)] = FactionEnum.Bandits,
			[new PrefabGUID(1344481611)] = FactionEnum.Bear,
			[new PrefabGUID(1094603131)] = FactionEnum.ChurchOfLum,
			[new PrefabGUID(2395673)] = FactionEnum.ChurchOfLum | FactionEnum.Spot_ShapeshiftHuman,
			[new PrefabGUID(10678632)] = FactionEnum.Critters,
			[new PrefabGUID(1522496317)] = FactionEnum.Cursed,
			[new PrefabGUID(1513046884)] = FactionEnum.Elementals,
			[new PrefabGUID(1731533561)] = FactionEnum.Harpy,
			[new PrefabGUID(-1430861195)] = FactionEnum.Ignored,
			[new PrefabGUID(1057375699)] = FactionEnum.Militia,
			[new PrefabGUID(1597367490)] = FactionEnum.NatureSpirit,
			[new PrefabGUID(-1414061934)] = FactionEnum.Plants,
			[new PrefabGUID(1106458752)] = FactionEnum.Players,
			[new PrefabGUID(-394968526)] = FactionEnum.PlayerCastlePrisoner,
			[new PrefabGUID(-1036907707)] = FactionEnum.Players_ShapeshiftHuman,
			[new PrefabGUID(-1632009503)] = FactionEnum.Spiders,
			//[new PrefabGUID(887347866)] = FactionEnum.None,//Faction_Traders
			[new PrefabGUID(929074293)] = FactionEnum.Undead,
			[new PrefabGUID(2120169232)] = FactionEnum.VampireHunters,
			[new PrefabGUID(-2024618997)] = FactionEnum.Werewolves,
			[new PrefabGUID(-1671358863)] = FactionEnum.Wolves,
			[new PrefabGUID(1977351396)] = FactionEnum.Prisoners,
		};

		#endregion

		#region Public Methods

		public static FactionEnum ToFactionEnum(this PrefabGUID prefabGUID)
		{
			if(PrefabGUIDToFactionEnumMapping.TryGetValue(prefabGUID, out var faction))
			{
				return faction;
			}
			return FactionEnum.None;
		}

		public static FactionEnum[] Split(this FactionEnum factions)
		{
			return ((FactionEnum[])Enum.GetValues(typeof(FactionEnum))).Where(x => x != FactionEnum.None && factions.HasFlag(x)).ToArray();
		}

		#endregion
	}
}
