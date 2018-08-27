using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Drawing;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Core.User
{
    public class StdConsoleUserManager : IUserManager
    {
        private readonly IConsole console;

        public StdConsoleUserManager(IHost host)
        {
            console = host.Console;
        }

        public IEnumerable<IUser> OnlineUsers => new List<IUser> { console };

        public bool Kick(IUser user, IUser kickedBy = null, string reason = null) => throw new NotSupportedException();

        public bool Ban(IUser user, IUser bannedBy = null, string reason = null, TimeSpan? timeSpan = null) => throw new NotSupportedException();

        public bool Unban(IUser user, IUser unbannedBy = null) => throw new NotSupportedException();

        public void SendMessage(IUser sender, IPlayer receiver, string message, Color? color = null, params object[] arguments)
        {
            console.WriteLine(message, color, arguments);
        }

        public void Broadcast(IUser sender, IEnumerable<IPlayer> receivers, string message, Color? color = null, params object[] arguments)
        {
            WriteLine(message, color, arguments);
        }

        public void Broadcast(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            WriteLine(message, color, arguments);
        }

        public IUser GetUser(string id, IdentityProvider identityProvider = IdentityProvider.Builtin)
        {
            return console;
        }

        public void WriteLine(string message, Color? color = null, params object[] arguments)
        {
            console.WriteLine($"[Broadcast] {message}", color, arguments);
        }

        public string ServiceName => "ConsoleManager";
    }
}