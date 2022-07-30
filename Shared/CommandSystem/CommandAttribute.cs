using System;
using System.Collections.Generic;
using System.Linq;

namespace VMods.Shared
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute
	{
		#region Properties

		public IReadOnlyList<string> Names { get; }
		public string Usage { get; }
		public string Description { get; }
		public bool ReqAdmin { get; }

		#endregion

		#region Livecycle

		public CommandAttribute(string name, string usage = "", string description = "", bool reqAdmin = false)
		{
			Names = name.Split(',').Select(x => x.Trim()).ToList();
			Usage = usage;
			Description = description;
			ReqAdmin = reqAdmin;
		}

		#endregion
	}
}
