using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using VMods.Shared;
using Wetstone.API;

namespace VMods.RecoverEmptyContainers
{
	[HarmonyPatch]
	public class TestHook
	{
		#region Consts

		private static readonly Dictionary<PrefabGUID, PrefabGUID> RecipeItemToReturnedItemMapping = new()
		{
			[new PrefabGUID(-1322000172)] = new PrefabGUID(-810738866),// Water-filled Canteen -> Empty Canteen
			[new PrefabGUID(-1382451936)] = new PrefabGUID(-437611596),// Water-filled Bottle -> Empty Glass Bottle
		};

		#endregion

		#region Public Methods

		[HarmonyPatch(typeof(UseConsumableSystem), nameof(UseConsumableSystem.CastAbility))]
		[HarmonyPostfix]
		public static void CastAbility(UseConsumableSystem __instance, InventoryBuffer inventoryItem, FromCharacter fromCharacter, NativeHashMap<PrefabGUID, Entity> prefabLookupMap, Entity itemEntity, bool removeByItemEntity, ref bool shouldConsumeItem)
		{
			if(!VWorld.IsServer)
			{
				return;
			}

			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			var gameDataSystem = server.GetExistingSystem<GameDataSystem>();

			foreach(var kvp in gameDataSystem.RecipeHashLookupMap)
			{
				var recipeData = kvp.Value;
				if(entityManager.HasComponent<RecipeOutputBuffer>(recipeData.Entity))
				{
					var outputBuffer = entityManager.GetBuffer<RecipeOutputBuffer>(recipeData.Entity);
					foreach(var output in outputBuffer)
					{
						if(output.Guid == inventoryItem.ItemType)
						{
							if(entityManager.HasComponent<RecipeRequirementBuffer>(recipeData.Entity))
							{
								var requirements = entityManager.GetBuffer<RecipeRequirementBuffer>(recipeData.Entity);
								foreach(var requirement in requirements)
								{
									if(RecipeItemToReturnedItemMapping.TryGetValue(requirement.Guid, out var returnItemGUID))
									{
										Utils.TryGiveItem(entityManager, gameDataSystem.ItemHashLookupMap, fromCharacter.Character, returnItemGUID, requirement.Stacks, out _, out _, dropRemainder: true);
									}
								}
							}
						}
					}
				}
			}
		}

		#endregion
	}
}
