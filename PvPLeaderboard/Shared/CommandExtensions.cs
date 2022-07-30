using ProjectM.Network;
using Unity.Entities;
using Wetstone.API;

namespace VMods.Shared
{
	public static class CommandExtensions
	{
		public static (string searchUsername, FromCharacter? fromCharacter) GetFromCharacter(this Command command, int argIdx = 0, bool sendCannotBeFoundMessage = true, EntityManager? entityManager = null)
		{
			FromCharacter? fromCharacter;
			string searchUsername;

			entityManager ??= Utils.CurrentWorld.EntityManager;

			if(argIdx >= 0 && command.Args.Length >= (argIdx + 1))
			{
				searchUsername = command.Args[0];
				fromCharacter = Utils.GetFromCharacter(searchUsername, entityManager);
			}
			else
			{
				searchUsername = command.User.CharacterName.ToString();
				fromCharacter = new FromCharacter()
				{
					User = command.SenderUserEntity,
					Character = command.SenderCharEntity,
				};
			}

			if(sendCannotBeFoundMessage && !fromCharacter.HasValue)
			{
				command.User.SendSystemMessage($"Vampire <color=#ffffff>{searchUsername}</color> couldn't be found.");
			}
			return (searchUsername, fromCharacter);
		}
	}
}
