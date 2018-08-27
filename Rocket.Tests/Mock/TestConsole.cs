using System;
using Rocket.API.Drawing;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.User;
using Rocket.Core.Logging;
using System.Collections.Generic;

namespace Rocket.Tests.Mock
{
    public class TestConsole : IConsole
    {
        public IDependencyContainer Container { get; }

        public TestConsole(IDependencyContainer container)
        {
            BaseLogger.SkipTypeFromLogging(GetType());
            Container = container.CreateChildContainer();
        }

        public string Id => "console";
        public string DisplayName => "Console";
        public string UserName => "Console";
        public UserType Type => UserType.Console;

        public IUserManager UserManager => Container.Resolve<IUserManager>("console");
        public DateTime? LastSeen => DateTime.Now;
        public List<IIdentity> Identities =>new List<IIdentity>();

        public void WriteLine(string format, params object[] bindings)
        {
            Console.WriteLine(format, bindings);
        }

        public void WriteLine(LogLevel level, string format, params object[] bindings)
        {
            Console.WriteLine(format, bindings);
        }

        public void WriteLine(LogLevel level, string format, Color? color = null, params object[] bindings)
        {
            Console.WriteLine(format, bindings);
        }

        public void WriteLine(string format, Color? color = null, params object[] bindings)
        {
            Console.WriteLine(format, bindings);
        }

        public void Write(string format, Color? color = null, params object[] bindings)
        {
            Console.Write(format, bindings);
        }

        public void Write(string format, params object[] bindings)
        {
            Write(format, null, bindings);
        }
    }
}