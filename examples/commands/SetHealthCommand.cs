using System;
using Rocket.API.Commands;
using Rocket.API.Entities;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.User;

namespace Rocket.Examples.CommandsPlugin
{
    public class SetHealthCommand : ICommand
    {
        public string Name => "SetHealth";
        public string Summary => "Sets health of players.";
        public string Description => null;
        public string Permission => "Rocket.Examples.SetHealth";
        public string Syntax => "<target player> <health>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => new[] {"sh"};

        public bool SupportsUser(Type user) => true;

        public void Execute(ICommandContext context)
        {
            IPlayer target = context.Parameters.Get<IPlayer>(0); // target player is first parameter
            double health = context.Parameters.Get<double>(1);               // health is second parameter

            if (target.IsOnline && target.Entity is ILivingEntity entity)
                entity.Health = health;
            else // the game likely does not support killing players (e.g. Eco)
                context.User.SendMessage("Target could not be killed :(");
        }
    }
}