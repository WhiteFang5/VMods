using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using VMods.Shared;
using Wetstone.API;

namespace VMods.ChestPvPProtection
{
	[HarmonyPatch]
	public static class InventoryHooks
	{
		#region Events

		public delegate void InventoryInteractionEventHandler(Entity eventEntity, VModCharacter vmodCharacter, params Entity[] inventoryEntities);
		public static event InventoryInteractionEventHandler InventoryInteractionEvent;
		private static void FireInventoryInteractionEvent(Entity eventEntity, VModCharacter vmodCharacter, params Entity[] inventoryEntities) => InventoryInteractionEvent?.Invoke(eventEntity, vmodCharacter, inventoryEntities);

		#endregion

		#region Private Methods

		[HarmonyPatch(typeof(MoveItemBetweenInventoriesSystem), nameof(MoveItemBetweenInventoriesSystem.OnUpdate))]
		[HarmonyPrefix]
		private static void MoveItemBetweenInventories(MoveItemBetweenInventoriesSystem __instance)
		{
			if(!VWorld.IsServer || __instance._MoveItemBetweenInventoriesEventQuery == null)
			{
				return;
			}

			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			var networkIdToEntityMap = __instance._NetworkIdSystem._NetworkIdToEntityMap;

			var entities = __instance._MoveItemBetweenInventoriesEventQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = new VModCharacter(entityManager.GetComponentData<FromCharacter>(entity), entityManager);
				var moveEvent = entityManager.GetComponentData<MoveItemBetweenInventoriesEvent>(entity);
				FireInventoryInteractionEvent(entity, fromCharacter, moveEvent.FromInventory.GetNetworkedEntity(networkIdToEntityMap)._Entity, moveEvent.ToInventory.GetNetworkedEntity(networkIdToEntityMap)._Entity);
			}
		}

		[HarmonyPatch(typeof(MoveAllItemsBetweenInventoriesSystem), nameof(MoveAllItemsBetweenInventoriesSystem.OnUpdate))]
		[HarmonyPrefix]
		private static void MoveAllItemsBetweenInventories(MoveAllItemsBetweenInventoriesSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__MoveAllItemsJob_entityQuery == null)
			{
				return;
			}

			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			var networkIdToEntityMap = __instance._NetworkIdSystem._NetworkIdToEntityMap;

			var entities = __instance.__MoveAllItemsJob_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = new VModCharacter(entityManager.GetComponentData<FromCharacter>(entity), entityManager);
				var moveEvent = entityManager.GetComponentData<MoveAllItemsBetweenInventoriesEvent>(entity);
				FireInventoryInteractionEvent(entity, fromCharacter, moveEvent.FromInventory.GetNetworkedEntity(networkIdToEntityMap)._Entity, moveEvent.ToInventory.GetNetworkedEntity(networkIdToEntityMap)._Entity);
			}
		}

		[HarmonyPatch(typeof(DropInventoryItemSystem), nameof(DropInventoryItemSystem.OnUpdate))]
		[HarmonyPrefix]
		private static void DropInventoryItem(DropInventoryItemSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__DropInventoryItemJob_entityQuery == null)
			{
				return;
			}

			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			var networkIdToEntityMap = __instance._NetworkIdSystem._NetworkIdToEntityMap;

			var entities = __instance.__DropInventoryItemJob_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = new VModCharacter(entityManager.GetComponentData<FromCharacter>(entity), entityManager);
				var dropEvent = entityManager.GetComponentData<DropInventoryItemEvent>(entity);
				FireInventoryInteractionEvent(entity, fromCharacter, dropEvent.Inventory.GetNetworkedEntity(networkIdToEntityMap)._Entity);
			}
		}

		[HarmonyPatch(typeof(SplitItemSystem), nameof(SplitItemSystem.OnUpdate))]
		[HarmonyPrefix]
		private static void SplitItem(SplitItemSystem __instance)
		{
			if(!VWorld.IsServer || __instance._Query == null)
			{
				return;
			}

			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			var networkIdToEntityMap = __instance._NetworkIdSystem._NetworkIdToEntityMap;

			var entities = __instance._Query.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = new VModCharacter(entityManager.GetComponentData<FromCharacter>(entity), entityManager);
				var splitEvent = entityManager.GetComponentData<SplitItemEvent>(entity);
				FireInventoryInteractionEvent(entity, fromCharacter, splitEvent.Inventory.GetNetworkedEntity(networkIdToEntityMap)._Entity);
			}
		}

		[HarmonyPatch(typeof(SortAllItemsSystem), nameof(SortAllItemsSystem.OnUpdate))]
		[HarmonyPrefix]
		private static void SortAllItems(SplitItemSystem __instance)
		{
			if(!VWorld.IsServer || __instance._Query == null)
			{
				return;
			}

			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			var networkIdToEntityMap = __instance._NetworkIdSystem._NetworkIdToEntityMap;

			var entities = __instance._Query.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = new VModCharacter(entityManager.GetComponentData<FromCharacter>(entity), entityManager);
				var sortEvent = entityManager.GetComponentData<SortAllItemsEvent>(entity);
				FireInventoryInteractionEvent(entity, fromCharacter, sortEvent.Inventory.GetNetworkedEntity(networkIdToEntityMap)._Entity);
			}
		}

		[HarmonyPatch(typeof(SmartMergeItemsBetweenInventoriesSystem), nameof(SmartMergeItemsBetweenInventoriesSystem.OnUpdate))]
		[HarmonyPrefix]
		private static void SmartMergeItemsBetweenInventories(SplitItemSystem __instance)
		{
			if(!VWorld.IsServer || __instance._Query == null)
			{
				return;
			}

			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			var networkIdToEntityMap = __instance._NetworkIdSystem._NetworkIdToEntityMap;

			var entities = __instance._Query.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				var fromCharacter = new VModCharacter(entityManager.GetComponentData<FromCharacter>(entity), entityManager);
				var mergeEvent = entityManager.GetComponentData<SmartMergeItemsBetweenInventoriesEvent>(entity);
				FireInventoryInteractionEvent(entity, fromCharacter, mergeEvent.FromInventory.GetNetworkedEntity(networkIdToEntityMap)._Entity, mergeEvent.ToInventory.GetNetworkedEntity(networkIdToEntityMap)._Entity);
			}
		}

		#endregion
	}
}
