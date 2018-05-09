using Rocket.API.Commands;
using Rocket.API.Entities;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.User;

namespace Rocket.Examples.CommandsPlugin
{
    public class CommandsCollection
    {
        private readonly IUserManager userManager;
        private readonly ITranslationCollection translations;

        public CommandsCollection(IUserManager userManager, ITranslationCollection translations)
        {
            this.userManager = userManager;
            this.translations = translations;
        }

        [Command(Summary = "Kills a player.")] //By default, name is the same as the method name, and it will support all command callers
        public void KillPlayer(IUser sender, IPlayer target)
        {
            if (target.IsOnline && target.Entity is ILivingEntity entity)
                entity.Kill(sender);
            else // the game likely does not support killing players (e.g. Eco)
                sender.SendMessage("Target could not be killed :(");
        }

        [Command(Name = "Broadcast", Summary = "Broadcasts a message.")]
        [CommandAlias("bc")]
        [CommandUser(typeof(IConsole))] // only console can call it
        public void Broadcast(IUser sender, string[] args)
        {
            userManager.Broadcast(sender, translations.Get("broadcast", sender, string.Join(" ", args)));
        }
    }
}