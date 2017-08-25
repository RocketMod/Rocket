using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Providers;
using Rocket.API.Providers.Commands;
using Rocket.API.Providers.Plugins;

namespace Rocket.Core.Providers.Plugin.Native
{
    public class NativeRocketCommandProvider : ProviderBase, IRocketCommandProvider
    {
        public NativeRocketCommandProvider(IRocketPluginProvider manager)
        {
            InternalCommands = new List<IRocketCommand>();
        }

        protected List<IRocketCommand> InternalCommands { get; set; }
        public ReadOnlyCollection<IRocketCommand> Commands => InternalCommands.AsReadOnly();
        public void AddCommands(IEnumerable<IRocketCommand> commands)
        {
            InternalCommands.AddRange(commands.AsEnumerable());
        }

        protected override void OnLoad(ProviderManager providerManager)
        {
            throw new NotImplementedException();
        }

        protected override void OnUnload()
        {
            throw new NotImplementedException();
        }
    }
}