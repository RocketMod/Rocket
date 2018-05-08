using System;
using System.Collections.Generic;
using System.Drawing;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.User;
using Rocket.Core.User;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleUserManager : IUserManager
    {
        private readonly IConsole console;

        public ConsoleUserManager(IImplementation impl)
        {
            console = impl.Console;
        }

        public void WriteLine(string message, Color? color = null, params object[] arguments)
        {
            console.SendMessage($"[Broadcast] {message}", color, arguments);
        }

        public IEnumerable<IUser> Users { get; }
        public bool Kick(IUser player, IUser caller = null, string reason = null)
        {
            throw new NotSupportedException();
        }

        public bool Ban(IUser player, IUser caller = null, string reason = null, TimeSpan? timeSpan = null)
        {
            throw new NotSupportedException();
        }

        public bool Unban(IUser player, IUser caller = null)
        {
            throw new NotSupportedException();
        }

        public void SendMessage(IUser sender, IUser receiver, string message, params object[] arguments)
        {
            WriteLine(message, null, arguments);
        }

        public void SendMessage(IUser sender, IEnumerable<IUser> receivers, string message, params object[] arguments)
        {
            WriteLine(message, null, arguments);
        }

        public void SendMessage(IUser sender, string message, params object[] arguments)
        {
            WriteLine(message, null, arguments);
        }

        public void SendMessage(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            WriteLine(message, color, arguments);
        }

        public void Broadcast(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            WriteLine(message, color, arguments);
        }
    }
}