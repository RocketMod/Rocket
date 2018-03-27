using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Core
{
    public enum PluginState { Loaded, Unloaded, Failure, Cancelled };
    
    public interface IPlugin
    {
        string Name { get; }
        PluginState State { get; }
        void Load(IDependencyResolver resolver, IConfigurationManager configuration, ITranslationManager translation);
        void Unload();
    }
}
