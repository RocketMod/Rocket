using System;
using System.IO;
using Logger = Rocket.API.Logging.Logger;

namespace Rocket.API
{
    public static class Environment
    {
        public static void Initialize()
        {
            if (!Directory.Exists(PluginsDirectory)) Directory.CreateDirectory(PluginsDirectory);
            if (!Directory.Exists(LibrariesDirectory)) Directory.CreateDirectory(LibrariesDirectory);
            if (!Directory.Exists(LogsDirectory)) Directory.CreateDirectory(LogsDirectory);
            if (System.Environment.OSVersion.Platform == PlatformID.Unix || System.Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                OperationSystem = OperationSystems.Unix;
            }
            else
            {
                OperationSystem = OperationSystems.Windows;
            }
        }

        public enum OperationSystems { Windows, Unix };
        public static OperationSystems OperationSystem = OperationSystems.Unix;
        

        public static readonly string PluginsDirectory = "Plugins";
        public static readonly string LibrariesDirectory = "Libraries";
        public static readonly string LogsDirectory = "Logs";

        public static readonly string SettingsFile = "Rocket.config.xml";
        public static readonly string LogConfigurationFile = "log4net.config.xml";
        public static readonly string TranslationFile = "Rocket.{0}.translation.xml";
        public static readonly string PermissionFile = "Permissions.config.xml";
        public static readonly string CommandsFile = "{0}.config.xml";

        public static readonly string PluginDirectory = "Plugins/{0}/";
        public static readonly string PluginTranslationFileTemplate = "{0}.{1}.translation.xml";
        public static readonly string PluginConfigurationFileTemplate = "{0}.configuration.xml";

        public static readonly string WebConfigurationTemplate = "{0}?configuration={1}&instance={2}";

        public static string LanguageCode { get; set; } = "en";
    }
}