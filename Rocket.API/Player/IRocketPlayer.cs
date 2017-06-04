using System;
using UnityEngine;

namespace Rocket.API.Player
{
    public interface IRocketPlayer : IComparable
    {
        string Id { get; }
        string DisplayName { get; }
        bool IsAdmin { get; }
        void Kick(string message);
        void Ban(string message, uint duration = 0);
        bool HasPermission(string permission);
        void Message(string message, Color? color);
    }
}