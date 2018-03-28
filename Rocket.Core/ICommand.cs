using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core
{
    public interface ICommand
    {
        //If this is true, Rocket won't register the cooldown, it requires the command to call the methods itself.
        //This is to prevent a cooldown from being registered if the command were to fail.
        bool SelfManagedCooldown { get; }

        //If this is empty, the command should handle the permissions itself.
        string[] Permissions { get; }
        
        void Execute(ICommandContext context);
    }
}
