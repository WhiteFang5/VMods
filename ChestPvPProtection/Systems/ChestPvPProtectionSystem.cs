using ProjectM;
using ProjectM.Network;
using ProjectM.Scripting;
using System.Linq;
using Unity.Entities;
using VMods.Shared;
using Wetstone.API;

namespace VMods.ChestPvPProtection
{
	public static class ChestPvPProtectionSystem
	{
		#region Public Methods

		public static void Initialize()
		{
			InventoryHooks.InventoryInteractionEvent += OnInventoryInteraction;
		}

		public static void Deinitialize()
		{
			InventoryHooks.InventoryInteractionEvent -= OnInventoryInteraction;
		}

		#endregion

		#region Private Methods

		private static void OnInventoryInteraction(Entity eventEntity, VModCharacter vmodCharacter, params Entity[] inventoryEntities)
		{
			if(!ChestPvPProtectionSystemConfig.ChestPvPProtectionEnabled.Value)
			{
				return;
			}

			var server = VWorld.Server;
			var entityManager = server.EntityManager;
			var teamChecker = server.GetExistingSystem<ServerScriptMapper>()._ServerGameManager._TeamChecker;

			if(!vmodCharacter.HasBuff(Utils.PvPProtectionBuff) || vmodCharacter.User.IsAdmin)
			{
				return;
			}

			var playerTeam = entityManager.GetComponentData<Team>(vmodCharacter.FromCharacter.Character);
			var teams = inventoryEntities.Select(FindTeam);

			// Check if the player is not an ally and the inventory isn't neutral (i.e. an npc chest) of any inventory
			if(teams.Any(team => !teamChecker.IsAllies(playerTeam, team) && !teamChecker.IsNeutral(team)))
			{
				entityManager.AddComponent<DestroyTag>(eventEntity);
				if(ChestPvPProtectionSystemConfig.ChestPvPProtectionSendMessage.Value)
				{
					Utils.SendMessage(vmodCharacter.FromCharacter.User, $"Cannot interact with enemy Chests/Workstations while PvP Protected", ServerChatMessageType.System);
				}
			}

			// Nested Method(s)
			Team FindTeam(Entity entity)
			{
				if(entityManager.HasComponent<Team>(entity))
				{
					return entityManager.GetComponentData<Team>(entity);
				}
				else
				{
					// Workstations don't have a team but are attached, thus retrieve the team of the station's parent.
					var attach = entityManager.GetComponentData<Attach>(entity);
					return entityManager.GetComponentData<Team>(attach.Parent);
				}
			}
		}

		#endregion
	}
}
