using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core
{
    public struct Cooldown
    {
        public IPlayer Player;
        public Permission Permission;
        public DateTime LastUTCExecution;

        public bool IsExpired => (DateTime.UtcNow - LastUTCExecution).TotalSeconds > Permission.Cooldown;
    }
}
