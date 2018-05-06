using System;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.Core.Logging;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleCommandCaller : IConsoleCommandCaller
    {
        private readonly ILogger logger;

        public ConsoleCommandCaller(ILogger logger)
        {
            ConsoleLogger.SkipTypeFromLogging(GetType());
            this.logger = logger;
        }

        public int CompareTo(object obj) => throw new NotImplementedException();

        public int CompareTo(IIdentifiable other) => throw new NotImplementedException();

        public bool Equals(IIdentifiable other) => throw new NotImplementedException();

        public int CompareTo(string other) => throw new NotImplementedException();

        public bool Equals(string other) => throw new NotImplementedException();

        public string Id => "Console";
        public string Name => "Console";
        public Type CallerType => typeof(ConsoleCommandCaller);

        public void SendMessage(string message, ConsoleColor? color = null, params object[] bindings)
        {
            logger.LogInformation(message, color, bindings);
        }

        public string ToString(string format, IFormatProvider formatProvider) => Id;
    }
}