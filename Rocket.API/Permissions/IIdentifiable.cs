using System;

namespace Rocket.API.Permissions
{
    /// <summary>
    ///     Defines an identifiable object.
    /// </summary>
    public interface IIdentifiable : IComparable, IComparable<IIdentifiable>, IEquatable<IIdentifiable>,
        IComparable<string>, IEquatable<string>
    {
        /// <summary>
        ///     The ID of the object.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The name of the object.
        /// </summary>
        string Name { get; }
    }
}