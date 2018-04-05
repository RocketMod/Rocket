namespace Rocket.API.Permissions
{
    public interface IPermissionProvider
    {
        bool HasPermissions(IIdentifiable identifiable);
    }
}