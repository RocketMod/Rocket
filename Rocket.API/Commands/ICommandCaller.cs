using Rocket.API.Permissions;

namespace Rocket.API.Commands
{
    public interface ICommandCaller : IPermissionable
    {
        string Name { get; }

        void SendMessage(string message);
    }
}