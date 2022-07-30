using BepInEx.Configuration;

namespace VMods.Shared
{
	public static class CommandSystemConfig
	{
		#region Properties

		public static ConfigEntry<bool> CommandSystemEnabled { get; private set; }
		public static ConfigEntry<string> CommandSystemPrefix { get; private set; }
		public static ConfigEntry<float> CommandSystemCommandCooldown { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			CommandSystemEnabled = config.Bind(nameof(CommandSystemConfig), nameof(CommandSystemEnabled), true, "Enabled/disable the Commands system (for this specific mod).");
			CommandSystemPrefix = config.Bind(nameof(CommandSystemConfig), nameof(CommandSystemPrefix), "!", "The prefix that needs to be used to execute a command (for this specific mod).");
			CommandSystemCommandCooldown = config.Bind(nameof(CommandSystemConfig), nameof(CommandSystemCommandCooldown), 5f, "The amount of seconds between two commands (for non-admins).");
		}

		#endregion
	}
}
