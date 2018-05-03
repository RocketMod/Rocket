using Rocket.API;
using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleChatManager : IChatManager
    {
        private readonly IConsoleCommandCaller console;

        public ConsoleChatManager(IImplementation impl)
        {
            console = impl.ConsoleCommandCaller;
        }
        public void SendMessage(IOnlinePlayer player, string message, params object[] bindings)
        {
            console.SendMessage($"[{player.Name}] {message}", null, bindings);
        }

        public void Broadcast(string message, params object[] bindings)
        {
            console.SendMessage($"[Broadcast] {message}", null, bindings);
        }
    }
}