using BepInEx.Configuration;

namespace VMods.GenericChatCommands
{
	public static class MutePlayerChatConfig
	{
		#region Properties

		public static ConfigEntry<bool> GenericChatCommandsMutingEnabled { get; private set; }
		public static ConfigEntry<bool> GenericChatCommandsModsCanMute { get; private set; }
		public static ConfigEntry<bool> GenericChatCommandsAnnounceMutes { get; private set; }
		public static ConfigEntry<bool> GenericChatCommandsAnnounceUnmutes { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			GenericChatCommandsMutingEnabled = config.Bind(nameof(MutePlayerChatConfig), nameof(GenericChatCommandsMutingEnabled), true, "Enabled/disable the Mute Player Chat system.");
			GenericChatCommandsModsCanMute = config.Bind(nameof(MutePlayerChatConfig), nameof(GenericChatCommandsModsCanMute), true, "When enabled, players with the Moderator permission level can use the Mute Player Chat system commands.");
			GenericChatCommandsAnnounceMutes = config.Bind(nameof(MutePlayerChatConfig), nameof(GenericChatCommandsAnnounceMutes), true, "When enabled, the muting of players is announced server-wide.");
			GenericChatCommandsAnnounceUnmutes = config.Bind(nameof(MutePlayerChatConfig), nameof(GenericChatCommandsAnnounceUnmutes), true, "When enabled, the unmiting of players is announced server-wide.");
		}

		#endregion
	}
}
