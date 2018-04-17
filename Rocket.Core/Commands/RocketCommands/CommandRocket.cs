using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Plugin;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandRocket : ICommand
    {
        public List<ICommand> ChildCommands => null;
        public string GetSyntax(ICommandContext context)
        {
            return context.Parameters.Length == 0 ? "<reload>" : "";
        }

        public string Description => "Manage Rocket";
        public List<string> Aliases => null;

        public string GetHelpText(ICommandContext context)
        {
            if (context.Parameters.Length == 0)
                return Description;

            string cmd = context.Parameters.Get<string>(1).ToLower();
            if (cmd == "reload")
                return "Reloads all configs.";

            return "Unknown sub command.";
        }

        public string Name => "Rocket";
        public string Permission => "Rocket.ManageRocket";

        public void Execute(ICommandContext context)
        {
            if (context.Parameters.Length == 0)
                throw new CommandParameterMismatchException();

            string cmd = context.Parameters.Get<string>(1).ToLower();
            switch (cmd)
            {
                case "reload":
                    {
                        var permissions = context.Container.Get<IPermissionProvider>();
                        permissions.Reload();

                        foreach (var plugin in context.Container.Get<IPluginManager>())
                        {
                            plugin.Reload();
                        }

                        context.Caller.SendMessage("Reload completed.", ConsoleColor.DarkGreen);
                        return;
                    }
            }

            throw new CommandWrongUsageException();
        }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return true;
        }
    }
}