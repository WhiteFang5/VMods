using BepInEx.Configuration;

namespace VMods.GenericChatCommands
{
	public static class GenericChatCommandsConfig
	{
		#region Properties

		public static ConfigEntry<bool> GenericChatCommandsAnnounceRename { get; private set; }
		public static ConfigEntry<bool> GenericChatCommandsAnnounceBloodMoonSkip { get; private set; }
		public static ConfigEntry<bool> GenericChatCommandsAllowNextBloodMoonServerWideAnnouncing { get; private set; }
		public static ConfigEntry<bool> GenericChatCommandsAnnounceGlobalCommandChanges { get; private set; }
		public static ConfigEntry<bool> GenericChatCommandsAnnounceAdminLevelChanges { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			GenericChatCommandsAnnounceRename = config.Bind(nameof(GenericChatCommandsConfig), nameof(GenericChatCommandsAnnounceRename), true, "When enabled, the renaming of players is announced server-wide.");
			GenericChatCommandsAnnounceBloodMoonSkip = config.Bind(nameof(GenericChatCommandsConfig), nameof(GenericChatCommandsAnnounceBloodMoonSkip), true, "When enabled, the skipping to the next bloodmoon is announced server-wide.");
			GenericChatCommandsAllowNextBloodMoonServerWideAnnouncing = config.Bind(nameof(GenericChatCommandsConfig), nameof(GenericChatCommandsAllowNextBloodMoonServerWideAnnouncing), true, "When enabled, it is possible to announce the time until the next blood moon server-wide.");
			GenericChatCommandsAnnounceGlobalCommandChanges = config.Bind(nameof(GenericChatCommandsConfig), nameof(GenericChatCommandsAnnounceGlobalCommandChanges), true, "When enabled, all the commands starting with 'global-' will be announced server-wide.");
			GenericChatCommandsAnnounceAdminLevelChanges = config.Bind(nameof(GenericChatCommandsConfig), nameof(GenericChatCommandsAnnounceAdminLevelChanges), true, "When enabled, all the changes made to a player's admin-level will be announced server-wide.");
		}

		#endregion
	}
}
