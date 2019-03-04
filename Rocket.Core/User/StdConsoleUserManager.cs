using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using System.Drawing;
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

        public async Task BroadcastAsync(IUser sender, IEnumerable<IUser> receivers, string message, Color? color = null, params object[] arguments)
        {
            WriteLine(message, color, arguments);
        }

        public async Task BroadcastAsync(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            WriteLine(message, color, arguments);
        }

        public void WriteLine(string message, Color? color = null, params object[] arguments)
        {
            console.WriteLine($"[Broadcast] {message}", color, arguments);
        }

        public Task<bool> BanAsync(IUser user, IUser bannedBy = null, string reason = null, TimeSpan? duration = null)
        {
            throw new NotSupportedException();
        }

        public Task<bool> UnbanAsync(IUser user, IUser unbannedBy = null)
        {
            throw new NotSupportedException();
        }


        public Task<bool> KickAsync(IUser user, IUser kickedBy = null, string reason = null)
        {
            throw new NotSupportedException();
        }

        public async Task SendMessageAsync(IUser sender, IUser receiver, string message, Color? color = null, params object[] arguments)
        {
            if (receiver != null && receiver != console)
                return;

            console.WriteLine(message, color, arguments);
        }

        public async Task<IUser> GetUserAsync(string id)
        {
            if (id.Equals("console", StringComparison.OrdinalIgnoreCase))
                return console;
            return null;
        }

        public Task<IIdentity> GetIdentity(string id)
        {
            throw new NotSupportedException();
        }

        public string ServiceName => "ConsoleManager";
    }
}