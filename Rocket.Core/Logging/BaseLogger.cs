using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using NuGet.Common;
using Rocket.API.DependencyInjection;
using Rocket.Core.Configuration;
using Rocket.Core.Extensions;
using Rocket.Core.Player;
using Rocket.Core.User;
using Rocket.NuGet;
using ILogger = Rocket.API.Logging.ILogger;
using LogLevel = Rocket.API.Logging.LogLevel;

namespace Rocket.Core.Logging
{
    public abstract class BaseLogger : ILogger
    {
        private static readonly ICollection<Type> ignoredLoggingTypes = new HashSet<Type>
        {
            typeof(BaseLogger),
            typeof(LoggerProxy),
            typeof(FormattedLogger),
            typeof(LoggingExtensions),
            typeof(StdConsoleUserManager),
            typeof(UserExtensions),
            typeof(EntityExtensions),
            typeof(NuGetConsoleLogger),
            typeof(LoggerBase)
        };

        private static readonly ICollection<MethodBase> ignoredLoggingMethods = new HashSet<MethodBase>
        {
        };

        protected Configuration.LogSettings LogSettings
        {
            get
            {
                if (Container.TryResolve(null, out IRocketConfigurationProvider settings))
                    return settings.Configuration.Logging;

                return new LogSettings();
            }
        }

        protected BaseLogger(IDependencyContainer container)
        {
            Container = container;
            SkipTypeFromLogging(GetType());
        }

        public IDependencyContainer Container { get; }

        public void Log(LogLevel level, object message, Exception exception = null, params object[] arguments)
        {
            if (!IsEnabled(level))
                return;

            OnLog(message, level, exception, arguments);
        }

        public virtual bool IsEnabled(LogLevel level)
        {
            if (logLevels.ContainsKey(level))
                return logLevels[level];

            var settingsLevel = (LogLevel) Enum.Parse(typeof(LogLevel), LogSettings.LogLevel, true);
            return level >= settingsLevel;
        }

        private Dictionary<LogLevel, bool> logLevels = new Dictionary<LogLevel, bool>();
        public virtual void SetEnabled(LogLevel level, bool enabled)
        {
            if (logLevels.ContainsKey(level))
                logLevels[level] = enabled;
            else
                logLevels.Add(level, enabled);
        }

        public static void SkipTypeFromLogging(Type type)
        {
            ignoredLoggingTypes.Add(type);
        }

        public static void SkipMethodFromLogging(MethodBase method)
        {
            ignoredLoggingMethods.Add(method);
        }

        public abstract void OnLog(object message, LogLevel level = LogLevel.Information, Exception exception = null, params object[] bindings);

        public static string GetLogLevelPrefix(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "Trace";
                case LogLevel.Debug:
                    return "Debug";
                case LogLevel.Game:
                    return "Game";
                case LogLevel.Information:
                    return "Info";
                case LogLevel.Warning:
                    return "Warn";
                case LogLevel.Error:
                    return "Error";
                case LogLevel.Fatal:
                    return "Fatal";
            }

            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        public static Color GetLogLevelColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return Color.DarkGray;
                case LogLevel.Debug:
                    return Color.Gray;
                case LogLevel.Game:
                    return Color.Magenta;
                case LogLevel.Information:
                    return Color.Green;
                case LogLevel.Warning:
                    return Color.Yellow;
                case LogLevel.Error:
                    return Color.Red;
                case LogLevel.Fatal:
                    return Color.DarkRed;
            }

            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        public virtual MethodBase GetLoggerCallingMethod()
            => ReflectionExtensions.GetCallingMethod(ignoredLoggingTypes.ToArray(), ignoredLoggingMethods.ToArray());

        public abstract string ServiceName { get; }
    }
}