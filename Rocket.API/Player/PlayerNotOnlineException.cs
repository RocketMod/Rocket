using System;
using Rocket.API.Commands;

namespace Rocket.API.Player
{
    /// <summary>
    ///     Thrown when an IPlayer is no longer online but methods are used on it that require it to be online.
    /// </summary>
    public abstract class PlayerNotOnlineException : Exception, ICommandFriendlyException
    {
        /// <param name="nameOrId">The name or ID of the player which was not found.</param>
        protected PlayerNotOnlineException(string nameOrId)
            : base(string.IsNullOrEmpty(nameOrId)
                ? "The requested player is no longer online."
                : $"The requested player: \"{nameOrId}\" is no longer online.")
        {
        }

        /// <inheritdoc/>
        public void SendErrorMessage(ICommandContext context)
        {
            context.Caller.UserManager.SendMessage(context.Caller, Message, ConsoleColor.Red);
        }
    }
}