using System;
using System.Drawing;
using Rocket.API.Commands;

namespace Rocket.API.Player
{
    /// <summary>
    ///     Thrown when a player was not found using an ID.
    /// </summary>
    public class PlayerIdNotFoundException : PlayerNotFoundException
    {
        /// <param name="playerId">The player ID.</param>
        public PlayerIdNotFoundException(string playerId) : base(playerId)
        {
            PlayerId = playerId;
        }

        /// <summary>
        ///     The player ID.
        /// </summary>
        public string PlayerId { get; }
    }

    /// <summary>
    ///     Thrown when a player was not found using a name.
    /// </summary>
    public class PlayerNameNotFoundException : PlayerNotFoundException
    {
        ///<param name="playerName">The player name.</param>
        public PlayerNameNotFoundException(string playerName) : base(playerName)
        {
            PlayerName = playerName;
        }

        /// <summary>
        ///     The player name.
        /// </summary>
        public string PlayerName { get; }
    }

    /// <summary>
    ///     Thrown when a player was not found.
    /// </summary>
    public abstract class PlayerNotFoundException : Exception, ICommandFriendlyException
    {
        /// <param name="nameOrId">The name or ID of the player which was not found.</param>
        protected PlayerNotFoundException(string nameOrId)
            : base(string.IsNullOrEmpty(nameOrId)
                ? "The requested player was not found."
                : $"The requested player: \"{nameOrId}\" was not found.")
        {
        }

        /// <inheritdoc/>
        public void SendErrorMessage(ICommandContext context)
        {
            context.Caller.UserManager.SendMessage(context.Caller, Message, Color.Red);
        }
    }
}