using System;

namespace Rocket.API.User
{
    /// <summary>
    ///     A message communication User.
    /// </summary>
    public interface IUser : IUserInfo
    {
        IUserManager UserManager { get; }

        bool IsOnline { get; }

        /// <summary>
        ///     The User connect time.
        /// </summary>
        DateTime SessionConnectTime { get; }

        /// <summary>
        ///     The User disconnect time.
        /// </summary>
        DateTime? SessionDisconnectTime { get; }

        string UserType { get; }
    }
}