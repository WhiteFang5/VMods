using BepInEx.Configuration;

namespace VMods.PvPPunishment
{
	public static class PvPPunishmentConfig
	{
		#region Properties

		public static ConfigEntry<bool> PvPPunishmentEnabled { get; private set; }
		public static ConfigEntry<int> PvPPunishmentLevelDifference { get; private set; }
		public static ConfigEntry<int> PvPPunishmentOffenseLimit { get; private set; }
		public static ConfigEntry<float> PvPPunishmentOffenseCooldown { get; private set; }
		public static ConfigEntry<float> PvPPunishmentDuration { get; private set; }
		public static ConfigEntry<float> PvPPunishmentMovementSpeedReduction { get; private set; }
		public static ConfigEntry<float> PvPPunishmentMaxHealthReduction { get; private set; }
		public static ConfigEntry<float> PvPPunishmentPhysResistReduction { get; private set; }
		public static ConfigEntry<float> PvPPunishmentSpellResistReduction { get; private set; }
		public static ConfigEntry<float> PvPPunishmentFireResistReduction { get; private set; }
		public static ConfigEntry<float> PvPPunishmentHolyResistReduction { get; private set; }
		public static ConfigEntry<float> PvPPunishmentSunResistReduction { get; private set; }
		public static ConfigEntry<float> PvPPunishmentSilverResistReduction { get; private set; }
		public static ConfigEntry<float> PvPPunishmentPhysPowerReduction { get; private set; }
		public static ConfigEntry<float> PvPPunishmentSpellPowerReduction { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			PvPPunishmentEnabled = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentEnabled), false, "Enabled/disable the PvP Punishment system.");
			PvPPunishmentLevelDifference = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentLevelDifference), 10, "The level difference at which to apply a punishment to the killer.");
			PvPPunishmentOffenseLimit = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentOffenseLimit), 3, "The amount of offenses a player can commit before being punished.");
			PvPPunishmentOffenseCooldown = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentOffenseCooldown), 300f, "The amount of seconds since the last offense at which the offense counter resets.");
			PvPPunishmentDuration = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentDuration), 1800f, "The amount of seconds the punishment buff lasts.");
			PvPPunishmentMovementSpeedReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentMovementSpeedReduction), 15f, "The percentage of reduced Movement Speed when a player is punished.");
			PvPPunishmentMaxHealthReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentMaxHealthReduction), 15f, "The percentage of reduced Max Health when a player is punished.");
			PvPPunishmentPhysResistReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentPhysResistReduction), 15f, "The amount of reduced Physical Resistance when a player is punished.");
			PvPPunishmentSpellResistReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentSpellResistReduction), 15f, "The amount of reduced Spell Resistance when a player is punished.");
			PvPPunishmentFireResistReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentFireResistReduction), 15f, "The amount of reduced Fire Resistance when a player is punished.");
			PvPPunishmentHolyResistReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentHolyResistReduction), 15f, "The amount of reduced Holy Resistance when a player is punished.");
			PvPPunishmentSunResistReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentSunResistReduction), 15f, "The amount of reduced Sun Resistance when a player is punished.");
			PvPPunishmentSilverResistReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentSilverResistReduction), 15f, "The amount of reduced Silver Resistance when a player is punished.");
			PvPPunishmentPhysPowerReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentPhysPowerReduction), 15f, "The percentage of reduced Physical Power when a player is punished.");
			PvPPunishmentSpellPowerReduction = config.Bind(nameof(PvPPunishmentConfig), nameof(PvPPunishmentSpellPowerReduction), 15f, "The percentage of reduced Spell Power when a player is punished.");
		}

		#endregion
	}
}
