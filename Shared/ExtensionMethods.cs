using System;

namespace VMods.Shared
{
	public static class ExtensionMethods
	{
		public static string ToAgoString(this TimeSpan timeSpan)
		{
			if(timeSpan.TotalDays >= 1d)
			{
				return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
			}
			else if(timeSpan.TotalHours >= 1d)
			{
				return $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
			}
			else if(timeSpan.TotalMinutes >= 1d)
			{
				return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
			}
			else if(timeSpan.TotalSeconds >= 1d)
			{
				return $"{timeSpan.Seconds}s";
			}
			return $"{timeSpan.Milliseconds}ms";
		}

		public static bool HasReqLevel(this ProjectM.AdminLevel adminLevel, ProjectM.AdminLevel reqAdminLevel) => (int)adminLevel >= (int)reqAdminLevel;

		public static bool HasReqLevel(this ProjectM.AdminLevel adminLevel, CommandAttribute.AdminLevel reqAdminLevel) => (int)adminLevel >= (int)reqAdminLevel;

		public static bool HasReqLevel(this CommandAttribute.AdminLevel adminLevel, ProjectM.AdminLevel reqAdminLevel) => (int)adminLevel >= (int)reqAdminLevel;

		public static ProjectM.AdminLevel ToAdminLevel(this CommandAttribute.AdminLevel adminLevel) => (ProjectM.AdminLevel)(int)adminLevel;

		public static CommandAttribute.AdminLevel ToAdminLevel(this ProjectM.AdminLevel adminLevel) => (CommandAttribute.AdminLevel)(int)adminLevel;
	}
}
