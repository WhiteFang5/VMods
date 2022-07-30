using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System.Collections.Generic;
using Unity.Collections;
using Wetstone.API;

namespace VMods.Shared
{
	[HarmonyPatch]
	public static class EquipmentHooks
	{
		#region Events

		public delegate void EquipmentChangedEventHandler(FromCharacter fromCharacter);
		public static event EquipmentChangedEventHandler EquipmentChangedEvent;
		private static void FireEquipmentChangedEvent(FromCharacter fromCharacter) => EquipmentChangedEvent?.Invoke(fromCharacter);

		#endregion

		#region Private Methods

		[HarmonyPatch(typeof(EquipItemSystem), nameof(EquipItemSystem.OnUpdate))]
		[HarmonyPostfix]
		private static void EquipItem(EquipItemSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery == null)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = entityManager.GetComponentData<FromCharacter>(entity);
				FireEquipmentChangedEvent(fromCharacter);
			}
		}

		[HarmonyPatch(typeof(EquipItemFromInventorySystem), nameof(EquipItemFromInventorySystem.OnUpdate))]
		[HarmonyPostfix]
		private static void EquipItemFromInventory(EquipItemFromInventorySystem __instance)
		{
			if(!VWorld.IsServer || __instance.__EquipItemFromInventoryJob_entityQuery == null)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__EquipItemFromInventoryJob_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = entityManager.GetComponentData<FromCharacter>(entity);
				FireEquipmentChangedEvent(fromCharacter);
			}
		}

		[HarmonyPatch(typeof(UnequipItemSystem), nameof(UnequipItemSystem.OnUpdate))]
		[HarmonyPostfix]
		private static void UnequipItem(UnequipItemSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery == null)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = entityManager.GetComponentData<FromCharacter>(entity);
				FireEquipmentChangedEvent(fromCharacter);
			}
		}

		[HarmonyPatch(typeof(MoveItemBetweenInventoriesSystem), nameof(MoveItemBetweenInventoriesSystem.OnUpdate))]
		private static class MoveItemBetweenInventories
		{
			private static void Prefix(MoveItemBetweenInventoriesSystem __instance, out List<FromCharacter> __state)
			{
				__state = new List<FromCharacter>();
				if(!VWorld.IsServer || __instance._MoveItemBetweenInventoriesEventQuery == null)
				{
					return;
				}

				var entityManager = VWorld.Server.EntityManager;
				var entities = __instance._MoveItemBetweenInventoriesEventQuery.ToEntityArray(Allocator.Temp);
				foreach(var entity in entities)
				{
					var fromCharacter = entityManager.GetComponentData<FromCharacter>(entity);
					if(!__state.Contains(fromCharacter))
					{
						__state.Add(fromCharacter);
					}
				}
			}

			private static void Postfix(List<FromCharacter> __state)
			{
				__state.ForEach(FireEquipmentChangedEvent);
			}
		}

		[HarmonyPatch(typeof(MoveAllItemsBetweenInventoriesSystem), nameof(MoveAllItemsBetweenInventoriesSystem.OnUpdate))]
		private static class MoveAllItemsBetweenInventories
		{
			private static void Prefix(MoveAllItemsBetweenInventoriesSystem __instance, out List<FromCharacter> __state)
			{
				__state = new List<FromCharacter>();
				if(!VWorld.IsServer || __instance.__MoveAllItemsJob_entityQuery == null)
				{
					return;
				}

				var entityManager = VWorld.Server.EntityManager;
				var entities = __instance.__MoveAllItemsJob_entityQuery.ToEntityArray(Allocator.Temp);
				foreach(var entity in entities)
				{
					var fromCharacter = entityManager.GetComponentData<FromCharacter>(entity);
					if(!__state.Contains(fromCharacter))
					{
						__state.Add(fromCharacter);
					}
				}
			}

			private static void Postfix(List<FromCharacter> __state)
			{
				__state.ForEach(FireEquipmentChangedEvent);
			}
		}

		[HarmonyPatch(typeof(DropInventoryItemSystem), nameof(DropInventoryItemSystem.OnUpdate))]
		[HarmonyPostfix]
		private static void DropInventoryItem(DropInventoryItemSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__DropInventoryItemJob_entityQuery == null)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__DropInventoryItemJob_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = entityManager.GetComponentData<FromCharacter>(entity);
				FireEquipmentChangedEvent(fromCharacter);
			}
		}

		[HarmonyPatch(typeof(DropItemSystem), nameof(DropItemSystem.OnUpdate))]
		private static class DropItem
		{
			private static void Prefix(DropItemSystem __instance, out List<FromCharacter> __state)
			{
				__state = new List<FromCharacter>();
				if(!VWorld.IsServer || __instance.__DropEquippedItemJob_entityQuery == null || __instance.__DropEquippedItemJob_entityQuery == null)
				{
					return;
				}

				var entityManager = VWorld.Server.EntityManager;
				var entities = __instance.__DropEquippedItemJob_entityQuery.ToEntityArray(Allocator.Temp);
				foreach(var entity in entities)
				{
					var fromCharacter = entityManager.GetComponentData<FromCharacter>(entity);
					if(!__state.Contains(fromCharacter))
					{
						__state.Add(fromCharacter);
					}
				}

				entities = __instance.__DropItemsJob_entityQuery.ToEntityArray(Allocator.Temp);
				foreach(var entity in entities)
				{
					var fromCharacter = entityManager.GetComponentData<FromCharacter>(entity);
					if(!__state.Contains(fromCharacter))
					{
						__state.Add(fromCharacter);
					}
				}
			}

			private static void Postfix(List<FromCharacter> __state)
			{
				__state.ForEach(FireEquipmentChangedEvent);
			}
		}

		[HarmonyPatch(typeof(ItemPickupSystem), nameof(ItemPickupSystem.OnUpdate))]
		[HarmonyPostfix]
		private static void ItemPickup(ItemPickupSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery == null)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var ownerData = entityManager.GetComponentData<EntityOwner>(entity);
				var characterEntity = ownerData.Owner;
				var playerCharacter = entityManager.GetComponentData<PlayerCharacter>(characterEntity);
				FireEquipmentChangedEvent(new FromCharacter()
				{
					Character = characterEntity,
					User = playerCharacter.UserEntity._Entity,
				});
			}
		}

		#endregion
	}
}
