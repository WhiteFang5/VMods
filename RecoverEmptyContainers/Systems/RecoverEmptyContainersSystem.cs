using ProjectM;
using ProjectM.Scripting;
using System.Collections.Generic;
using Unity.Entities;
using VMods.Shared;
using Wetstone.API;

namespace VMods.RecoverEmptyContainers
{
	public static class RecoverEmptyContainersSystem
	{
		#region Variables

		private static readonly Dictionary<PrefabGUID, (PrefabGUID itemGUID, int stackCount)> BuffToEmptyContainerMapping = new();

		private static readonly Dictionary<PrefabGUID, PrefabGUID> RecipeItemToReturnedItemMapping = new()
		{
			[new PrefabGUID(-1322000172)] = new PrefabGUID(-810738866),// Water-filled Canteen -> Empty Canteen
			[new PrefabGUID(-1382451936)] = new PrefabGUID(-437611596),// Water-filled Bottle -> Empty Glass Bottle
		};

		#endregion

		#region Public Methods

		public static void Initialize()
		{
			BuildBuffToEmptyContainerMapping();

			BuffSystemHook.ProcessBuffEvent += OnProcessBuffEvent;
		}

		public static void Deinitialize()
		{
			BuffSystemHook.ProcessBuffEvent -= OnProcessBuffEvent;

			BuffToEmptyContainerMapping.Clear();
		}

		#endregion

		#region Private Methods

		private static void OnProcessBuffEvent(Entity entity, PrefabGUID buffGUID)
		{
			var server = VWorld.Server;
			var entityManager = server.EntityManager;

			if(!entityManager.HasComponent<EntityCreator>(entity) || !entityManager.HasComponent<EntityOwner>(entity))
			{
				return;
			}

			var entityOwner = entityManager.GetComponentData<EntityOwner>(entity);
			var entityCreator = entityManager.GetComponentData<EntityCreator>(entity);
			var entityCreatorEntity = entityCreator.Creator._Entity;
			if(!entityManager.HasComponent<AbilityOwner>(entityCreatorEntity))
			{
				return;
			}

			var abilityOwner = entityManager.GetComponentData<AbilityOwner>(entityCreatorEntity);
			var abilityGroupEntity = abilityOwner.AbilityGroup._Entity;
			if(!entityManager.HasComponent<PrefabGUID>(abilityGroupEntity))
			{
				return;
			}

			var abilityGroupPrefabGUID = entityManager.GetComponentData<PrefabGUID>(abilityGroupEntity);
			if(!BuffToEmptyContainerMapping.TryGetValue(abilityGroupPrefabGUID, out var returnData))
			{
				return;
			}

			var gameDataSystem = server.GetExistingSystem<ServerScriptMapper>()._GameDataSystem;

			Utils.TryGiveItem(entityManager, gameDataSystem.ItemHashLookupMap, entityOwner.Owner, returnData.itemGUID, returnData.stackCount, out _, out _, dropRemainder: true);
		}

		private static void BuildBuffToEmptyContainerMapping()
		{
			BuffToEmptyContainerMapping.Clear();

			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			var gameDataSystem = server.GetExistingSystem<ServerScriptMapper>()._GameDataSystem;
			var prefabCollectionSystem = server.GetExistingSystem<PrefabCollectionSystem>();

			var duplicateBuffs = new List<PrefabGUID>();

			foreach(var recipeKvp in gameDataSystem.RecipeHashLookupMap)
			{
				var recipeEntity = recipeKvp.Value.Entity;
				if(!entityManager.HasComponent<RecipeRequirementBuffer>(recipeEntity) || !entityManager.HasComponent<RecipeOutputBuffer>(recipeEntity))
				{
					continue;
				}

				// Find the returned item
				PrefabGUID? returnItemGUID = null;
				int returnItemStackCount = 0;
				var requirementBuffer = entityManager.GetBuffer<RecipeRequirementBuffer>(recipeEntity);
				foreach(var requirement in requirementBuffer)
				{
					if(RecipeItemToReturnedItemMapping.TryGetValue(requirement.Guid, out var prefabGUID))
					{
						returnItemGUID = prefabGUID;
						returnItemStackCount = requirement.Stacks;
						break;
					}
				}

				// Check if we've found a returnable item
				if(!returnItemGUID.HasValue)
				{
					continue;
				}

				// Find the buff that belongs to this item
				var outputBuffer = entityManager.GetBuffer<RecipeOutputBuffer>(recipeEntity);
				foreach(var output in outputBuffer)
				{
					var outputEntity = prefabCollectionSystem.PrefabLookupMap[output.Guid];
					if(!entityManager.HasComponent<CastAbilityOnConsume>(outputEntity))
					{
						continue;
					}

					var castAbilityOnConsuime = entityManager.GetComponentData<CastAbilityOnConsume>(outputEntity);
					var abilityGUID = castAbilityOnConsuime.AbilityGuid;

#if DEBUG
					Utils.Logger.LogMessage($"Matched Buff {prefabCollectionSystem.PrefabNameLookupMap[abilityGUID]} -> {prefabCollectionSystem.PrefabNameLookupMap[returnItemGUID.Value]}");
#endif
					if(BuffToEmptyContainerMapping.ContainsKey(abilityGUID))
					{
						Utils.Logger.LogWarning($"Found duplicate Buff {prefabCollectionSystem.PrefabNameLookupMap[abilityGUID]}. Remove it!");
						duplicateBuffs.Add(abilityGUID);
					}
					BuffToEmptyContainerMapping[abilityGUID] = (returnItemGUID.Value, returnItemStackCount);
				}
			}

			duplicateBuffs.ForEach(x => BuffToEmptyContainerMapping.Remove(x));
		}

		#endregion
	}
}
