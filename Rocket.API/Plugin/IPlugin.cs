using System.Collections.Generic;
using Rocket.API.Eventing;

namespace Rocket.API.Plugin
{
    public interface IPlugin : IEventEmitter
    {
        void Load();

        void Unload();
    }
}