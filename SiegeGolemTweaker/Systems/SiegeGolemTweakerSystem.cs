using ProjectM;
using Unity.Entities;
using VMods.Shared;
using Wetstone.API;

namespace VMods.SiegeGolemTweaker
{
	public static class SiegeGolemTweakerSystem
	{
		#region Public Methods

		public static void Initialize()
		{
			BuffSystemHook.ProcessBuffEvent += OnProcessBuff;
		}

		public static void Deinitialize()
		{
			BuffSystemHook.ProcessBuffEvent -= OnProcessBuff;
		}

		#endregion

		#region Private Methods

		private static void OnProcessBuff(Entity entity, PrefabGUID buffGUID)
		{
			if(!VWorld.IsServer ||
				!SiegeGolemTweakerConfig.SiegeGolemTweakerEnabled.Value ||
				(buffGUID != Utils.SiegeGolemT01 && buffGUID != Utils.SiegeGolemT02))
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;

			var buffer = entityManager.AddBuffer<ModifyUnitStatBuff_DOTS>(entity);
			TryAddReductionBuff(buffer, UnitStatType.SiegePower, ModificationType.Multiply, SiegeGolemTweakerConfig.SiegeGolemTweakerSiegePowerMultiplier.Value);
			TryAddReductionBuff(buffer, UnitStatType.PhysicalPower, ModificationType.Multiply, SiegeGolemTweakerConfig.SiegeGolemTweakerPhysicalPowerMultiplier.Value);
			TryAddReductionBuff(buffer, UnitStatType.SpellPower, ModificationType.Multiply, SiegeGolemTweakerConfig.SiegeGolemTweakerSpellPowerMultiplier.Value);
			TryAddReductionBuff(buffer, UnitStatType.MovementSpeed, ModificationType.Multiply, SiegeGolemTweakerConfig.SiegeGolemTweakerMovementSpeedMultiplier.Value);
			TryAddReductionBuff(buffer, UnitStatType.AttackSpeed, ModificationType.Multiply, SiegeGolemTweakerConfig.SiegeGolemTweakerAttackSpeedMultiplier.Value);
			TryAddReductionBuff(buffer, UnitStatType.MaxHealth, ModificationType.Multiply, SiegeGolemTweakerConfig.SiegeGolemTweakerMaxHealthMultiplier.Value);
			TryAddReductionBuff(buffer, UnitStatType.PassiveHealthRegen, ModificationType.Set, SiegeGolemTweakerConfig.SiegeGolemTweakerPassiveHealthRegen.Value);
			TryAddReductionBuff(buffer, UnitStatType.PhysicalResistance, ModificationType.Set, SiegeGolemTweakerConfig.SiegeGolemTweakerPhysicalResistance.Value);
			TryAddReductionBuff(buffer, UnitStatType.SpellResistance, ModificationType.Set, SiegeGolemTweakerConfig.SiegeGolemTweakerSpellResistance.Value);
			TryAddReductionBuff(buffer, UnitStatType.FireResistance, ModificationType.Set, SiegeGolemTweakerConfig.SiegeGolemTweakerFireResistance.Value);
			TryAddReductionBuff(buffer, UnitStatType.HolyResistance, ModificationType.Set, SiegeGolemTweakerConfig.SiegeGolemTweakerHolyResistance.Value);
			TryAddReductionBuff(buffer, UnitStatType.SunResistance, ModificationType.Set, SiegeGolemTweakerConfig.SiegeGolemTweakerSunResistance.Value);
			TryAddReductionBuff(buffer, UnitStatType.SilverResistance, ModificationType.Set, SiegeGolemTweakerConfig.SiegeGolemTweakerSilverResistance.Value);
			TryAddReductionBuff(buffer, UnitStatType.GarlicResistance, ModificationType.Set, SiegeGolemTweakerConfig.SiegeGolemTweakerGarlicResistance.Value);
		}

		private static void TryAddReductionBuff(DynamicBuffer<ModifyUnitStatBuff_DOTS> buffer, UnitStatType unitStatType, ModificationType modificationType, float value)
		{
			if(!float.IsNaN(value))
			{
				buffer.Add(new ModifyUnitStatBuff_DOTS()
				{
					StatType = unitStatType,
					Value = modificationType switch
					{
						ModificationType.Multiply => value / 100f,
						ModificationType.Set => value,
						_ => value,
					},
					ModificationType = modificationType,
					Id = ModificationId.NewId(0),
				});
			}
		}

		#endregion
	}
}
