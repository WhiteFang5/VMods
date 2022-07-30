using System;
using System.IO;
using System.Text.Json;
using Wetstone.API;

namespace VMods.Shared
{
	public static class VModStorage
	{
		#region Consts

		public const string StoragePath = "BepInEx/config/VMods/Storage";

		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			WriteIndented = false,
			IncludeFields = false,
		};

		#endregion

		#region Events

		public delegate void SaveEventHandler();
		public static event SaveEventHandler SaveEvent;
		private static void FireSaveEvent() => SaveEvent?.Invoke();

		#endregion

		#region Public Methods

		public static void SaveAll() => FireSaveEvent();

		public static void Save<T>(string filename, T data)
		{
			try
			{
				File.WriteAllText(Path.Combine(StoragePath, filename), JsonSerializer.Serialize(data, JsonOptions));
#if DEBUG
				Utils.Logger.LogInfo($"{filename} has been saved.");
#endif
			}
			catch(Exception ex)
			{
				Utils.Logger.LogError($"Failed to save {filename}! - Error: {ex.Message}\r\n{ex.StackTrace}");
			}
		}

		public static T Load<T>(string filename, Func<T> getDefaultValue)
		{
			try
			{
				if(!Directory.Exists(StoragePath))
				{
					Directory.CreateDirectory(StoragePath);
				}
				var fullPath = Path.Combine(StoragePath, filename);
				if(!File.Exists(fullPath))
				{
					return getDefaultValue();
				}
				string json = File.ReadAllText(fullPath);
				return JsonSerializer.Deserialize<T>(json);
			}
			catch(Exception ex)
			{
				Utils.Logger.LogError($"Failed to load {filename}! - Error: {ex.Message}\r\n{ex.StackTrace}");
				return getDefaultValue();
			}
		}

		#endregion

		#region Private Methods

		[Command("saveall", "saveall", "Saves all data of all VMod plugins", true)]
		private static void OnSaveAllCommand(Command command)
		{
			SaveAll();
			command.User.SendSystemMessage($"VMod Plugin '{Utils.PluginName}' saved successfully.");
		}

		#endregion
	}
}
