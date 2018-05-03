using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Extensions;

namespace Rocket.Core.Commands
{
    public class RocketCommandProvider : ICommandProvider
    {
        public RocketCommandProvider()
        {
            var types = (typeof(RocketCommandProvider).Assembly.FindTypes<ICommand>())
                .Where(c => c.GetCustomAttributes(typeof(DontAutoRegisterAttribute), true).Length == 0)
                .Where(c => !typeof(ISubCommand).IsAssignableFrom(c));

            List<ICommand> list = new List<ICommand>();
            foreach (Type type in types)
                list.Add((ICommand)Activator.CreateInstance(type, new object[0]));
            Commands = list;
        }

        public IEnumerable<ICommand> Commands { get; }
    }
}