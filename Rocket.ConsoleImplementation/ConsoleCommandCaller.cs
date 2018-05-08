using System;
using Rocket.API.Commands;
using Rocket.API.Communication;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.Core.Logging;

namespace Rocket.ConsoleImplementation
{
    public class Console : IConsole
    {
        private readonly ILogger logger;

        public ConsoleUser(ILogger logger)
        {
            BaseLogger.SkipTypeFromLogging(GetType());
            this.logger = logger;
        }

        public string Id => "Console";
        public string Name => "Console";
        public Type CallerType => typeof(ConsoleUser);

        public void SendMessage(string message, ConsoleColor? color = null, params object[] arguments)
        {
            logger.LogInformation(message, color, arguments);
        }

        public string ToString(string format, IFormatProvider formatProvider) => Id;
    }
}