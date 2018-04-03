namespace Rocket.API.Permissions
{
    public interface IPermissionable
    {
        bool HasPermission(string permission);
    }
}