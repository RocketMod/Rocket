using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Permissions;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandPermissionGroups : ICommand
    {
        public string GetSyntax(ICommandContext context)
        {
            return "[add/remove] [player] [group]";
        }

        public List<string> Aliases => new List<string> { "PG" };
        public string Name => "PermissionGroups";
        public string Permission => null;

        public void Execute(ICommandContext context)
        {
            var permissions = context.Container.Get<IPermissionProvider>("default_permissions");
            if (context.Parameters.Length != 3)
                throw new CommandParameterMismatchException();

            throw new NotImplementedException();

            switch (context.Parameters[0].ToLower())
            {
                case "add":
                    break;

                case "remove":
                    break;

                default:
                    throw new CommandWrongUsageException("Unknown command syntax: " + context.Parameters[0]);
            }
        }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return true;
        }
    }
}