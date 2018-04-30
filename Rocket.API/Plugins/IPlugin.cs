using Rocket.API.Eventing;

namespace Rocket.API.Plugins
{
    public interface IPlugin : IEventEmitter
    {
        string WorkingDirectory { get; }

        void Activate();
        void Deactivate();
        void Reload();
    }
}