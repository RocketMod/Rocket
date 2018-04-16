using Rocket.API.Commands;

namespace Rocket.Core.Exceptions
{
    public interface ICommandFriendlyException
    {
        void SendErrorMessage(ICommandContext context);
    }
}