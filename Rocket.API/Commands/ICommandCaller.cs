using Rocket.API.Permissions;

namespace Rocket.API.Commands {
    public interface ICommandCaller : IIdentifiable {
        string Name { get; }

        void SendMessage(string message);
    }
}