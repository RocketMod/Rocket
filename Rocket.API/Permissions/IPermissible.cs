using System;

namespace Rocket.API.Permissions
{
    public interface IPermissible: IFormattable, IIdentifiable
    {
        string Name { get; }
    }
}