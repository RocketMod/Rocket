using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.Core.Configuration;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Commands
{
    public class CommandProviderProxy : ServiceProxy<ICommandProvider>, ICommandProvider
    {
        private readonly IDependencyContainer container;
        private readonly IConfiguration configuration;

        public CommandProviderProxy(IDependencyContainer container, IConfiguration configuration) : base(container)
        {
            this.container = container;
            this.configuration = configuration;
        }

        public ILifecycleObject GetOwner(ICommand command)
        {
            if (command is ProxyCommand)
                command = ((ProxyCommand)command).BaseCommand;

            return GetProvider(command)?.GetOwner(command) ?? throw new Exception("Owner not found.");
        }

        public async Task InitAsync()
        {
            foreach (var service in ProxiedServices)
            {
                await service.InitAsync();
            }

            IRuntime runtime = container.Resolve<IRuntime>();
            var context = runtime.CreateChildConfigurationContext("Commands");
            await configuration.LoadAsync(context, new CommandProviderProxyConfig());
        }

        public ICommandProvider GetProvider(ICommand command)
        {
            if (command is ProxyCommand)
                command = ((ProxyCommand)command).BaseCommand;

            foreach (ICommandProvider service in ProxiedServices)
                if (service.Commands.Any(c => c == command))
                    return service;

            return null;
        }

        public ICommandProvider GetProvider(string name)
        {
            return ProxiedServices.FirstOrDefault(c => c.ServiceName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<ICommand> registeredCommands;

        public void Invalidate()
        {
            Dictionary<ICommand, ICommandProvider> commands = new Dictionary<ICommand, ICommandProvider>();

            //first register from config
            foreach (var command in configuration.Get<CommandProviderProxyConfig>().Commands)
            {
                var providerName = command.Provider.Split('/')[0];
                var originalCommandName = command.Provider.Split('/')[1];

                var provider = GetProvider(providerName);

                var providerCommand = provider?.Commands.FirstOrDefault(c
                    => c.Name.Equals(originalCommandName,
                        StringComparison.OrdinalIgnoreCase));

                if (providerCommand == null || !command.IsEnabled)
                    continue;

                commands.Add(new ProxyCommand(providerCommand, command.Name), provider);
            }

            //now register from serivces and override if required
            foreach (var proxy in ProxiedServices)
                foreach (var command in proxy.Commands)
                {
                    ICommand previousRegistration = null;
                    ICommandProvider previousProvider = null;

                    foreach (ICommand c in commands.Keys)
                    {
                        if ((c as ProxyCommand)?.BaseCommand?.Name?.Equals(command.Name, StringComparison.OrdinalIgnoreCase) ?? false || c.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            previousRegistration = c;
                            previousProvider = commands[c];
                            break;
                        }
                    }

                    if (((previousRegistration as ProxyCommand)?.BaseCommand ?? previousRegistration)
                        == command)
                    {
                        continue;
                    }

                    if (previousProvider != null)
                    {
                        var currentPriority = ServicePriorityComparer.GetPriority(proxy);
                        var previousPriority = ServicePriorityComparer.GetPriority(previousProvider);

                        if (currentPriority < previousPriority)
                            continue; // dont override if priority is lower
                    }

                    if (previousRegistration != null)
                        commands.Remove(previousRegistration);

                    commands.Add(command, proxy);
                }

            registeredCommands = commands.Keys;
            IEnumerable<ConfigCommandProxy> configCommands =
                registeredCommands.Select(c => GetProxyCommand(c, commands[c]));

            configuration.Set(new CommandProviderProxyConfig { Commands = configCommands.ToArray() });
            configuration.SaveAsync().GetAwaiter().GetResult();
        }

        private ConfigCommandProxy GetProxyCommand(ICommand command, ICommandProvider provider)
        {
            var implName = (command as ProxyCommand)?.BaseCommand?.Name ?? command.Name;
            return new ConfigCommandProxy
            {
                Name = command.Name,
                Provider = provider.ServiceName + "/" + implName,
                IsEnabled = true
            };
        }

        private int previousProxyCount = 0;
        public IEnumerable<ICommand> Commands
        {
            get
            {
                if (registeredCommands == null)
                    Invalidate();

                var currentCount = ProxiedServices.Count();
                if (currentCount != previousProxyCount)
                {
                    Invalidate();
                    previousProxyCount = currentCount;
                }

                return registeredCommands;
            }
        }

        public string ServiceName => "ProxyCommands";
    }

    class CommandProviderProxyConfig
    {
        [ConfigArray]
        public ConfigCommandProxy[] Commands { get; set; } = new ConfigCommandProxy[0];
    }
}