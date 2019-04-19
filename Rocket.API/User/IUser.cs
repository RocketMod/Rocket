using System;
using System.Collections.Generic;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;

namespace Rocket.API.User
{
    /// <summary>
    ///     A message communication User.
    /// </summary>
    public interface IUser : IPermissionActor
    {
        /// <summary>
        ///     Gets the last time the user was online on the server or null if the user was never online before.
        /// </summary>
        DateTime? LastSeen { get; }

        /// <summary>
        ///     The related user manager.
        /// </summary>
        IUserManager UserManager { get; }

        string UserName { get; }

        string DisplayName { get; }

        List<IIdentity> Identities { get; }

        /// <summary>
        /// The user type (e.g. "Player", "Console", "Discord" etc)
        /// </summary>
        string UserType { get; }

        /// <summary>
        ///     The distinct dependency container of the user.
        /// </summary>
        IDependencyContainer Container { get; }
    }
}