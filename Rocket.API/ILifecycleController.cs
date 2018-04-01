using System;
using Rocket.API.Eventing;

namespace Rocket.API
{
    public interface ILifecycleObject : IEventEmitter
    {
        bool IsAlive { get; }
    }
}