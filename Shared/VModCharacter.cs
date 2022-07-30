using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace VMods.Shared
{
	public readonly struct VModCharacter
	{
		#region Variables

		public readonly User User;
		public readonly PlayerCharacter Character;
		public readonly FromCharacter FromCharacter;

		#endregion

		#region Lifecycle

		public VModCharacter(User user, PlayerCharacter character) => (User, Character, FromCharacter) = (user, character, FromCharacter = new FromCharacter()
		{
			User = character.UserEntity._Entity,
			Character = user.LocalCharacter._Entity,
		});

		public VModCharacter(Entity userEntity, Entity charEntity, EntityManager? entityManager = null)
		{
			entityManager ??= Utils.CurrentWorld.EntityManager;
			User = entityManager.Value.GetComponentData<User>(userEntity);
			Character = entityManager.Value.GetComponentData<PlayerCharacter>(charEntity);
			FromCharacter = new FromCharacter()
			{
				User = Character.UserEntity._Entity,
				Character = User.LocalCharacter._Entity,
			};
		}

		#endregion

		#region Public Methods

		public static VModCharacter? GetVModCharacter(string charactername, EntityManager? entityManager = null)
		{
			entityManager ??= Utils.CurrentWorld.EntityManager;
			var users = entityManager.Value.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp);
			foreach(var userEntity in users)
			{
				var userData = entityManager.Value.GetComponentData<User>(userEntity);
				var playerCharacter = entityManager.Value.GetComponentData<PlayerCharacter>(userData.LocalCharacter._Entity);
				if(userData.CharacterName.ToString() == charactername)
				{
					return new VModCharacter(userData, playerCharacter);
				}
			}
			return null;
		}

		public void ApplyBuff(PrefabGUID buffGUID)
		{
			Utils.ApplyBuff(FromCharacter, buffGUID);
		}

		public void RemoveBuff(PrefabGUID buffGUID)
		{
			Utils.RemoveBuff(FromCharacter.Character, buffGUID);
		}

		public bool HasBuff(PrefabGUID buffGUID, EntityManager? entityManager = null)
		{
			entityManager ??= Utils.CurrentWorld.EntityManager;
			return BuffUtility.HasBuff(entityManager.Value, FromCharacter.Character, buffGUID);
		}

		#endregion
	}
}
