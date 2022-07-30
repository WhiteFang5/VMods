using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Reflection;
using VMods.Shared;
using Wetstone.API;

namespace VMods.ResourceStashWithdrawal
{
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[BepInDependency("xyz.molenzwiebel.wetstone")]
	[Reloadable]
	public class Plugin : BasePlugin
	{
		#region Variables

		private Harmony _hooks;

		#endregion

		#region Public Methods

		public sealed override void Load()
		{
			Utils.Initialize(Log, PluginInfo.PLUGIN_NAME);
			ResourceStashWithdrawalConfig.Initialize(Config);
			if(VWorld.IsClient)
			{
				UIClickHook.Reset();
			}

			ResourceStashWithdrawalSystem.Initialize();

			_hooks = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

			Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} (v{PluginInfo.PLUGIN_VERSION}) is loaded!");
		}

		public sealed override bool Unload()
		{
			_hooks?.UnpatchSelf();
			ResourceStashWithdrawalSystem.Deinitialize();
			Config.Clear();
			Utils.Deinitialize();
			return true;
		}

		#endregion
	}
}
