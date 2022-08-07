using BepInEx.Configuration;

namespace VMods.PvELeaderboard
{
	public static class PvELeaderboardConfig
	{
		#region Properties

		public static ConfigEntry<bool> PvELeaderboardEnabled { get; private set; }
		public static ConfigEntry<int> PvELeaderboardLevelDifference { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			PvELeaderboardEnabled = config.Bind(nameof(PvELeaderboardConfig), nameof(PvELeaderboardEnabled), false, "Enabled/disable the PvE Leaderboard system.");
			PvELeaderboardLevelDifference = config.Bind(nameof(PvELeaderboardConfig), nameof(PvELeaderboardLevelDifference), 10, "The level difference at which the K/D isn't counting anymore for the leaderboard.");
		}

		#endregion
	}
}
