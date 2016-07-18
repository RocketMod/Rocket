using Rocket.API.Serialisation;
using System;

namespace Rocket.API.Commands
{
    public class RocketCommandCooldown
    {
        public IRocketPlayer Player;
        public DateTime CommandRequested;
        public IRocketCommand Command;
        public Permission ApplyingPermission;

        public RocketCommandCooldown(IRocketPlayer player, IRocketCommand command, Permission applyingPermission)
        {
            Player = player;
            Command = command;
            CommandRequested = DateTime.Now;
            ApplyingPermission = applyingPermission;
        }
    }
}