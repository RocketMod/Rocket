using System;
using System.Drawing;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.User;
using Rocket.Core.Logging;

namespace Rocket.Tests.Mock
{
    public class TestConsole : IConsole
    {
        private readonly IDependencyContainer container;
        public TestConsole(IDependencyContainer container)
        {
            SessionConnectTime = DateTime.Now;
            BaseLogger.SkipTypeFromLogging(GetType());
            this.container = container;
        }

        public string Id => "Console";
        public string Name => "Console";
        public IdentityType Type => IdentityType.Console;

        public IUserManager UserManager => container.Resolve<IUserManager>("console");
        public bool IsOnline => true;
        public DateTime SessionConnectTime { get; }
        public DateTime? SessionDisconnectTime => null;
        public string UserType => "Console";

        public void WriteLine(string format, Color color, object[] bindings)
        {
            Console.WriteLine(format, bindings);
        }

        public void Write(string format, Color color, object[] bindings)
        {
            Console.Write(format, bindings);
        }
    }
}