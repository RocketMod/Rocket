namespace Rocket.API.Permissions
{
    public interface IPermissionGroup : IPermissible
    {
        string Name { get; }
        int Priority { get; }
    }
}