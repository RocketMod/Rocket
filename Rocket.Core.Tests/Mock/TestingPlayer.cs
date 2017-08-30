using System;
using Rocket.API.Player;
using UnityEngine;

namespace Rocket.Core.Tests.Mock
{
    public class TestingPlayer : IPlayer
    {
        public string DisplayName => "TestingPlayer";

        public TestingPlayer(string id= "1", bool admin= false)
        {
            IsAdmin = admin;
            Id = id;
        }

        public bool IsAdmin { get; set; }

        public string Id { get; set; }

        public int CompareTo(object obj)
        {
            return String.Compare(Id, ((IPlayer)obj).Id, StringComparison.Ordinal);
        }

        public void Kick(string message)
        {
            throw new NotImplementedException();
        }

        public void Ban(string message, uint duration = 0)
        {
            throw new NotImplementedException();
        }

        public bool HasPermission(string permission)
        {
            throw new NotImplementedException();
        }

        public void Message(string message, Color? color)
        {
            throw new NotImplementedException();
        }
    }
}
