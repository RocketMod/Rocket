using System;
using Rocket.API.Commands;

namespace Rocket.API.Player
{
    public interface IPlayer : IComparable<IPlayer>, IComparable<string>, IEquatable<IPlayer>, IEquatable<string>,
        ICommandCaller
    {
        string UniqueID { get; }
        string Name { get; }
        bool IsAdmin { get; }
    }
}