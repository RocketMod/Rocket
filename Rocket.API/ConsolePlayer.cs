using System;
using System.Collections.Generic;

namespace Rocket.API
{
    public class ConsolePlayer : IRocketPlayer
    {
        string IRocketPlayer.Id
        {
            get
            {
                return "Console";
            }
        }

        string IRocketPlayer.DisplayName
        {
            get
            {
                return "Console";
            }
        }

        public bool IsAdmin
        {
            get
            {
                return true;
            }
        }
    }
}
