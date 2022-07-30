using BepInEx.Configuration;

namespace VMods.ResourceStashWithdrawal
{
	public static class ResourceStashWithdrawalConfig
	{
		#region Properties

		public static ConfigEntry<bool> ResourceStashWithdrawalEnabled { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			ResourceStashWithdrawalEnabled = config.Bind("Server", nameof(ResourceStashWithdrawalEnabled), false, "Enabled/disable the resource stash withdrawal system.");
		}

		#endregion
	}
}
