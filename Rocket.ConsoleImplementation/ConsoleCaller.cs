using System;
using Rocket.API.Commands;
using Rocket.API.Permissions;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleCaller : IConsoleCommandCaller
    {
        public int CompareTo(object obj) => throw new NotImplementedException();

        public int CompareTo(IIdentifiable other) => throw new NotImplementedException();

        public bool Equals(IIdentifiable other) => throw new NotImplementedException();

        public int CompareTo(string other) => throw new NotImplementedException();

        public bool Equals(string other) => throw new NotImplementedException();

        public string Id => "Console";
        public string Name => "Console";
        public Type CallerType => typeof(ConsoleCaller);

        public void SendMessage(string message, ConsoleColor? color = null, params object[] bindings)
        {
            ConsoleColor tmp = Console.ForegroundColor;
            Console.ForegroundColor = color ?? tmp;
            Console.WriteLine("[SendMessage] " + message, bindings);
            Console.ForegroundColor = tmp;
        }

        public string ToString(string format, IFormatProvider formatProvider) => Id;
    }
}