using Rocket.API.Collections;

namespace Rocket.API
{
    public class RocketTranslations : TranslationList
    {
        public RocketTranslations()
        {
            this.AddRange( new TranslationList()
            {
                { "rocket_join_public","{0} connected to the server" },
                { "rocket_leave_public","{0} disconnected from the server"},
                { "command_no_permission","You do not have permissions to execute this command."},
                { "command_cooldown","You have to wait {0} seconds before you can use this command again."}
            });
        }
    }
}
