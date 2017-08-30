using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Providers;

namespace Rocket.Core.Providers.Plugin.Native
{
    public class NativeRocketCommandProvider : ProviderBase, ICommandProvider
    {
        public NativeRocketCommandProvider(IPluginProvider manager)
        {
            InternalCommands = new List<Type>();
        }

        protected List<Type> InternalCommands { get; set; }
        public ReadOnlyCollection<Type> Commands => InternalCommands.AsReadOnly();
        public void AddCommands(IEnumerable<Type> commands)
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