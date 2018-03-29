using System;

namespace Rocket.API.Player
{
    public interface IPlayer : IComparable<IPlayer>
    {
        string UniqueID { get; }
        string DisplayName { get; }
        string IsAdmin { get; }
    }
}
