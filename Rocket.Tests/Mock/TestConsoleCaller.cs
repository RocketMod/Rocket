using System;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.Core.Logging;

namespace Rocket.Tests.Mock
{
    public class TestConsoleCaller : IConsoleUser
    {
        private readonly ILogger logger;

        public TestConsoleCaller(ILogger logger)
        {
            BaseLogger.SkipTypeFromLogging(GetType());
            this.logger = logger;
        }

        public string Id => "Console";
        public string Name => "Console";
        public Type CallerType => typeof(TestConsoleCaller);

        public void SendMessage(string message, params object[] arguments)
        {
            logger.LogInformation(message, arguments);
        }

        public int CompareTo(object obj) => throw new NotImplementedException();

        public int CompareTo(IIdentity other) => throw new NotImplementedException();

        public bool Equals(IIdentity other) => throw new NotImplementedException();

        public int CompareTo(string other) => throw new NotImplementedException();

        public bool Equals(string other) => throw new NotImplementedException();

        public string ToString(string format, IFormatProvider formatProvider) => Id;
    }
}