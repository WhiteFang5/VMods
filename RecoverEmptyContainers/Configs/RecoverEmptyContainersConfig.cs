using BepInEx.Configuration;

namespace VMods.RecoverEmptyContainers
{
	public static class RecoverEmptyContainersConfig
	{
		#region Properties

		public static ConfigEntry<bool> RecoverEmptyContainersEnabled { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			RecoverEmptyContainersEnabled = config.Bind("Server", nameof(RecoverEmptyContainersEnabled), false, "Enabled/disable the recovery of empty containers system.");
		}

		#endregion
	}
}
