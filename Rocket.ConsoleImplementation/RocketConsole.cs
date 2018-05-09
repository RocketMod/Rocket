using System;
using System.Drawing;
using System.Reflection;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.User;
using Rocket.Core.Configuration;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;

namespace Rocket.ConsoleImplementation
{
    public class RocketConsole : IConsole
    {
        private readonly IDependencyContainer container;

        public RocketConsole(IDependencyContainer container)
        {
            SessionConnectTime = DateTime.Now;
            BaseLogger.SkipTypeFromLogging(GetType());
            this.container = container;
        }

        public string Id => "Console";
        public string Name => "Console";
        public IdentityType Type => IdentityType.Console;

        public IUserManager UserManager => container.Resolve<IUserManager>("game");
        public bool IsOnline => true;
        public DateTime SessionConnectTime { get; }
        public DateTime? SessionDisconnectTime => null;
        public DateTime? LastSeen => DateTime.Now;
        public string UserType => "Console";

        public void WriteLine(string format, params object[] bindings)
            => WriteLine(LogLevel.Information, format, Color.White, bindings);

        public void WriteLine(string format, Color? color = null, params object[] bindings)
            => WriteLine(LogLevel.Information, format, color, bindings);

        public void WriteLine(LogLevel level, string format, params object[] bindings)
            => WriteLine(LogLevel.Information, format, Color.White, bindings);

        public void WriteLine(LogLevel level, string format, Color? color = null, params object[] bindings)
        {
            IRocketSettingsProvider rocketSettings = container.Resolve<IRocketSettingsProvider>();
            Color orgCol = ConsoleLogger.GetForegroundColor();

            SetForegroundColor(Color.White);
            Console.Write("[");

            SetForegroundColor(BaseLogger.GetLogLevelColor(level));
            Console.Write(BaseLogger.GetLogLevelPrefix(level));

            SetForegroundColor(Color.White);
            Console.Write("] ");

            if (rocketSettings?.Settings.IncludeMethodsInLogs ?? true)
            {
                SetForegroundColor(Color.White);
                Console.Write("[");

                SetForegroundColor(Color.DarkGray);
                Console.Write(GetLoggerCallingMethod().GetDebugName());

                SetForegroundColor(Color.White);
                Console.Write("] ");
            }

            SetForegroundColor(color ?? Color.White);

            string line = string.Format(format, bindings);
            Console.WriteLine(line);

            SetForegroundColor(orgCol);
        }

        public void Write(string format, Color? color = null, params object[] bindings)
        {
            ConsoleColor orgColor = Console.ForegroundColor;
            ConsoleLogger.SetForegroundColor(color ?? Color.White);
            Console.Write(format, bindings);
            Console.ForegroundColor = orgColor;
        }

        public void Write(string format, params object[] bindings)
        {
            Write(format, null, bindings);
        }

        private MethodBase GetLoggerCallingMethod() => ReflectionExtensions.GetCallingMethod(typeof(RocketConsole));

        private void SetForegroundColor(Color color)
        {
            ConsoleLogger.SetForegroundColor(color);
        }
    }
}
