using System;
using System.Linq;
using System.Reflection;

namespace Web.Code.Common
{
	public class VersionInfo
	{
		static VersionInfo()
		{
			Assembly assembly = typeof (VersionInfo).Assembly;
			BuildNumber = string.Join("", assembly.GetName().Version.ToString().Split('.').Select(x => x.PadLeft(5, '0')));
			AssemblyVersion = "v" + BuildNumber + (Configuration.Current.IsDeveloperMode ? DateTime.Now.ToString("hhmmfffff") : "");
		}

		public static string AssemblyVersion { get; private set; }
		public static string BuildNumber { get; private set; }
	}
}