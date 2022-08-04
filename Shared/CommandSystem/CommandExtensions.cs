using ProjectM.Network;
using Unity.Entities;
using Wetstone.API;

namespace VMods.Shared
{
	public static class CommandExtensions
	{
		public static (string searchUsername, VModCharacter? vmodCharacter) FindVModCharacter(this Command command, int argIdx = 0, bool sendCannotBeFoundMessage = true, EntityManager? entityManager = null)
		{
			VModCharacter? fromCharacter;
			string searchUsername;

			entityManager ??= Utils.CurrentWorld.EntityManager;

			if(argIdx >= 0 && command.Args.Length >= (argIdx + 1))
			{
				searchUsername = command.Args[0];
				fromCharacter = VModCharacter.GetVModCharacter(searchUsername, entityManager);
			}
			else
			{
				searchUsername = command.VModCharacter.User.CharacterName.ToString();
				fromCharacter = command.VModCharacter;
			}

			if(sendCannotBeFoundMessage && !fromCharacter.HasValue)
			{
				command.VModCharacter.SendSystemMessage($"[{Utils.PluginName}] Vampire <color=#ffffff>{searchUsername}</color> couldn't be found.");
			}
			return (searchUsername, fromCharacter);
		}
	}
}
