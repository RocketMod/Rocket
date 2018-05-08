using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.User
{
    public interface IUserManager
    {
        IEnumerable<IUser> Users { get; }

        /// <summary>
        ///     Kicks (disconnects) the given player from the server.
        /// </summary>
        /// <param name="player">The player to kick.</param>
        /// <param name="caller">The command caller which kicks the player (optional).</param>
        /// <param name="reason">The kick reason whicht might be shown to the player (optional).</param>
        /// <returns><b>true</b> if the player could be kicked; otherwise, <b>false</b>.</returns>
        bool Kick(IUser player, IUser caller = null, string reason = null);

        /// <summary>
        ///     Bans the given player from the server.
        /// </summary>
        /// <param name="player">The player to ban.</param>
        /// <param name="caller">The command caller which bans the player (optional).</param>
        /// <param name="reason">The ban reason which might be shown to the player (optional).</param>
        /// <param name="timeSpan">The ban duration. Will never expire if null.</param>
        /// <returns><b>true</b> if the player could be banned; otherwise, <b>false</b>.</returns>
        bool Ban(IUser player, IUser caller = null, string reason = null, TimeSpan? timeSpan = null);

        /// <summary>
        ///     Unbans the given player from the server.
        /// </summary>
        /// <param name="player">The player to unban.</param>
        /// <param name="caller">The command caller which unbans the player.</param>
        /// <returns><b>true</b> if the player could be unbanned; otherwise, <b>false</b>.</returns>
        bool Unban(IUser player, IUser caller = null);

        /// <summary>
        ///     Sends a message to the given User.
        /// </summary>
        /// <param name="sender">The sender of the message (optional).</param>
        /// <param name="receiver">The receiver of the message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])"/>.</param>
        void SendMessage(IUser sender, IUser receiver, string message, params object[] arguments);

        /// <summary>
        ///     Sends a message without sender to the given Users.
        /// </summary>
        /// <param name="sender">The sender of the message (optional).</param>
        /// <param name="receivers">The receivers of the message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])"/>.</param>
        void SendMessage(IUser sender, IEnumerable<IUser> receivers, string message, params object[] arguments);

        /// <summary>
        ///     Sends a message to all Users.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])"/>.</param>
        void SendMessage(IUser sender, string message, params object[] arguments);
    }
}
