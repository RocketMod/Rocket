using System;

namespace Rocket.API
{
    public interface ILifecycleObject
    {
        bool IsAlive { get; }
    }
}