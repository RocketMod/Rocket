//using Rocket.API;
//using Rocket.API.Commands;
//using Rocket.API.Exceptions;
//using Rocket.API.Extensions;
//using Rocket.API.Providers;
//using Rocket.Core;
//using Rocket.Core.Commands;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Rocket.API.Player;
//using Rocket.Core.Extensions;

//namespace Rocket.Core.Commands
//{
//    public class CommandP : IRocketCommand
//    {
//        public AllowedCaller AllowedCaller
//        {
//            get
//            {
//                return AllowedCaller.Both;
//            }
//        }

//        public string Name
//        {
//            get { return "p"; }
//        }

//        public string Help
//        {
//            get { return "Sets a Rocket permission group of a specific player"; }
//        }

//        public string Syntax
//        {
//            get { return "<player> [group] | reload"; }
//        }

//        public List<string> Aliases
//        {
//            get { return new List<string>() { "permissions" }; }
//        }

//        public List<string> Permissions
//        {
//            get { return new List<string>() { "rocket.p", "rocket.permissions" }; }
//        }

//        public void Execute(IRocketPlayer caller, string[] command)
//        {
//            if(command.Length == 1 && command[0].ToLower() == "reload" && caller.HasPermission("p.reload"))
//            {
//                R.Permissions.Reload();
//                R.Implementation.Chat.Say(caller, R.Translate("command_p_permissions_reload"));
//                return;
//            }




//            if (command.Length == 0 && !(caller is ConsolePlayer))
//            {
//                R.Implementation.Chat.Say(caller, R.Translate("command_p_groups_private", "Your", string.Join(", ", R.Permissions.GetGroups(caller).Select(g => g.DisplayName).ToArray())));
//                R.Implementation.Chat.Say(caller, R.Translate("command_p_permissions_private", "Your", string.Join(", ", R.Permissions.GetPermissions(caller).ToArray())));
//            }
//            else if(command.Length == 1) {

//                IRocketPlayer player = command.GetRocketPlayerParameter(0);
//                if (player != null) {
//                    R.Implementation.Chat.Say(caller, R.Translate("command_p_groups_private", player.DisplayName+"s", string.Join(", ", R.Permissions.GetGroups(player).Select(g => g.DisplayName).ToArray())));
//                    R.Implementation.Chat.Say(caller, R.Translate("command_p_permissions_private", player.DisplayName + "s", string.Join(", ", R.Permissions.GetPermissions(player).ToArray())));
//                }
//                else
//                {
//                    R.Implementation.Chat.Say(caller, R.Translate("command_generic_invalid_parameter"));
//                    return;
//                }
//            }
//            else if (command.Length == 3)
//            {
//                string c = command.GetStringParameter(0).ToLower();

//                IRocketPlayer player = command.GetRocketPlayerParameter(0);

//                string groupName = command.GetStringParameter(2);
                
//                switch (c)
//                {
//                    case "add":
//                        if (caller.HasPermission("p.add")&& player != null && groupName != null) {

//                            if (R.Permissions.AddPlayerToGroup(groupName, player))
//                            {
//                                R.Implementation.Chat.Say(caller, R.Translate("command_p_group_player_removed", player.DisplayName, groupName));
//                            }
//                            else
//                            {
//                                R.Implementation.Chat.Say(caller, R.Translate("command_p_unknown_error", player.DisplayName, groupName));
//                            }
//                        }
//                        return;
//                    case "remove":
//                        if (caller.HasPermission("p.remove") && player != null && groupName != null) {
//                            if (R.Permissions.RemovePlayerFromGroup(groupName, player))
//                            {
//                                R.Implementation.Chat.Say(caller, R.Translate("command_p_group_player_removed", player.DisplayName, groupName));
//                            }
//                            else
//                            {
//                                R.Implementation.Chat.Say(caller, R.Translate("command_p_unknown_error", player.DisplayName, groupName));
//                            }
//                        }
//                        return;
//                    default:
//                        R.Implementation.Chat.Say(caller, R.Translate("command_generic_invalid_parameter"));
//                        throw new WrongUsageOfCommandException(caller, this);
//                }


//            }
//            else
//            {
//                R.Implementation.Chat.Say(caller, R.Translate("command_generic_invalid_parameter"));
//                throw new WrongUsageOfCommandException(caller, this);
//            }

            
//         }
//    }
//}
