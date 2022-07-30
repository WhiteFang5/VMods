using ProjectM.Network;
using Unity.Entities;

namespace VMods.Shared
{
	public class Command
	{
		#region Properties

		public string Name { get; }
		public string[] Args { get; }

		public User User { get; }
		public Entity SenderUserEntity { get; }
		public Entity SenderCharEntity { get; }

		public bool Used { get; private set; }

		#endregion

		#region Lifecycle

		public Command(User user, Entity senderUserEntity, Entity senderCharEntity, string name, params string[] args)
			=> (User, SenderUserEntity, SenderCharEntity, Name, Args) = (user, senderUserEntity, senderCharEntity, name, args);

		#endregion

		#region Public Methods

		public void Use() => Used = true;

		#endregion
	}
}
