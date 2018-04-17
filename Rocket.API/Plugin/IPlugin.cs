using System.Collections.Generic;
using Rocket.API.Eventing;

namespace Rocket.API.Plugin
{
    public interface IPlugin : IEventEmitter
    {
        string WorkingDirectory { get; }

        void Load();
        void Unload();
        void Reload();
    }
}