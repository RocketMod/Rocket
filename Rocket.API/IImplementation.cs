using Rocket.API.Commands;
using Rocket.API.Eventing;

namespace Rocket.API
{
    public interface IImplementation : IEventEmitter, IConfigurationContext
    {
        string InstanceId { get; }
        string WorkingDirectory { get; }
        void Init(IRuntime runtime);
        void Shutdown();
        void Reload();

        IConsoleCommandCaller GetConsoleCaller();
    }
}