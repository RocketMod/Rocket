using System;

namespace Rocket.API.Permissions
{
    public interface IPermissionGroup: IFormattable
    {
        string Id { get; }
        string Name { get; }
        int Priority { get; }
    }
}