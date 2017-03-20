using System;
using Rocket.API.Player;

namespace Rocket.Core.Tests.Mock
{
    public class TestingPlayer : IRocketPlayer
    {
        public string DisplayName
        {
            get
            {
                return "TestingPlayer";
            }
        }
        public TestingPlayer(string id= "1", bool admin= false)
        {
            IsAdmin = admin;
            Id = id;
        }

        private bool isAdmin;
        public bool IsAdmin
        {
            get
            {
                return isAdmin;
            }
            set { isAdmin = value; }
        }

        private string id;

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(((IRocketPlayer)obj).Id);
        }

        public void Kick(string message)
        {
            throw new NotImplementedException();
        }

        public void Ban(string message, uint duration = 0)
        {
            throw new NotImplementedException();
        }
    }
}
