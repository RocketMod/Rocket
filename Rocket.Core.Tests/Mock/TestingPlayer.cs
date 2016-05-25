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
        private static TestingPlayer instance = null;
        public static TestingPlayer GetPlayer()
        {
            if (instance == null) instance = new TestingPlayer();
            return instance;
        }

        public string DisplayName
        {
            get
            {
                return "TestingPlayer";
            }
        }

        public string Id
        {
            get
            {
                return "12345678901234567";
            }
        }

        private bool isAdmin = false;
        public bool IsAdmin
        {
            get
            {
                return isAdmin;
            }
            set { isAdmin = value; }
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(((IRocketPlayer)obj).Id);
        }
    }
}
