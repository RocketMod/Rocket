using System.Collections.Generic;

namespace Rocket.API
{
    public interface IImplementation : ILifecycleObject
    {
        IEnumerable<string> Capabilities { get; }
        string InstanceId { get; }
        void Load(IRuntime runtime);
        void Shutdown();
        void Reload();
    }
}