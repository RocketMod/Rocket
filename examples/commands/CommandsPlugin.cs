using System.Collections.Generic;
using Rocket.API.Chat;
using Rocket.API.DependencyInjection;
using Rocket.Core.Plugins;

namespace Rocket.Examples.CommandsPlugin
{
    public class CommandsPlugin : Plugin
    {
        private readonly IChatManager chatManager;

        public override Dictionary<string, string> DefaultTranslations => new Dictionary<string, string>
        {
            {"broadcast", "[{0:Name}] {1}"}
        };

        public CommandsPlugin(
            IDependencyContainer container,
            IChatManager chatManager) : base(container)
        {
            this.chatManager = chatManager;
        }

        protected override void OnLoad(bool isReload)
        {
            CommandsCollection commandsObject = new CommandsCollection(chatManager, Translations);
            RegisterCommandsFromObject(commandsObject);
        }
    }
}
