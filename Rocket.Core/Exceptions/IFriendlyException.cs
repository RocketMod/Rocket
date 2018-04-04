using Rocket.API.Commands;

namespace Rocket.Core.Exceptions {
    public interface IFriendlyException {
        string ToFriendlyString(ICommandContext context);
    }
}