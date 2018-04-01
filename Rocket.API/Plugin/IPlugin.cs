using System;
using System.Collections.Generic;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;

namespace Rocket.API.Plugin
{
    public interface IPlugin : ILifecycleObject
    {
        IEnumerable<string> Capabilities { get; }
        
        void Load();

        void Unload();
    }
}
