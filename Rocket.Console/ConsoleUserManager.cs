using System;
using System.Collections.Generic;
using System.Drawing;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.User;

namespace Rocket.Console
{
    public class ConsoleUserManager : IUserManager
    {
        private readonly IConsole console;

        public ConsoleUserManager(IHost impl)
        {
            console = impl.Console;
        }

        public IEnumerable<IUser> Users { get; }

        IEnumerable<IUser> IUserManager.Users => Users;

        public bool Kick(IUser user, IUser kickedBy = null, string reason = null) => throw new NotSupportedException();

        public bool Ban(IUserInfo user, IUser bannedBy = null, string reason = null, TimeSpan? timeSpan = null)
            => throw new NotSupportedException();

        public bool Unban(IUserInfo user, IUser unbannedBy = null) => throw new NotSupportedException();

        public void SendMessage(IUser sender, IUser receiver, string message, Color? color = null, params object[] arguments)
        {
            console.WriteLine(message, color, arguments);
        }

        public void Broadcast(IUser sender, IEnumerable<IUser> receivers, string message, Color? color = null, params object[] arguments)
        {
            WriteLine(message, color, arguments);
        }

        public void Broadcast(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            WriteLine(message, color, arguments);
        }

        public void WriteLine(string message, Color? color = null, params object[] arguments)
        {
            console.WriteLine($"[Broadcast] {message}", color, arguments);
        }

        public string ServiceName => "ConsoleManager";
    }
}