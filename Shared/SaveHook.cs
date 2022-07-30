using HarmonyLib;
using ProjectM;

namespace VMods.Shared
{
	[HarmonyPatch]
	public static class SaveHook
	{
		#region Private Methods

		[HarmonyPatch(typeof(TriggerPersistenceSaveSystem), nameof(TriggerPersistenceSaveSystem.TriggerSave))]
		[HarmonyPrefix]
		private static void TriggerSave()
		{
			VModStorage.SaveAll();
		}

		[HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnDestroy))]
		[HarmonyPrefix]
		private static void OnDestroy()
		{
			VModStorage.SaveAll();
		}

		#endregion
	}
}
