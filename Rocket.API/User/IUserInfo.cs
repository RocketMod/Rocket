namespace Rocket.API.User
{
    public interface IUserInfo: IIdentity
    {
        IUserManager UserManager { get; }
    }
}