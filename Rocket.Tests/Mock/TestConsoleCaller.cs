using System;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Permissions;

namespace Rocket.Tests.Mock {
    public class TestConsoleCaller : IConsoleCommandCaller
    {
        public string Id => "Console";
        public string Name => "Console";
        public Type CallerType => typeof(TestConsoleCaller);
        public void SendMessage(string message, ConsoleColor? color = null, params object[] bindings)
        {
            var tmp = Console.ForegroundColor;
            Console.ForegroundColor = color ?? tmp;
            Console.WriteLine("[SendMessage] " + message, bindings);
            Console.ForegroundColor = tmp;
        }
        public void SendLocalizedMessage(ITranslationLocator translations, string translationKey, ConsoleColor? color = null,
                                         params object[] bindings)
        {
            SendMessage(translations.GetLocalizedMessage(translationKey), color, bindings);
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IIdentifiable other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IIdentifiable other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(string other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(string other)
        {
            throw new NotImplementedException();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Id;
        }
    }
}