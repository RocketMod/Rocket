using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.Core.Configuration;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Commands
{
    public class ProxyCommandProvider : ServiceProxy<ICommandProvider>, ICommandProvider
    {
        private readonly IConfiguration configuration;

        public ProxyCommandProvider(IDependencyContainer container, IConfiguration configuration) : base(container)
        {
            this.configuration = configuration;
            IRuntime runtime = container.Resolve<IRuntime>();
            var context = runtime.CreateChildConfigurationContext("Commands");
            configuration.Load(context, new ProxyCommandProviderConfig());
        }

        public ILifecycleObject GetOwner(ICommand command)
        {
            if (command is ProxyCommand)
                command = ((ProxyCommand)command).BaseCommand;

            return GetProvider(command)?.GetOwner(command) ?? throw new Exception("Owner not found.");
        }

        public ICommandProvider GetProvider(ICommand command)
        {
            if (command is ProxyCommand)
                command = ((ProxyCommand) command).BaseCommand;

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
            foreach (var command in configuration.Get<ProxyCommandProviderConfig>().Commands)
            {
                var providerName = command.Provider.Split('/')[0];
                var originalCommandName = command.Provider.Split('/')[1];

                var provider = GetProvider(providerName);

                var providerCommand = provider?.Commands.FirstOrDefault(c
                    => c.Name.Equals(originalCommandName,
                        StringComparison.OrdinalIgnoreCase));

                if (providerCommand == null)
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
            IEnumerable<ProxyConfigCommand> configCommands = registeredCommands.Select(c => GetProxyCommand(c, commands[c]));

            configuration.Set(new ProxyCommandProviderConfig { Commands = configCommands.ToArray() });
            configuration.Save();
        }

        private ProxyConfigCommand GetProxyCommand(ICommand command, ICommandProvider provider)
        {
            var implName = (command as ProxyCommand)?.BaseCommand?.Name ?? command.Name;
            return new ProxyConfigCommand
            {
                Name = command.Name,
                Provider = provider.ServiceName + "/" + implName
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

    class ProxyCommandProviderConfig
    {
        [ConfigArray]
        public ProxyConfigCommand[] Commands { get; set; } = new ProxyConfigCommand[0];
    }
}