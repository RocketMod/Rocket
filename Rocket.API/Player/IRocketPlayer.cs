using System;
using UnityEngine;

namespace Rocket.API.Player
{
    public interface IPlayer : IComparable
    {
        string Id { get; }
        string DisplayName { get; }
        bool IsAdmin { get; }
    }
}