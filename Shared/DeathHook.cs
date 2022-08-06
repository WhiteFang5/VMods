using HarmonyLib;
using ProjectM;
using Unity.Collections;
using Wetstone.API;

namespace VMods.Shared
{
	[HarmonyPatch]
	public static class DeathHook
	{
		#region Events

		public delegate void DeathEventHandler(DeathEvent deathEvent);
		public static event DeathEventHandler DeathEvent;
		private static void FireDeathEvent(DeathEvent deathEvent) => DeathEvent?.Invoke(deathEvent);

		#endregion

		#region Public Methods

		[HarmonyPatch(typeof(DeathEventListenerSystem), nameof(DeathEventListenerSystem.OnUpdate))]
		[HarmonyPostfix]
		private static void OnUpdate(DeathEventListenerSystem __instance)
		{
			if(!VWorld.IsServer || __instance._DeathEventQuery == null)
			{
				return;
			}

			NativeArray<DeathEvent> deathEvents = __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
			foreach(DeathEvent deathEvent in deathEvents)
			{
				FireDeathEvent(deathEvent);
			}
		}

		#endregion
	}
}
