using System;

namespace Rocket.API
{
    public interface IRocketPlayer : IComparable
    {
        string Id { get; }
        string DisplayName { get; }
        bool IsAdmin { get; }
    }
}