using System;
using Rocket.API.DependencyInjection;

namespace Rocket.API.User
{
    /// <summary>
    ///     A message communication User.
    /// </summary>
    public interface IUser : IUserInfo
    {
        /// <summary>
        ///     Checks if the user is online.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        ///     The User connect time.
        /// </summary>
        DateTime SessionConnectTime { get; }

        /// <summary>
        ///     The User disconnect time.
        /// </summary>
        DateTime? SessionDisconnectTime { get; }

        /// <summary>
        ///     Gets the last time the user was online on the server or null if the user was never online before.
        /// </summary>
        DateTime? LastSeen { get; }

        /// <summary>
        ///     The user type (e.g. "DiscordUser", "Unturned", etc...)
        /// </summary>
        string UserType { get; }

        /// <summary>
        ///     The distinct dependency container of the user.
        /// </summary>
        IDependencyContainer Container { get; }
    }
}