using BepInEx.Configuration;

namespace VMods.ChestPvPProtection
{
	public static class ChestPvPProtectionSystemConfig
	{
		#region Properties

		public static ConfigEntry<bool> ChestPvPProtectionEnabled { get; private set; }

		public static ConfigEntry<bool> ChestPvPProtectionSendMessage { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			ChestPvPProtectionEnabled = config.Bind(nameof(ChestPvPProtectionSystemConfig), nameof(ChestPvPProtectionEnabled), false, "Enabled/disable the Chest PvP Protection system.");
			ChestPvPProtectionSendMessage = config.Bind(nameof(ChestPvPProtectionSystemConfig), nameof(ChestPvPProtectionSendMessage), true, "Enabled/disable the sending of a system message to the player with the PvP Protection buff that's attempting to interact with an enemy player's chest.");
		}

		#endregion
	}
}
