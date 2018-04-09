using System;
using Rocket.API.Permissions;

namespace Rocket.API.Commands
{
    public interface ICommandCaller : IIdentifiable
    {
        string Name { get; }
        Type PlayerType { get; }

        void SendMessage(string message);
    }
}