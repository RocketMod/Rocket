using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Providers.Commands;
using Rocket.API.Providers.Plugins;

namespace Rocket.Plugins.Native
{
    public class NativeRocketCommandProvider : IRocketCommandProvider
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

        public void Load(bool isReload = false)
        {

        }

        public void Unload(bool isReload = false)
        {

        }
    }
}