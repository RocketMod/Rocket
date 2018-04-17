namespace Rocket.API.Permissions
{
    public interface IPermissionGroup: IPermissible
    {
        int Priority { get; }
    }
}