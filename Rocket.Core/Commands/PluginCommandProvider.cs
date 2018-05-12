using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Plugins;
using Rocket.Core.DependencyInjection;

namespace Rocket.Core.Commands
{
    [DontAutoRegister]
    public class PluginCommandProvider : ICommandProvider
    {
        private readonly IDependencyContainer pluginContainer;
        private readonly IPlugin plugin;

        public PluginCommandProvider(IPlugin plugin, IDependencyContainer pluginContainer)
        {
            this.pluginContainer = pluginContainer;
            this.plugin = plugin;
        }

        public ILifecycleObject GetOwner(ICommand command) => plugin;
        public string ProviderName => plugin.Name;
        public IEnumerable<ICommand> Commands => pluginContainer.ResolveAll<ICommand>();
    }
}