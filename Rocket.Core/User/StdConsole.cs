using System;
using System.Drawing;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.User;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Logging;
using System.Collections.Generic;

namespace Rocket.Core.User
{
    [DontAutoRegister]
    public class StdConsole : IConsole
    {
        private ConsoleLogger consoleLogger => (ConsoleLogger)Container.Resolve<ILogger>("console_logger");

        public string UserType => "Console";

        public IDependencyContainer Container { get; }

        public StdConsole(IDependencyContainer container)
        {
            SessionConnectTime = DateTime.UtcNow;
            BaseLogger.SkipTypeFromLogging(GetType());
            Container = container.CreateChildContainer();
        }

        public string Id => "console";
        public string DisplayName => "Console";
        public string UserName => "Console";

        public IUserManager UserManager => Container.Resolve<IUserManager>("stdconsole");

        public bool IsOnline => true;
        public DateTime SessionConnectTime { get; }
        public DateTime? SessionDisconnectTime => null;
        public DateTime? LastSeen => DateTime.UtcNow;

        public List<IIdentity> Identities => new List<IIdentity>();

        public virtual void WriteLine(string format, params object[] bindings)
            => WriteLine(LogLevel.Information, format, Color.White, bindings);

        public virtual void WriteLine(string format, Color? color = null, params object[] bindings)
        {
            WriteLine(LogLevel.Information, format, color, bindings);
        }

        public virtual void Write(string format, Color? color = null, params object[] bindings)
        {
            consoleLogger.Write(format, color, bindings);
        }

        public virtual void Write(string format, params object[] bindings)
        {
            consoleLogger.Write(format, null, bindings);
        }

        public virtual void WriteLine(LogLevel level, string format, params object[] bindings)
        {
            WriteLine(LogLevel.Information, format, Color.White, bindings);
        }

        public virtual void WriteLine(LogLevel level, string format, Color? color = null, params object[] bindings)
        {
            consoleLogger.WriteLine(level, format, color, bindings);
        }
    }
}
