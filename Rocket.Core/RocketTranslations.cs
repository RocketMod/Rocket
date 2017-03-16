using Rocket.API.Collections;
using Rocket.API.Serialisation;

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
                { "command_rocket_plugins_loaded","Loaded: {0}"},
                { "command_rocket_plugins_unloaded","Unloaded: {0}"},
                { "command_rocket_plugins_failure","Failure: {0}"},
                { "command_rocket_plugins_cancelled","Cancelled: {0}"},
                { "command_rocket_reload_plugin","Reloading {0}"},
                { "command_rocket_not_loaded","The plugin {0} is not loaded"},
                { "command_rocket_unload_plugin","Unloading {0}"},
                { "command_rocket_load_plugin","Loading {0}"},
                { "command_rocket_already_loaded","The plugin {0} is already loaded"},
                { "command_rocket_reload","Reloading Rocket"},
                { "command_p_group_not_found","Group not found"},
                { "command_p_group_player_added","{0} was added to the group {1}"},
                { "command_p_group_player_removed","{0} was removed from from the group {1}"},
                { "command_p_unknown_error","Unknown error"},
                { "command_p_player_not_found","{0} was not found"},
                { "command_p_group_not_found","{1} was not found"},
                { "command_p_duplicate_entry","{0} is already in the group {1}"},
                { "command_p_permissions_reload","Permissions reloaded"},
                { "invalid_character_name","invalid character name"},
                { "command_not_found","Command not found."},
                { "command_cooldown","You have to wait {0} seconds before you can use this command again."}
            });
        }
    }
}
