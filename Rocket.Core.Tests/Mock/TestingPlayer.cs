using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;

namespace Rocket.Core.Tests
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
    }
}
