using System;

namespace Rocket.API.Permissions
{
    /// <summary>
    ///     Defines an object which can have permissions.
    /// </summary>
    public interface IPermissible : IFormattable, IIdentifiable
    {
        /// <summary>
        ///     The name of the object.
        /// </summary>
        string Name { get; }
    }
}