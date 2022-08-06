using BepInEx.Configuration;

namespace VMods.BloodRefill
{
	public static class BloodRefillConfig
	{
		#region Properties

		public static ConfigEntry<bool> BloodRefillEnabled { get; private set; }

		public static ConfigEntry<bool> BloodRefillRequiresFeeding { get; private set; }

		public static ConfigEntry<bool> BloodRefillRequiresSameBloodType { get; private set; }

		public static ConfigEntry<bool> BloodRefillExcludeVBloodFromSameBloodTypeCheck { get; private set; }

		public static ConfigEntry<float> BloodRefillDifferentBloodTypeMultiplier { get; private set; }

		public static ConfigEntry<int> BloodRefillVBloodRefillType { get; private set; }

		public static ConfigEntry<float> BloodRefillVBloodRefillMultiplier { get; private set; }

		public static ConfigEntry<bool> BloodRefillRandomRefill { get; private set; }

		public static ConfigEntry<float> BloodRefillAmount { get; private set; }

		public static ConfigEntry<float> BloodRefillMultiplier { get; private set; }

		public static ConfigEntry<bool> BloodRefillSendRefillMessage { get; private set; }

		#endregion

		#region Public Methods

		public static void Initialize(ConfigFile config)
		{
			BloodRefillEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillEnabled), false, "Enabled/disable the blood refilling system.");
			BloodRefillRequiresFeeding = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillRequiresFeeding), true, "When enabled, blood can only be refilled when feeding (i.e. when aborting the feed).");
			BloodRefillRequiresSameBloodType = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillRequiresSameBloodType), false, "When enabled, blood can only be refilled when the target has the same blood type.");
			BloodRefillExcludeVBloodFromSameBloodTypeCheck = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillExcludeVBloodFromSameBloodTypeCheck), true, "When enabled, V-blood is excluded from the 'same blood type' check (i.e. it's always considered to be 'the same blood type' as the player's blood type).");
			BloodRefillVBloodRefillType = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillVBloodRefillType), 2, "0 = disabled (i.e. normal refill); 1 = fully refill; 2 = refill based on V-blood monster level.");
			BloodRefillVBloodRefillMultiplier = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillVBloodRefillMultiplier), 0.1f, $"[Only applies when {nameof(BloodRefillVBloodRefillType)} is set to 2] The multiplier used in the v-blood refill calculation ('EnemyLevel' * '{nameof(BloodRefillVBloodRefillMultiplier)}' * '{nameof(BloodRefillMultiplier)}').");
			BloodRefillRandomRefill = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillRandomRefill), true, "When enabled, the amount of refilled blood is randomized (between 1 and the calculated refillable amount).");
			BloodRefillAmount = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillAmount), 2.0f, "The maximum amount of blood to refill with no level difference, a matching blood type and quality (Expressed in Litres of blood).");
			BloodRefillMultiplier = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillMultiplier), 1.0f, $"The multiplier used in the blood refill calculation. [Formula: (('Enemy Level' / 'Player Level') * ((100 - ('Player Blood Quality %' - 'Enemy Blood Quality %')) / 100)) * '{nameof(BloodRefillAmount)}' * '(If applicable) {nameof(BloodRefillDifferentBloodTypeMultiplier)}' * '{nameof(BloodRefillMultiplier)}']");
			BloodRefillDifferentBloodTypeMultiplier = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillDifferentBloodTypeMultiplier), 0.1f, $"The multiplier used in the blood refill calculation as a penalty for feeding on a different blood type (only works when {nameof(BloodRefillRequiresSameBloodType)} is disabled).");
			BloodRefillSendRefillMessage = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillSendRefillMessage), true, $"When enabled, a refill chat message is sent to the player.");
		}

		#endregion
	}
}
