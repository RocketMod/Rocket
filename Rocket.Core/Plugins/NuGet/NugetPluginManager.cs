using System;
using System.Collections;
using System.Collections.Generic;
using Rocket.API.Plugins;

namespace Rocket.Core.Plugins.NuGet
{
    public class NuGetPluginManager : IPluginManager
    {
        public IEnumerator<IPlugin> GetEnumerator()
        {
            return Plugins.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string ServiceName => "NuGet";
        public IEnumerable<IPlugin> Plugins { get; }
        public IPlugin GetPlugin(string name)
        {
            throw new NotImplementedException();
        }

        public bool PluginExists(string name)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void ExecuteSoftDependCode(string pluginName, Action<IPlugin> action)
        {
            throw new NotImplementedException();
        }
    }
}