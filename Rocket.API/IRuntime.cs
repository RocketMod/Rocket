using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;

namespace Rocket.API
{
    public interface IRuntime : IEventEmitter
    {
        IDependencyContainer Container { get; }
    }
}