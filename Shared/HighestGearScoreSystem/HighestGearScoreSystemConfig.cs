using BepInEx.Configuration;

namespace VMods.Shared
{
	public static class HighestGearScoreSystemConfig
	{
		#region Properties

		public static ConfigEntry<bool> HighestGearScoreSystemEnabled { get; private set; }
		public static ConfigEntry<float> HighestGearScoreDuration { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			HighestGearScoreSystemEnabled = config.Bind(nameof(HighestGearScoreSystemConfig), nameof(HighestGearScoreSystemEnabled), true, "Enabled/disable the Highest Gear Score system (for this specific mod).");
			HighestGearScoreDuration = config.Bind(nameof(HighestGearScoreSystemConfig), nameof(HighestGearScoreDuration), 600f, "The amount of seconds the highest gear score is remembered/stored.");
		}

		#endregion
	}
}
