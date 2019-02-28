using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.DependencyInjection;
using Rocket.API.User;
using Rocket.Core.Plugins;

namespace Rocket.Examples.CommandsExample
{
    public class CommandsPlugin : Plugin
    {
        private readonly IUserManager userManager;

        public CommandsPlugin(
            IDependencyContainer container,
            IUserManager userManager) : base(container)
        {
            this.userManager = userManager;
        }

        public override Dictionary<string, string> DefaultTranslations => new Dictionary<string, string>
        {
            {"broadcast", "[{0:Name}] {1}"}
        };

        protected override async Task OnActivate(bool isFromReload)
        {
            CommandsCollection commandsObject = new CommandsCollection(userManager, Translations);
            RegisterCommandsFromObject(commandsObject);
        }
    }
}