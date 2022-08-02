using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Reflection;
using VMods.Shared;
using Wetstone.API;

namespace VMods.ChestPvPProtection
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
			if(VWorld.IsClient)
			{
				Log.LogMessage($"{PluginInfo.PLUGIN_NAME} only needs to be installed server side.");
				return;
			}
			Utils.Initialize(Log, PluginInfo.PLUGIN_NAME);

			ChestPvPProtectionSystemConfig.Initialize(Config);

			ChestPvPProtectionSystem.Initialize();

			_hooks = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

			Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} (v{PluginInfo.PLUGIN_VERSION}) is loaded!");
		}

		public sealed override bool Unload()
		{
			if(VWorld.IsClient)
			{
				return true;
			}
			_hooks?.UnpatchSelf();
			ChestPvPProtectionSystem.Deinitialize();
			Config.Clear();
			Utils.Deinitialize();
			return true;
		}

		#endregion
	}
}
