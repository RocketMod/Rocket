using Rocket.API.Logging;
using System;
using System.IO;

namespace Rocket.API
{
    public static class Environment
    {
        static Environment()
        {
            if (!Directory.Exists(LogsDirectory)) Directory.CreateDirectory(LogsDirectory);
            WorkingDirectory = Directory.GetCurrentDirectory();
            if (System.Environment.OSVersion.Platform == PlatformID.Unix || System.Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                OperationSystem = OperationSystems.Unix;
            }
            else
            {
                OperationSystem = OperationSystems.Windows;
            }
            Logger.Initialize("Rocket.log");
        }

        public enum OperationSystems { Windows, Unix };
        public static OperationSystems OperationSystem = OperationSystems.Unix;

        public static readonly string WorkingDirectory;
        public static readonly string LogsDirectory = "Logs";
    }
}