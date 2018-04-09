using System.Collections.Generic;
using Rocket.API.Eventing;

namespace Rocket.API
{
    public interface IImplementation : IEventEmitter
    {
        IEnumerable<string> Capabilities { get; }
        string InstanceId { get; }
        void Load(IRuntime runtime);
        void Shutdown();
        void Reload();
    }
}