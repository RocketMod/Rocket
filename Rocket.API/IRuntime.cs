using Rocket.API.DependencyInjection;

namespace Rocket.API {
    public interface IRuntime {
        IDependencyContainer Container { get; }
    }
}