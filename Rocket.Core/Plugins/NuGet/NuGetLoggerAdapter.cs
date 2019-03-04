using NuGet.Common;
using Rocket.Core.Logging;
using System;
using System.Threading.Tasks;
using RocketLogger = Rocket.API.Logging.ILogger;

namespace Rocket.Core.Plugins.NuGet
{
    public class NuGetLoggerAdapter : LoggerBase
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

        public override void Log(ILogMessage message)
        {
            var level = message.Level;
            string prefix = "[NuGet] ";
            var text = prefix + message.Message;

            switch (level)
            {
                case LogLevel.Debug:
                    logger.LogDebug(text);
                    break;
                case LogLevel.Verbose:
                    logger.LogTrace(text);
                    break;
                case LogLevel.Information:
                    logger.LogInformation(text);
                    break;
                case LogLevel.Minimal:
                    logger.LogInformation(text);
                    break;
                case LogLevel.Warning:
                    logger.LogWarning(text);
                    break;
                case LogLevel.Error:
                    logger.LogError(text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public override async Task LogAsync(ILogMessage message)
        {
            Log(message);
        }
    }
}