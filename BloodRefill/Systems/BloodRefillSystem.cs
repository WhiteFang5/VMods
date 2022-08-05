using ProjectM;
using ProjectM.Network;
using System;
using Unity.Entities;
using VMods.Shared;
using Wetstone.API;
using Random = UnityEngine.Random;

namespace VMods.BloodRefill
{
	public static class BloodRefillSystem
	{
		#region Public Methods

		public static void Initialize()
		{
			DeathHook.DeathEvent += OnDeath;
		}

		public static void Deinitialize()
		{
			DeathHook.DeathEvent -= OnDeath;
		}

		#endregion

		#region Private Methods

		private static void OnDeath(DeathEvent deathEvent)
		{
			EntityManager entityManager = VWorld.Server.EntityManager;

			// Make sure a player killed an appropriate monster
			if(!BloodRefillConfig.BloodRefillEnabled.Value ||
				!entityManager.HasComponent<PlayerCharacter>(deathEvent.Killer) ||
				!entityManager.HasComponent<Equipment>(deathEvent.Killer) ||
				!entityManager.HasComponent<Blood>(deathEvent.Killer) ||
				!entityManager.HasComponent<Movement>(deathEvent.Died) ||
				!entityManager.HasComponent<UnitLevel>(deathEvent.Died) ||
				!entityManager.HasComponent<BloodConsumeSource>(deathEvent.Died))
			{
				return;
			}

			PlayerCharacter playerCharacter = entityManager.GetComponentData<PlayerCharacter>(deathEvent.Killer);
			Equipment playerEquipment = entityManager.GetComponentData<Equipment>(deathEvent.Killer);
			Blood playerBlood = entityManager.GetComponentData<Blood>(deathEvent.Killer);
			UnitLevel unitLevel = entityManager.GetComponentData<UnitLevel>(deathEvent.Died);
			BloodConsumeSource bloodConsumeSource = entityManager.GetComponentData<BloodConsumeSource>(deathEvent.Died);

#if DEBUG
			Utils.Logger.LogMessage($"DE.Killer = {deathEvent.Killer.Index}");
			Utils.Logger.LogMessage($"DE.Died = {deathEvent.Died.Index}");
			Utils.Logger.LogMessage($"DE.Source = {deathEvent.Source.Index}");
#endif

			Entity userEntity = playerCharacter.UserEntity._Entity;
			User user = entityManager.GetComponentData<User>(userEntity);

			bool killedByFeeding = deathEvent.Killer.Index == deathEvent.Source.Index;

			if(!playerBlood.BloodType.ParseBloodType(out BloodType playerBloodType))
			{
				// Invalid/unknown blood type
				return;
			}

			if(!bloodConsumeSource.UnitBloodType.ParseBloodType(out BloodType bloodType))
			{
				// Invalid/unknown blood type
				return;
			}

			bool isVBlood = bloodType == BloodType.VBlood;

			// Allow V-Bloods to skip the 'killed by feeding' check, otherwise additional feeders won't get a refill.
			if(!isVBlood && BloodRefillConfig.BloodRefillRequiresFeeding.Value && !killedByFeeding)
			{
				// Can only gain blood when killing the enemy while feeding (i.e. abort the feed)
				return;
			}

			bool isSameBloodType = playerBloodType == bloodType || (BloodRefillConfig.BloodRefillExcludeVBloodFromSameBloodTypeCheck.Value && isVBlood);

			if(BloodRefillConfig.BloodRefillRequiresSameBloodType.Value && !isSameBloodType)
			{
				// Can only gain blood when killing an enemy of the same blood type
				return;
			}

			float bloodTypeMultiplier = isSameBloodType ? 1f : BloodRefillConfig.BloodRefillDifferentBloodTypeMultiplier.Value;

			float playerLevel = playerEquipment.WeaponLevel + playerEquipment.ArmorLevel + playerEquipment.SpellLevel;
			float enemyLevel = unitLevel.Level;

#if DEBUG
			Utils.Logger.LogMessage($"Player Blood Quality: {playerBlood.Quality}");
			Utils.Logger.LogMessage($"Player Blood Value: {playerBlood.Value}");
			Utils.Logger.LogMessage($"Player Level: {playerLevel}");

			Utils.Logger.LogMessage($"Enemy Blood Quality: {bloodConsumeSource.BloodQuality}");
			Utils.Logger.LogMessage($"Enemy Level {enemyLevel}");
#endif

			float levelRatio = enemyLevel / playerLevel;

			float qualityRatio = (100f - (playerBlood.Quality - bloodConsumeSource.BloodQuality)) / 100f;

			float refillRatio = levelRatio * qualityRatio;

			// Config amount is expressed in 'Litres of blood' -> the game's formule is 'blood value / 10', hence the * 10 multiplier here.
			float refillAmount = BloodRefillConfig.BloodRefillAmount.Value * 10f * refillRatio;

			refillAmount *= bloodTypeMultiplier;

#if DEBUG
			Utils.Logger.LogMessage($"Lvl Ratio: {levelRatio}");
			Utils.Logger.LogMessage($"Quality Ratio: {qualityRatio}");
			Utils.Logger.LogMessage($"Refill Ratio: {refillRatio}");
			Utils.Logger.LogMessage($"Blood Type Multiplier: {bloodTypeMultiplier}");
			Utils.Logger.LogMessage($"Refill Amount: {refillAmount}");
#endif

			if(BloodRefillConfig.BloodRefillRandomRefill.Value)
			{
				refillAmount = Random.RandomRange(1f, refillAmount);

#if DEBUG
				Utils.Logger.LogMessage($"Refill Roll: {refillAmount}");
#endif
			}

			if(isVBlood)
			{
				switch(BloodRefillConfig.BloodRefillVBloodRefillType.Value)
				{
					case 1: // V-blood fully refills the blood pool
						refillAmount = playerBlood.MaxBlood - playerBlood.Value;
						break;

					case 2: // V-blood refills based on the unit's level
						refillAmount = enemyLevel * BloodRefillConfig.BloodRefillVBloodRefillMultiplier.Value;
						break;
				}
			}

			refillAmount *= BloodRefillConfig.BloodRefillMultiplier.Value;

			if(refillAmount > 0f)
			{
				int roundedRefillAmount = (int)Math.Ceiling(refillAmount);

				if(roundedRefillAmount > 0)
				{
#if DEBUG
					Utils.Logger.LogMessage($"New Blood Amount: {playerBlood.Value + roundedRefillAmount}");
#endif

					float newTotalBlood = Math.Min(playerBlood.MaxBlood, playerBlood.Value + roundedRefillAmount);
					float actualBloodGained = newTotalBlood - playerBlood.Value;
					float refillAmountInLitres = (int)(actualBloodGained * 10f) / 100f;
					float newTotalBloodInLitres = (int)Math.Round(newTotalBlood) / 10f;
					Utils.SendMessage(userEntity, $"+{refillAmountInLitres}L Blood ({newTotalBloodInLitres}L)", ServerChatMessageType.Lore);

					playerBloodType.ApplyToPlayer(user, playerBlood.Quality, roundedRefillAmount);
					return;
				}
			}

			Utils.SendMessage(userEntity, $"No blood gained from the enemy.", ServerChatMessageType.Lore);
		}

		#endregion
	}
}
