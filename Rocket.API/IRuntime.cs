using Rocket.API.Eventing;
using Rocket.API.Ioc;

namespace Rocket.API
{
    public interface IRuntime : IEventEmitter
    {
        IDependencyContainer Container { get; }
    }
}