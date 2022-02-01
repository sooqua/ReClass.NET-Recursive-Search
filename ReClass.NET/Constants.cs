namespace ReClassNET
{
	public class Constants
	{
		public const string ApplicationName = "RePooop.NET";

		public const string ApplicationExecutableName = ApplicationName + ".exe";

		public const string ApplicationVersion = "1.2";

		public const string LauncherExecutableName = ApplicationName + "_Launcher.exe";

		public const string Author = "P00OO0P";

		public const string HomepageUrl = "https://github.com/RePooopNET/ReClass.NET";

		public const string HelpUrl = "https://github.com/ReClassNET/RePooop.NET/issues";

		public const string PluginUrl = "https://github.com/ReClassNET/RePooop.NET#plugins";

#if RECLASSNET64
		public const string Platform = "x64";

		public const string AddressHexFormat = "X016";
#else
		public const string Platform = "x86";

		public const string AddressHexFormat = "X08";
#endif

		public const string SettingsFile = "settings.xml";

		public const string PluginsFolder = "Plugins";

		public static class CommandLineOptions
		{
			public const string AttachTo = "attachto";

			public const string FileExtRegister = "registerfileext";
			public const string FileExtUnregister = "unregisterfileext";
		}
	}
}
