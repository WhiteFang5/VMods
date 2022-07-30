using ProjectM;
using ProjectM.Network;
using ProjectM.Scripting;
using System;
using Unity.Entities;
using VMods.Shared;
using Wetstone.API;

namespace VMods.ResourceStashWithdrawal
{
	public static class ResourceStashWithdrawalSystem
	{
		#region Public Methods

		public static void Initialize()
		{
			VNetworkRegistry.RegisterServerboundStruct<ResourceStashWithdrawalRequest>(OnResourceStashWithdrawalRequest);
		}

		public static void Deinitialize()
		{
			VNetworkRegistry.UnregisterStruct<ResourceStashWithdrawalRequest>();
		}

		private static void OnResourceStashWithdrawalRequest(FromCharacter fromCharacter, ResourceStashWithdrawalRequest request)
		{
			if(!VWorld.IsServer || fromCharacter.Character == Entity.Null)
			{
				// This isn't running on a server, or a non-existing character made the request -> stop trying to move items
				return;
			}

			var server = VWorld.Server;
			var gameManager = server.GetExistingSystem<ServerScriptMapper>()?._ServerGameManager;
			var teamChecker = gameManager._TeamChecker;
			var gameDataSystem = server.GetExistingSystem<GameDataSystem>();
			var itemHashLookupMap = gameDataSystem.ItemHashLookupMap;
			var prefabCollectionSystem = server.GetExistingSystem<PrefabCollectionSystem>();
			var prefabLookupMap = prefabCollectionSystem.PrefabLookupMap;
			var entityManager = server.EntityManager;

			if(!InventoryUtilities.TryGetInventoryEntity(entityManager, fromCharacter.Character, out Entity playerInventory) || playerInventory == Entity.Null)
			{
				// Player inventory couldn't be found -> stop trying to move items
				return;
			}

			var remainingAmount = request.Amount;

			var stashes = Utils.GetAlliedStashes(entityManager, teamChecker, fromCharacter.Character);
			foreach(var stash in stashes)
			{
				var stashInventory = entityManager.GetBuffer<InventoryBuffer>(stash);

				for(int i = 0; i < stashInventory.Length; i++)
				{
					var stashItem = stashInventory[i];

					// Only withdraw the requested item
					if(stashItem.ItemType.GuidHash != request.ItemGUIDHash)
					{
						continue;
					}

					var transferAmount = Math.Min(remainingAmount, stashItem.Stacks);
					if(!Utils.TryGiveItem(entityManager, itemHashLookupMap, playerInventory, stashItem.ItemType, transferAmount, out int remainingStacks, out _))
					{
						// Failed to add the item(s) to the player's inventory -> stop trying to move any items at all
						return;
					}
					transferAmount -= remainingStacks;
					if(!InventoryUtilitiesServer.TryRemoveItem(entityManager, stash, stashItem.ItemType, transferAmount))
					{
						// Failed to remove the item from the stash -> Remove the items from the player's inventory & stop trying to move any items at all
						InventoryUtilitiesServer.TryRemoveItem(entityManager, playerInventory, stashItem.ItemType, transferAmount);
						return;
					}

					InventoryUtilitiesServer.CreateInventoryChangedEvent(entityManager, fromCharacter.Character, stashItem.ItemType, stashItem.Stacks, InventoryChangedEventType.Moved);
					remainingAmount -= transferAmount;
					if(remainingAmount <= 0)
					{
						break;
					}
				}

				if(remainingAmount <= 0)
				{
					break;
				}
			}

			if(remainingAmount > 0)
			{
				var name = Utils.GetItemName(new PrefabGUID(request.ItemGUIDHash), gameDataSystem, entityManager, prefabLookupMap);
				if(remainingAmount == request.Amount)
				{
					Utils.SendMessage(fromCharacter.User, $"Couldn't find any {name} in the stash(es).", ServerChatMessageType.System);
				}
				else
				{
					Utils.SendMessage(fromCharacter.User, $"Couldn't find all {name} in the stash(es). {remainingAmount} {(remainingAmount == 1 ? "is" : "are")} missing.", ServerChatMessageType.System);
				}
			}
		}

		#endregion
	}
}
