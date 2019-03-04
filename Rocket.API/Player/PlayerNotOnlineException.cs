using Rocket.API.Commands;
using System;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace Rocket.API.Player
{
    /// <summary>
    ///     Thrown when an IPlayer is no longer online but methods are used on it that require it to be online.
    /// </summary>
    public class PlayerNotOnlineException : Exception, ICommandFriendlyException
    {
        /// 
        public PlayerNotOnlineException() : this(null) { }

        /// <param name="nameOrId">The name or ID of the player which was not found.</param>
        public PlayerNotOnlineException(string nameOrId)
            : base(string.IsNullOrEmpty(nameOrId)
                ? "The requested player is not online."
                : $"The requested player: \"{nameOrId}\" is not online.")
        { }

        /// <inheritdoc />
        public async Task SendErrorMessageAsync(ICommandContext context)
        {
            await context.User.UserManager.SendMessageAsync(null, context.User, Message, Color.Red);
        }
    }
}