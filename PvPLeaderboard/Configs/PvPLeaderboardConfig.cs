using BepInEx.Configuration;

namespace VMods.PvPLeaderboard
{
	public static class PvPLeaderboardConfig
	{
		#region Properties

		public static ConfigEntry<bool> PvPLeaderboardEnabled { get; private set; }
		public static ConfigEntry<int> PvPLeaderboardLevelDifference { get; private set; }
		public static ConfigEntry<bool> PvPLeaderboardAnnounceKill { get; private set; }
		public static ConfigEntry<bool> PvPLeaderboardAnnounceLowLevelKill { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			PvPLeaderboardEnabled = config.Bind(nameof(PvPLeaderboardConfig), nameof(PvPLeaderboardEnabled), false, "Enabled/disable the PvP Leaderboard system.");
			PvPLeaderboardLevelDifference = config.Bind(nameof(PvPLeaderboardConfig), nameof(PvPLeaderboardLevelDifference), 10, "The level difference at which the K/D isn't counting anymore for the leaderboard.");
			PvPLeaderboardAnnounceKill = config.Bind(nameof(PvPLeaderboardConfig), nameof(PvPLeaderboardAnnounceKill), true, "When enabled, a legitemate kill is announced server-wide.");
			PvPLeaderboardAnnounceLowLevelKill = config.Bind(nameof(PvPLeaderboardConfig), nameof(PvPLeaderboardAnnounceLowLevelKill), false, "When enabled, a kill of a lower level player is announced server-wide.");
		}

		#endregion
	}
}
