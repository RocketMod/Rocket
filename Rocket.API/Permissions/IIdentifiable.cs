using System;

namespace Rocket.API.Permissions
{
    public interface IIdentifiable : IComparable, IComparable<IIdentifiable>, IEquatable<IIdentifiable>, IComparable<string>, IEquatable<string>
    {
        string Id { get; set; }
    }
}