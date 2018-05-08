using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.User;

namespace Rocket.Core.Commands
{
    public static class CommandsExtensions
    {
        public static ICommand GetCommand(this IEnumerable<ICommand> commandsEnumerable, string commandName, IUser caller)
        {
            var commands = commandsEnumerable.ToList();
            var command =
                commands
                    .Where(c => caller == null || c.SupportsUser(caller.GetType()))
                    .FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));
            if (command != null)
                return command;

            return commands
                   .Where(c => caller == null || c.SupportsUser(caller.GetType()))
                   .FirstOrDefault(c => c.Aliases != null && c.Aliases.Any(a => a.Equals(commandName, StringComparison.OrdinalIgnoreCase)));
        }
    }
}