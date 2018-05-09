using System.IO;
using Rocket.API.DependencyInjection;
using Rocket.API.Plugins;
using Rocket.Core.DependencyInjection;

namespace Rocket.Core.Logging
{
    [DontAutoRegister]
    public class PluginLogger : FileLogger
    {
        public PluginLogger(IDependencyContainer container, IPlugin plugin) : base(container)
        {
            string path = Path.Combine(plugin.WorkingDirectory, "Logs");
            File = Path.Combine(path, plugin.Name + ".log");
        }
    }
}