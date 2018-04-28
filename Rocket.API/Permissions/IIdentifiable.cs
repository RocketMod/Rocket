using System;

namespace Rocket.API.Permissions
{
    /// <summary>
    ///     Defines an identifieable object.
    /// </summary>
    public interface IIdentifiable : IComparable, IComparable<IIdentifiable>, IEquatable<IIdentifiable>,
        IComparable<string>, IEquatable<string>
    {
        /// <summary>
        ///     The ID of the object.
        /// </summary>
        string Id { get; }
    }
}