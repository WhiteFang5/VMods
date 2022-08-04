namespace VMods.Shared
{
	public class Command
	{
		#region Properties

		public string Name { get; }
		public string[] Args { get; }

		public VModCharacter VModCharacter { get; }

		public bool Used { get; private set; }

		#endregion

		#region Lifecycle

		public Command(VModCharacter vmodCharacter, string name, params string[] args)
			=> (VModCharacter, Name, Args) = (vmodCharacter, name, args);

		#endregion

		#region Public Methods

		public void Use() => Used = true;

		#endregion
	}
}
