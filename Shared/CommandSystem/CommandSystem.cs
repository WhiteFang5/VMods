using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wetstone.API;
using Wetstone.Hooks;
using static VMods.Shared.CommandAttribute;

namespace VMods.Shared
{
	public static class CommandSystem
	{
		#region Consts

		private const char CommandSplitChar = ' ';

		#endregion

		#region Variables

		private static readonly Dictionary<ulong, DateTime> _lastUsedCommandTimes = new();

		private static List<(MethodInfo method, CommandAttribute attribute)> _commandReflectionMethods;
		private static readonly List<(string id, Action<Command> method, CommandAttribute attribute)> _commandMethods = new();

		#endregion

		#region Public Methods

		public static void Initialize()
		{
			if(!VWorld.IsServer)
			{
				Utils.Logger.LogMessage($"{nameof(CommandSystem)} only needs to be called server-side.");
				return;
			}

			_commandReflectionMethods = Assembly.GetExecutingAssembly().GetTypes().SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(y => y.GetCustomAttributes<CommandAttribute>(false).Count() > 0)).Select(x => (x, x.GetCustomAttribute<CommandAttribute>(false))).ToList();

			Chat.OnChatMessage += OnChatMessage;
		}

		public static void Deinitialize()
		{
			Chat.OnChatMessage -= OnChatMessage;
			_commandReflectionMethods?.Clear();
			_commandMethods.Clear();
		}

		public static void RegisterCommand(string uniqueId, Action<Command> commandMethod, CommandAttribute commandAttribute)
		{
			_commandMethods.Add((uniqueId, commandMethod, commandAttribute));
		}

		public static void UnregisterCommand(string uniqueId)
		{
			_commandMethods.RemoveAll(x => x.id == uniqueId);
		}

		public static void UnregisterCommand(Action<Command> commandMethod, CommandAttribute commandAttribute)
		{
			_commandMethods.RemoveAll(x => x.method == commandMethod && x.attribute == commandAttribute);
		}

		public static void SendInvalidCommandMessage(Command command, bool invalidArgument = false)
		{
			SendInvalidCommandMessage(command.VModCharacter, invalidArgument);
		}

		public static void SendInvalidCommandMessage(VModCharacter vmodCharacter, bool invalidArgument = false)
		{
			vmodCharacter.SendSystemMessage($"<color=#ff0000>Invalid command{(invalidArgument ? " argument" : string.Empty)}. Check {CommandSystemConfig.CommandSystemPrefix.Value}help [<command>] for more information.</color>");
		}

		#endregion

		#region Private Methods

		private static void OnChatMessage(VChatEvent chatEvent)
		{
			if(!CommandSystemConfig.CommandSystemEnabled.Value)
			{
				return;
			}
			if(chatEvent.Cancelled)
			{
				return;
			}
			string commandPrefix = CommandSystemConfig.CommandSystemPrefix.Value;
			string message = chatEvent.Message;
			if(chatEvent.Cancelled || !message.StartsWith(commandPrefix, StringComparison.Ordinal))
			{
				return;
			}

			PruneCommandTimes();

			// Extract the command name and arguments
			string name;
			string[] args;
			if(message.Contains(CommandSplitChar))
			{
				var splitted = message.Split(CommandSplitChar);
				name = splitted[0][commandPrefix.Length..];
				args = splitted.Skip(1).ToArray();
			}
			else
			{
				name = message[commandPrefix.Length..];
				args = new string[0];
			}

			// Anti-spam for non-admins
			var user = chatEvent.User;
			if(!user.IsAdmin)
			{
				if(_lastUsedCommandTimes.TryGetValue(user.PlatformId, out var lastUsedCommandTime))
				{
					var timeDiff = DateTime.UtcNow.Subtract(lastUsedCommandTime);
					if(timeDiff.TotalSeconds < CommandSystemConfig.CommandSystemCommandCooldown.Value)
					{
						int waitTime = (int)Math.Ceiling(CommandSystemConfig.CommandSystemCommandCooldown.Value - timeDiff.TotalSeconds);
						user.SendSystemMessage($"<color=#ff0000>Please wait for {waitTime} second(s) before sending another command.</color>");
						chatEvent.Cancel();
						return;
					}
				}
				_lastUsedCommandTimes[user.PlatformId] = DateTime.UtcNow;
			}

			// Fire the command (so an event handler can actually handle/execute it)
			Command command = new(new VModCharacter(chatEvent.SenderUserEntity, chatEvent.SenderCharacterEntity), name, args);

			foreach((_, var method, var attribute) in _commandMethods)
			{
				if(!attribute.Names.Contains(command.Name) || !command.VModCharacter.AdminLevel.HasReqLevel(attribute.ReqAdminLevel))
				{
					continue;
				}
				try
				{
					method.Invoke(command);
				}
				catch(Exception ex)
				{
					SendInvalidCommandMessage(command);
					throw ex;
				}
				if(command.Used)
				{
					break;
				}
			}

			if(!command.Used)
			{
				foreach((var method, var attribute) in _commandReflectionMethods)
				{
					if(!attribute.Names.Contains(command.Name) || !command.VModCharacter.AdminLevel.HasReqLevel(attribute.ReqAdminLevel))
					{
						continue;
					}
					try
					{
						method.Invoke(null, new[] { command });
					}
					catch(Exception ex)
					{
						SendInvalidCommandMessage(command);
						throw ex;
					}
					if(command.Used)
					{
						break;
					}
				}
			}

			if(command.Used)
			{
				chatEvent.Cancel();
			}
		}

		private static void PruneCommandTimes()
		{
			var now = DateTime.UtcNow;
			var keys = _lastUsedCommandTimes.Keys.ToList();
			var cooldown = CommandSystemConfig.CommandSystemCommandCooldown.Value;
			foreach(var key in keys)
			{
				var lastUsedCommandTime = _lastUsedCommandTimes[key];
				if(now.Subtract(lastUsedCommandTime).TotalSeconds >= cooldown)
				{
					_lastUsedCommandTimes.Remove(key);
				}
			}
		}

		[Command("help", "help [<command>]", "Shows a list of commands, or details about a command.")]
		private static void OnHelpCommand(Command command)
		{
			var commandPrefix = CommandSystemConfig.CommandSystemPrefix.Value;
			var vmodCharacter = command.VModCharacter;
			switch(command.Args.Length)
			{
				case 0:
					{
						vmodCharacter.SendSystemMessage($"List of {Utils.PluginName} commands:");
						_commandMethods.ForEach(x => SendCommandInfo(x.attribute));
						_commandReflectionMethods.ForEach(x => SendCommandInfo(x.attribute));

						// Nested Method(s)
						void SendCommandInfo(CommandAttribute attribute)
						{
							if(!vmodCharacter.AdminLevel.HasReqLevel(attribute.ReqAdminLevel))
							{
								return;
							}
							string message = $"<color=#00ff00>{string.Join(", ", attribute.Names.Select(x => $"{commandPrefix}{x}"))}</color>";
							switch(attribute.ReqAdminLevel)
							{
								case AdminLevel.Moderator:
									message += " - <color=#FFA500>[MOD]</color>";
									break;
								case AdminLevel.Admin:
									message += " - <color=#ff0000>[ADMIN]</color>";
									break;
								case AdminLevel.SuperAdmin:
									message += " - <color=#ff0000>[SUPER-ADMIN]</color>";
									break;
							}
							message += $" - <color=#ffffff>{attribute.Description}</color>";
							vmodCharacter.SendSystemMessage(message);
						}
					}
					break;

				case 1:
					{
						string searchCommandName = command.Args[0];

						// Find the command info
						CommandAttribute attribute = null;
						if(_commandMethods.Exists(x => x.attribute.Names.Contains(searchCommandName)))
						{
							attribute = _commandMethods.Find(x => x.attribute.Names.Contains(searchCommandName)).attribute;
						}
						else if(_commandReflectionMethods.Exists(x => x.attribute.Names.Contains(searchCommandName)))
						{
							attribute = _commandReflectionMethods.Find(x => x.attribute.Names.Contains(searchCommandName)).attribute;
						}

						// Check the found info
						if(attribute == null || !vmodCharacter.AdminLevel.HasReqLevel(attribute.ReqAdminLevel))
						{
							return;
						}

						vmodCharacter.SendSystemMessage($"Help for <color=#00ff00>{commandPrefix}{attribute.Names[0]}</color>");
						if(attribute.Names.Count > 1)
						{
							vmodCharacter.SendSystemMessage($"<color=#ffffff>Aliases: {string.Join(", ", attribute.Names.Skip(1).Select(x => $"{commandPrefix}{x}"))}</color>");
						}
						vmodCharacter.SendSystemMessage($"<color=#ffffff>Description: {attribute.Description}</color>");
						vmodCharacter.SendSystemMessage($"<color=#ffffff>Usage: {commandPrefix}{attribute.Usage}</color>");
					}
					return;

				default:
					SendInvalidCommandMessage(command);
					return;
			}
		}

		#endregion
	}
}
