using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.Core.Extensions;

namespace Rocket.Core.Commands
{
    public class RocketCommandProvider : ICommandProvider
    {
        public RocketCommandProvider(IRuntime runtime)
        {
            IEnumerable<Type> types = runtime.FindTypes<ICommand>(false);

            List<ICommand> list = new List<ICommand>();
            foreach (Type type in types)
                list.Add((ICommand) Activator.CreateInstance(type, new object[0]));
            Commands = list;
        }

        public IEnumerable<ICommand> Commands { get; }
    }
}