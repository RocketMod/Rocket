using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.Utility;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.Core.Extensions;

namespace Rocket.Core.Commands
{
    public class RocketCommandProvider : ICommandProvider
    {
        public RocketCommandProvider(IRuntime runtime)
        {
            var types = runtime.FindTypes<ICommand>(false);

            var list = new List<ICommand>();
            foreach (var type in types)
                list.Add((ICommand) Activator.CreateInstance(type, new object[0]));
            Commands = list;
        }

        public IEnumerable<ICommand> Commands { get; }
    }
}