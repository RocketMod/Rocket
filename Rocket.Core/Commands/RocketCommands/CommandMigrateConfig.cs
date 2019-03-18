using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.User;
using Rocket.Core.Configuration;
using Rocket.Core.User;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandMigrateConfig : ICommand
    {
        public string Name => "MigrateConfig";
        public string[] Aliases => null;
        public string Summary => "Migrates configs from one type to another.";
        public string Description => null;
        public string Syntax => "[<from type> <to type> <path>]";
        public IChildCommand[] ChildCommands { get; }

        public bool SupportsUser(IUser user) => user is IConsole;

        public async Task ExecuteAsync(ICommandContext context)
        {
            if (context.Parameters.Length != 0 && context.Parameters.Length < 3) throw new CommandWrongUsageException();

            IConfiguration[] configProviders = context.Container.ResolveAll<IConfiguration>().ToArray();

            if (context.Parameters.Length == 0)
            {
                await context.User.SendMessageAsync(GetConfigTypes(configProviders));
                await context.SendCommandUsageAsync();
                return;
            }

            string from = context.Parameters[0];
            string to = context.Parameters[1];
            string path = context.Parameters.GetArgumentLine(2);

            if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
            {
                await context.User.SendMessageAsync("\"from\" and \"to\" can not be the same config type!");
                return;
            }

            IConfiguration fromProvider =
                configProviders.FirstOrDefault(c => c.Name.Equals(from, StringComparison.OrdinalIgnoreCase));

            if (fromProvider == null)
                throw new CommandWrongUsageException($"\"{from}\" is not a valid config type. "
                    + GetConfigTypes(configProviders));

            IConfiguration toProvider =
                configProviders.FirstOrDefault(c => c.Name.Equals(to, StringComparison.OrdinalIgnoreCase));

            if (toProvider == null)
                throw new CommandWrongUsageException($"\"{to}\" is not a valid config type. "
                    + GetConfigTypes(configProviders));

            string workingDir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            ConfigurationContext cc = new ConfigurationContext(workingDir, fileName);

            await fromProvider.LoadAsync(cc);
            toProvider.ConfigurationContext = cc;
            toProvider.LoadEmpty();

            CopyConfigElement(fromProvider, toProvider);

            await toProvider.SaveAsync();
            await context.User.SendMessageAsync("Configuration was successfully migrated.");
        }

        private void CopyConfigElement(IConfigurationElement fromSection, IConfigurationElement toSection)
        {
            foreach (IConfigurationSection fromChild in fromSection.GetChildren())
            {
                IConfigurationSection toChild = toSection.CreateSection(fromChild.Key, fromChild.Type);

                if (fromChild.Type != SectionType.Object)
                    toChild.Set(fromChild.Get());
                else
                    CopyConfigElement(fromChild, toChild);
            }
        }

        private string GetConfigTypes(IConfiguration[] configProviders)
        {
            return "Available config types: " + string.Join(", ", configProviders.Select(c => c.Name).ToArray());
        }
    }
}