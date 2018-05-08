namespace Rocket.API.User
{
    /// <summary>
    ///     Provides information for an online or offine user.
    /// </summary>
    public interface IUserInfo: IIdentity
    {
        /// <summary>
        ///     The related user manager.
        /// </summary>
        IUserManager UserManager { get; }
    }
}