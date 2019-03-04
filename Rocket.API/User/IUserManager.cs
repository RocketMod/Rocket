using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using Rocket.API.DependencyInjection;
using Color = System.Drawing.Color;
using Rocket.API.Player;

namespace Rocket.API.User
{
    /// <summary>
    ///     The service responsible for managing users.
    /// </summary>
    public interface IUserManager: IProxyableService, IIdentityProvider
    {
        /// <summary>
        ///     Bans the given user from the server.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="bannedBy">The user which bans (optional).</param>
        /// <param name="reason">The ban reason which might be shown to the user (optional).</param>
        /// <param name="duration">The ban duration. Will never expire if null.</param>
        /// <returns><b>true</b> if the user could be banned; otherwise, <b>false</b>.</returns>
        Task<bool> BanAsync(IUser user, IUser bannedBy = null, string reason = null, TimeSpan? duration = null);

        /// <summary>
        ///     Unbans the given user from the server.
        /// </summary>
        /// <param name="user">The user to unban.</param>
        /// <param name="unbannedBy">The user which unbans.</param>
        /// <returns><b>true</b> if the user could be unbanned; otherwise, <b>false</b>.</returns>
        Task<bool> UnbanAsync(IUser user, IUser unbannedBy = null);

        /// <summary>
        ///     Kicks (disconnects) the given user from the server.
        /// </summary>
        /// <param name="user">The user to kick.</param>
        /// <param name="kickedBy">The user which kicks (optional).</param>
        /// <param name="reason">The kick reason whicht might be shown to the user (optional).</param>
        /// <returns><b>true</b> if the user could be kicked; otherwise, <b>false</b>.</returns>
        Task<bool> KickAsync(IUser user, IUser kickedBy = null, string reason = null);

        /// <summary>
        ///     Sends a message to the given User.
        /// </summary>
        /// <param name="color">The message color.</param>
        /// <param name="sender">The sender of the message (optional).</param>
        /// <param name="receiver">The receiver of the message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" />.</param>
        Task SendMessageAsync(IUser sender, IUser receiver, string message, Color? color = null, params object[] arguments);

        /// <summary>
        ///     Sends a message without sender to the given Users.
        /// </summary>
        /// <param name="sender">The sender of the message (optional).</param>
        /// <param name="receivers">The receivers of the message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The message color.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" />.</param>
        Task BroadcastAsync(IUser sender, IEnumerable<IUser> receivers, string message, Color? color = null, params object[] arguments);

        /// <summary>
        ///     Broadcasts the given message.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="color">The message color.</param>
        /// <param name="arguments">The arguments for the message. See <see cref="string.Format(string, object[])" />.</param>
        Task BroadcastAsync(IUser sender, string message, Color? color = null, params object[] arguments);

        /// <summary>
        ///     Gets the given user info.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The user info.</returns>
        Task<IUser> GetUserAsync(string id);
    }
}