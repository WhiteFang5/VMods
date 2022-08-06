using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using VMods.Shared;
using Wetstone.API;
using Wetstone.Hooks;
using AdminLevel = VMods.Shared.CommandAttribute.AdminLevel;

namespace VMods.GenericChatCommands
{
	public static class MutePlayerChatSystem
	{
		#region Consts

		private const string MutedPlayersFileName = "MutedPlayers.json";

		#endregion

		#region Variables

		private static Dictionary<ulong, MuteData> _mutes;

		#endregion

		#region Public Methods

		public static void Initialize()
		{
			_mutes = VModStorage.Load(MutedPlayersFileName, () => new Dictionary<ulong, MuteData>());

			PruneMutes();

			VModStorage.SaveEvent += Save;
			Chat.OnChatMessage += OnChatMessage;
		}

		public static void Deinitialize()
		{
			Chat.OnChatMessage -= OnChatMessage;
			VModStorage.SaveEvent -= Save;
		}

		public static void Save()
		{
			PruneMutes();

			VModStorage.Save(MutedPlayersFileName, _mutes);
		}

		#endregion

		#region Private Methods

		private static void PruneMutes()
		{
			var now = DateTime.UtcNow;
			var keys = _mutes.Keys.ToList();
			foreach(var key in keys)
			{
				var muteData = _mutes[key];
				if(now > muteData.MutedUntil)
				{
					_mutes.Remove(key);
				}
			}
		}

		private static void OnChatMessage(VChatEvent chatEvent)
		{
			if(chatEvent.Cancelled || chatEvent.User.IsAdmin || !MutePlayerChatConfig.GenericChatCommandsMutingEnabled.Value)
			{
				return;
			}

			var now = DateTime.UtcNow;
			if(_mutes.TryGetValue(chatEvent.User.PlatformId, out var muteData) && muteData.MutedUntil > now)
			{
				switch(muteData.ChatType)
				{
					case -1:
						// Always mute everything!
						break;

					default:
						Utils.Logger.LogMessage($"{chatEvent.Type}");
						if((int)chatEvent.Type != muteData.ChatType)
						{
							// Not muted for the given chat type.
							return;
						}
						break;
				}

				chatEvent.Cancel();
				chatEvent.User.SendSystemMessage($"You have been muted for {Math.Ceiling(muteData.MutedUntil.Subtract(now).TotalMinutes)} more minutes.");
			}
		}

		[Command("mute", "mute <player-name> <number-of-minutes> [global/local]", "Mutes the given player for the given number of minutes in the given chat/channel (or all chats/channels when omitted) - commands can still be used by the muted player", AdminLevel.Moderator)]
		private static void OnMuteCommand(Command command)
		{
			if(MutePlayerChatConfig.GenericChatCommandsModsCanMute.Value || command.VModCharacter.AdminLevel.HasReqLevel(AdminLevel.Admin))
			{
				if(command.Args.Length >= 2)
				{
					var entityManager = VWorld.Server.EntityManager;
					(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);
					if(vmodCharacter.HasValue)
					{
						if(int.TryParse(command.Args[1], out var minutes) && minutes >= 1)
						{
							int? chatType = null;
							if(command.Args.Length >= 3)
							{
								switch(command.Args[2].ToLowerInvariant())
								{
									case "global":
										chatType = (int)ChatMessageType.Global;
										break;

									case "local":
										chatType = (int)ChatMessageType.Local;
										break;

									default:
										command.VModCharacter.SendSystemMessage($"<color=#ff0000>Invalid chat-type option. Options are: global, local</color>");
										break;
								}
							}
							else
							{
								chatType = -1;
							}

							if(chatType.HasValue)
							{
								if(vmodCharacter.Value.AdminLevel != ProjectM.AdminLevel.None)
								{
									var inChat = chatType.Value != -1 ? $" in {(ChatMessageType)chatType.Value} chat" : string.Empty;
									Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has muted {searchUsername} for {minutes} minutes{inChat}");

									PruneMutes();

									var platformId = vmodCharacter.Value.User.PlatformId;
									if(!_mutes.TryGetValue(platformId, out var muteData))
									{
										muteData = new MuteData();
										_mutes.Add(platformId, muteData);
									}
									muteData.MutedUntil = DateTime.UtcNow.AddMinutes(minutes);
									muteData.ChatType = chatType.Value;

									if(MutePlayerChatConfig.GenericChatCommandsAnnounceMutes.Value)
									{
										ServerChatUtils.SendSystemMessageToAllClients(entityManager, $"Vampire <color=#ffffff>{searchUsername}</color> has been muted for <color=#ffffff>{minutes}</color> minutes!");
									}
									else
									{
										if(vmodCharacter.Value != command.VModCharacter)
										{
											vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has muted you for <color=#ffffff>{minutes}</color> minutes");
										}
										command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> has been muted for <color=#ffffff>{minutes}</color> minutes{inChat}");
									}
								}
								else
								{
									command.VModCharacter.SendSystemMessage($"<color=#ff0000>{vmodCharacter.Value.AdminLevel}s cannot be muted!</color>");
								}
							}
						}
						else
						{
							CommandSystem.SendInvalidCommandMessage(command, true);
						}
					}
				}
				else
				{
					CommandSystem.SendInvalidCommandMessage(command);
				}
			}
			command.Use();
		}

		[Command("unmute", "unmute <player-name>", "Unmutes the given player", AdminLevel.Moderator)]
		private static void OnUnmuteCommand(Command command)
		{
			if(MutePlayerChatConfig.GenericChatCommandsModsCanMute.Value || command.VModCharacter.AdminLevel.HasReqLevel(AdminLevel.Admin))
			{
				if(command.Args.Length >= 1)
				{
					var entityManager = VWorld.Server.EntityManager;
					(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);
					if(vmodCharacter.HasValue)
					{
						PruneMutes();

						var platformId = vmodCharacter.Value.User.PlatformId;
						if(_mutes.TryGetValue(platformId, out var muteData))
						{
							var remainingMinutes = Math.Ceiling(muteData.MutedUntil.Subtract(DateTime.UtcNow).TotalMinutes);
							var inChat = muteData.ChatType != -1 ? $" in {(ChatMessageType)muteData.ChatType} chat" : string.Empty;

							Utils.Logger.LogMessage($"{command.VModCharacter.User.CharacterName} has unmuted {searchUsername} (who had {remainingMinutes} minutes of mute remaining{inChat})");

							_mutes.Remove(platformId);

							if(MutePlayerChatConfig.GenericChatCommandsAnnounceUnmutes.Value)
							{
								ServerChatUtils.SendSystemMessageToAllClients(entityManager, $"Vampire <color=#ffffff>{searchUsername}</color> has been unmuted");
							}
							else
							{
								if(vmodCharacter.Value != command.VModCharacter)
								{
									vmodCharacter.Value.SendSystemMessage($"<color=#ffffff>{command.VModCharacter.CharacterName}</color> has unmuted you");
								}
								command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> has been unmuted");
							}
						}
						else
						{
							command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> wasn't even muted to begin with");
						}
					}
				}
				else
				{
					CommandSystem.SendInvalidCommandMessage(command);
				}
			}
			command.Use();
		}

		[Command("remaining-mute,remainingmute", "remaining-mute <player-name>", "Tells you how many more minutes the mute for the given player will last", AdminLevel.Moderator)]
		private static void OnRemainingMuteCommand(Command command)
		{
			if(MutePlayerChatConfig.GenericChatCommandsModsCanMute.Value || command.VModCharacter.AdminLevel.HasReqLevel(AdminLevel.Admin))
			{
				if(command.Args.Length >= 1)
				{
					var entityManager = VWorld.Server.EntityManager;
					(var searchUsername, var vmodCharacter) = command.FindVModCharacter(entityManager: entityManager);
					if(vmodCharacter.HasValue)
					{
						PruneMutes();

						var platformId = vmodCharacter.Value.User.PlatformId;
						if(_mutes.TryGetValue(platformId, out var muteData))
						{
							var remainingMinutes = Math.Ceiling(muteData.MutedUntil.Subtract(DateTime.UtcNow).TotalMinutes);
							var inChat = muteData.ChatType != -1 ? $" in {(ChatMessageType)muteData.ChatType} chat" : string.Empty;

							command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> will remain muted for another <color=#ffffff>{remainingMinutes}</color> minutes{inChat}");
						}
						else
						{
							command.VModCharacter.SendSystemMessage($"<color=#ffffff>{searchUsername}</color> isn't even muted to begin with");
						}
					}
				}
				else
				{
					CommandSystem.SendInvalidCommandMessage(command);
				}
			}
			command.Use();
		}

		#endregion

		#region Nested

		private class MuteData
		{
			public DateTime MutedUntil { get; set; }
			public int ChatType { get; set; }
		}

		#endregion
	}
}
