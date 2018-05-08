using Rocket.API;
using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleChatManager : IChatManager
    {
        private readonly IConsoleUser console;

        public ConsoleChatManager(IImplementation impl)
        {
            console = impl.ConsoleUser;
        }
        public void SendMessage(IOnlinePlayer player, string message, params object[] arguments)
        {
            console.SendMessage($"[{player.Name}] {message}", null, arguments);
        }

        public void Broadcast(string message, params object[] arguments)
        {
            console.SendMessage($"[Broadcast] {message}", null, arguments);
        }
    }
}