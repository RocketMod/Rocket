using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.Entities;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.Player;
using Rocket.Core.User;

namespace Rocket.Examples.CommandsExample
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
        public async Task KillPlayer(IUser sender, IPlayer target)
        {
            if (target.IsOnline && target.GetEntity() is ILivingEntity entity)
                await entity.KillAsync(sender);
            else // the game likely does not support killing players (e.g. Eco)
                await sender.SendMessageAsync("Target could not be killed :(");
        }

        [Command(Name = "Broadcast", Summary = "Broadcasts a message.")]
        [CommandAlias("bc")]
        [CommandUser(typeof(IConsole))] // only console can call it
        public void Broadcast(IUser sender, string[] args)
        {
            userManager.BroadcastAsync(sender, translations.GetAsync("broadcast", sender, string.Join(" ", args)).Result);
        }
    }
}