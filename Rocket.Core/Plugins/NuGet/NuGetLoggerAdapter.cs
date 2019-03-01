using NuGet.Common;
using Rocket.Core.Logging;
using System;
using System.Threading.Tasks;
using RocketLogger = Rocket.API.Logging.ILogger;

namespace Rocket.Core.Plugins.NuGet
{
    public class NuGetLoggerAdapter : ILogger
    {
        private readonly RocketLogger logger;

        static NuGetLoggerAdapter()
        {
            BaseLogger.SkipTypeFromLogging(typeof(NuGetLoggerAdapter));
        }

        public NuGetLoggerAdapter(RocketLogger logger)
        {
            this.logger = logger;
        }

        public void LogDebug(string data)
        {
            logger.LogDebug("[NuGet] " + data);
        }

        public void LogVerbose(string data)
        {
            logger.LogTrace("[NuGet] " + data);
        }

        public void LogInformation(string data)
        {
            logger.LogInformation("[NuGet] " + data);
        }

        public void LogMinimal(string data)
        {
            logger.LogInformation("[NuGet] " + data);
        }

        public void LogWarning(string data)
        {
            logger.LogWarning("[NuGet] " + data);
        }

        public void LogError(string data)
        {
            logger.Log("[NuGet] " + data);
        }

        public void LogInformationSummary(string data)
        {
            logger.LogInformation("[NuGet] " + data);
        }

        public void Log(LogLevel level, string data)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    LogDebug(data);
                    break;
                case LogLevel.Verbose:
                    LogVerbose(data);
                    break;
                case LogLevel.Information:
                    LogInformation(data);
                    break;
                case LogLevel.Minimal:
                    LogMinimal(data);
                    break;
                case LogLevel.Warning:
                    LogWarning(data);
                    break;
                case LogLevel.Error:
                    LogError(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public async Task LogAsync(LogLevel level, string data)
        {
            Log(level, data);
        }

        public void Log(ILogMessage message)
        {
            Log(message.Level, message.Message);
        }

        public async Task LogAsync(ILogMessage message)
        {
            Log(message);
        }
    }
}