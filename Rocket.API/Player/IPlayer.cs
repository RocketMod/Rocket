using System;

namespace Rocket.API.Player
{
    public interface IPlayer : IComparable<IPlayer>, IComparable<string>, IEquatable<IPlayer>, IEquatable<string>
    {
        string UniqueID { get; }
        string DisplayName { get; }
        bool IsAdmin { get; }
    }
}
