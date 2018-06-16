using System;
using Rocket.API.Drawing;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.User;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Logging;

namespace Rocket.Core.User
{
    [DontAutoRegister]
    public class DefaultConsole : IConsole
    {
        private readonly ConsoleLogger consoleLogger;
        public IDependencyContainer Container { get; }

        public DefaultConsole(IDependencyContainer container, IUserManager userManager)
        {
            UserManager = userManager;
            SessionConnectTime = DateTime.Now;
            BaseLogger.SkipTypeFromLogging(GetType());
            Container = container.CreateChildContainer();
            consoleLogger = (ConsoleLogger) container.Resolve<ILogger>("console_logger");
        }

        public string Id => "Console";
        public string Name => "Console";
        public string IdentityType => IdentityTypes.Console;

        public IUserManager UserManager { get; }
        public bool IsOnline => true;
        public DateTime SessionConnectTime { get; }
        public DateTime? SessionDisconnectTime => null;
        public DateTime? LastSeen => DateTime.Now;
        public string UserType => "Console";

        public virtual void WriteLine(string format, params object[] bindings)
            => WriteLine(LogLevel.Information, format, Color.White, bindings);

        public virtual void WriteLine(string format, Color? color = null, params object[] bindings)
            => WriteLine(LogLevel.Information, format, color, bindings);

        public virtual void Write(string format, Color? color = null, params object[] bindings)
        {
            consoleLogger.Write(format, color, bindings);
        }

        public virtual void Write(string format, params object[] bindings)
        {
            consoleLogger.Write(format, null, bindings);
        }

        public virtual void WriteLine(LogLevel level, string format, params object[] bindings)
            => WriteLine(LogLevel.Information, format, Color.White, bindings);

        public virtual void WriteLine(LogLevel level, string format, Color? color = null, params object[] bindings)
        {
            consoleLogger.WriteLine(level, format, color, bindings);
        }
    }
}
