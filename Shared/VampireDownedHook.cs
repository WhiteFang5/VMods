using HarmonyLib;
using ProjectM;
using Unity.Collections;
using Unity.Entities;
using Wetstone.API;

namespace VMods.Shared
{
	[HarmonyPatch]
	public static class VampireDownedHook
	{
		#region Events

		public delegate void VampireDownedEventHandler(Entity killer, Entity victim);
		public static event VampireDownedEventHandler VampireDownedEvent;
		private static void FireVampireDownedEvent(Entity killer, Entity victim) => VampireDownedEvent?.Invoke(killer, victim);

		#endregion

		#region Private Methods

		[HarmonyPatch(typeof(VampireDownedServerEventSystem), nameof(VampireDownedServerEventSystem.OnUpdate))]
		[HarmonyPostfix]
		private static void OnUpdate(VampireDownedServerEventSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery == null)
			{
				return;
			}

			EntityManager entityManager = __instance.EntityManager;
			var eventsQuery = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);

			foreach(var entity in eventsQuery)
			{
				VampireDownedServerEventSystem.TryFindRootOwner(entity, 1, entityManager, out var victim);
				Entity source = entityManager.GetComponentData<VampireDownedBuff>(entity).Source;
				VampireDownedServerEventSystem.TryFindRootOwner(source, 1, entityManager, out var killer);

				if(entityManager.HasComponent<PlayerCharacter>(killer) && entityManager.HasComponent<PlayerCharacter>(victim) && !killer.Equals(victim))
				{
					FireVampireDownedEvent(killer, victim);
				}
			}
		}

		#endregion
	}
}
