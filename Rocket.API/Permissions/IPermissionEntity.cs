using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.API.Permissions
{
    /// <summary>
    /// Represents an object that can have permissions.
    /// </summary>
    public interface IPermissionEntity
    {
        /// <summary>
        /// The unique and persistent ID of the entity.
        /// </summary>
        string Id { get; }
    }
}
