using ProjectM;
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
		public AdminLevel ReqAdminLevel { get; }

		#endregion

		#region Lifecycle

		public CommandAttribute(string name, string usage = "", string description = "", AdminLevel reqAdminLevel = AdminLevel.None)
		{
			Names = name.Split(',').Select(x => x.Trim()).ToList();
			Usage = usage;
			Description = description;
			ReqAdminLevel = reqAdminLevel;
		}

		#endregion

		#region Nested

		/// Exact copy of <see cref="ProjectM.AdminLevel"/>
		public enum AdminLevel
		{
			None = 0,
			Moderator = 1,
			Admin = 2,
			SuperAdmin = 3
		}

		#endregion
	}
}
