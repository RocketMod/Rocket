using System;

namespace Rocket.API.Permissions
{
    public interface IPermissible: IFormattable
    {
        string Id { get; }
        string Name { get; }
    }
}