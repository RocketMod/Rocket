using System;
using System.Collections.Generic;
using System.Drawing;
using Rocket.API.DependencyInjection;

namespace Rocket.API.User
{
    /// <summary>
    ///     The service responsible for managing users.
    /// </summary>
    public interface IUserManager: IProxyableService
    {
        /// <summary>
        ///     The online users.
        /// </summary>
        IEnumerable<IUser> OnlineUsers { get; }

        /// <summary>
        ///     Kicks (disconnects) the given user from the server.
        /// </summary>
        /// <param name="user">The user to kick.</param>
        /// <param name="kickedBy">The user which kicks (optional).</param>
        /// <param name="reason">The kick reason whicht might be shown to the user (optional).</param>
        /// <returns><b>true</b> if the user could be kicked; otherwise, <b>false</b>.</returns>
        bool Kick(IUser user, IUser kickedBy = null, string reason = null);

        /// <summary>
        ///     Bans the given user from the server.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="bannedBy">The user which bans (optional).</param>
        /// <param name="reason">The ban reason which might be shown to the user (optional).</param>
        /// <param name="duration">The ban duration. Will never expire if null.</param>
        /// <returns><b>true</b> if the user could be banned; otherwise, <b>false</b>.</returns>
        bool Ban(IUserInfo user, IUser bannedBy = null, string reason = null, TimeSpan? duration = null);

        /// <summary>
        ///     Unbans the given user from the server.
        /// </summary>
        /// <param name="user">The user to unban.</param>
        /// <param name="unbannedBy">The user which unbans.</param>
        /// <returns><b>true</b> if the user could be unbanned; otherwise, <b>false</b>.</returns>
        bool Unban(IUserInfo user, IUser unbannedBy = null);

        /// <summary>
        ///     Sends a message to the given User.
        /// </summary>
        /// <param name="color">The message color.</param>
        /// <param name="sender">The sender of the message (optional).</param>
        /// <param name="receiver">The receiver of the message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" />.</param>
        void SendMessage(IUser sender, IUser receiver, string message, Color? color = null, params object[] arguments);

        /// <summary>
        ///     Sends a message without sender to the given Users.
        /// </summary>
        /// <param name="sender">The sender of the message (optional).</param>
        /// <param name="receivers">The receivers of the message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The message color.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" />.</param>
        void Broadcast(IUser sender, IEnumerable<IUser> receivers, string message, Color? color = null, params object[] arguments);

        /// <summary>
        ///     Broadcasts the given message.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="color">The message color.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" />.</param>
        void Broadcast(IUser sender, string message, Color? color = null, params object[] arguments);

        /// <summary>
        ///     Gets the given online or offline user info.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The user info.</returns>
        IUserInfo GetUser(string id);
    }
}