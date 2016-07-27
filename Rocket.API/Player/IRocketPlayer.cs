using System;

namespace Rocket.API
{
    public interface IRocketPlayer : IComparable
    {
        string Id { get; }
        string DisplayName { get; }
        bool IsAdmin { get; }
        void Kick(string message);
        void Ban(string message);
    }
}