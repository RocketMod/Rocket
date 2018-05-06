using System;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.Core.Logging;

namespace Rocket.Tests.Mock
{
    public class TestConsoleCaller : IConsoleCommandCaller
    {
        private readonly ILogger logger;

        public TestConsoleCaller(ILogger logger)
        {
            ConsoleLogger.SkipTypeFromLogging(GetType());
            this.logger = logger;
        }

        public string Id => "Console";
        public string Name => "Console";
        public Type CallerType => typeof(TestConsoleCaller);

        public void SendMessage(string message, ConsoleColor? color = null, params object[] bindings)
        {
            logger.LogInformation(message, color, bindings);
        }

        public int CompareTo(object obj) => throw new NotImplementedException();

        public int CompareTo(IIdentifiable other) => throw new NotImplementedException();

        public bool Equals(IIdentifiable other) => throw new NotImplementedException();

        public int CompareTo(string other) => throw new NotImplementedException();

        public bool Equals(string other) => throw new NotImplementedException();

        public string ToString(string format, IFormatProvider formatProvider) => Id;
    }
}