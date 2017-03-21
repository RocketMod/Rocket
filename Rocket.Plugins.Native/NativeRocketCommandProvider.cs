using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Providers;

namespace Rocket.Plugins.Native
{
    public class NativeRocketCommandProvider : IRocketCommandProvider
    {
        public NativeRocketCommandProvider(IRocketPluginProvider manager)
        {
            Commands = new RocketCommandList(manager);
        }

        public RocketCommandList Commands { get; }
        public void AddCommands(IEnumerable<IRocketCommand> commands)
        {
            Commands.AddRange(commands.AsEnumerable());
        }

        public void Load(bool isReload = false)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }
    }
}