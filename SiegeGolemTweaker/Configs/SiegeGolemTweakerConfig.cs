using BepInEx.Configuration;

namespace VMods.SiegeGolemTweaker
{
	public static class SiegeGolemTweakerConfig
	{
		#region Properties

		public static ConfigEntry<bool> SiegeGolemTweakerEnabled { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerSiegePowerMultiplier { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerPhysicalPowerMultiplier { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerSpellPowerMultiplier { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerMovementSpeedMultiplier { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerAttackSpeedMultiplier { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerMaxHealthMultiplier { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerPassiveHealthRegen { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerPhysicalResistance { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerSpellResistance { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerFireResistance { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerHolyResistance { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerSunResistance { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerSilverResistance { get; private set; }
		public static ConfigEntry<float> SiegeGolemTweakerGarlicResistance { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			SiegeGolemTweakerEnabled = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerEnabled), false, "Enabled/disable the Siege Golem Tweaker system.");

			SiegeGolemTweakerSiegePowerMultiplier = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerSiegePowerMultiplier), 100f, "The percentage of Siege Power a Siege Golem has.");
			SiegeGolemTweakerPhysicalPowerMultiplier = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerPhysicalPowerMultiplier), 100f, "The percentage of Physical Power a Siege Golem has.");
			SiegeGolemTweakerSpellPowerMultiplier = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerSpellPowerMultiplier), 100f, "The percentage of Spell Power a Siege Golem has.");
			SiegeGolemTweakerMovementSpeedMultiplier = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerMovementSpeedMultiplier), 100f, "The percentage of Movement Speed a Siege Golem has.");
			SiegeGolemTweakerAttackSpeedMultiplier = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerAttackSpeedMultiplier), 100f, "The percentage of Attack Speed a Siege Golem has.");
			SiegeGolemTweakerMaxHealthMultiplier = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerMaxHealthMultiplier), 100f, "The percentage of Max Health a Siege Golem has.");
			SiegeGolemTweakerPassiveHealthRegen = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerPassiveHealthRegen), float.NaN, "The (absolute) amount of Passive Health Regen a Siege Golem has. - Use NaN to disable");
			SiegeGolemTweakerPhysicalResistance = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerPhysicalResistance), 0.5f, "The (absolute) amount of Physical Resistance a Siege Golem has. - Use NaN to disable");
			SiegeGolemTweakerSpellResistance = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerPhysicalResistance), 0.5f, "The (absolute) amount of Spell Resistance a Siege Golem has. - Use NaN to disable");
			SiegeGolemTweakerFireResistance = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerFireResistance), float.NaN, "The (absolute) amount of Fire Resistance a Siege Golem has. - Use NaN to disable");
			SiegeGolemTweakerHolyResistance = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerHolyResistance), float.NaN, "The (absolute) amount of Holy Resistance a Siege Golem has. - Use NaN to disable");
			SiegeGolemTweakerSunResistance = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerSunResistance), float.NaN, "The (absolute) amount of Sun Resistance a Siege Golem has. - Use NaN to disable");
			SiegeGolemTweakerSilverResistance = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerSilverResistance), float.NaN, "The (absolute) amount of Silver Resistance a Siege Golem has. - Use NaN to disable");
			SiegeGolemTweakerGarlicResistance = config.Bind(nameof(SiegeGolemTweakerConfig), nameof(SiegeGolemTweakerGarlicResistance), float.NaN, "The (absolute) amount of Garlic Resistance a Siege Golem has. - Use NaN to disable");
		}

		#endregion
	}
}
