//using Rocket.API;
//using Rocket.API.Commands;
//using Rocket.Core;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Reflection;
//using Rocket.API.Player;
//using Rocket.API.Providers.Plugins;

//namespace Rocket.Core.Commands
//{
//    public class CommandHelp : IRocketCommand
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
//            get { return "help"; }
//        }

//        public string Help
//        {
//            get { return "Shows you a specific help"; }
//        }

//        public string Syntax
//        {
//            get { return "[command]"; }
//        }

//        public List<string> Aliases
//        {
//            get { return new List<string>(); }
//        }

//        public List<string> Permissions
//        {
//            get { return new List<string>() { "rocket.help" }; }
//        }

//        public void Execute(IRocketPlayer caller, string[] command)
//        {
//            ReadOnlyCollection<IRocketCommand> commands = R.co();
//            if (command.Length == 0)
//            {
//                foreach (IRocketPluginProvider pluginManager in R.PluginProviders)
//                {
//                    string name = pluginManager.GetType().Assembly.GetName().Name;
//                    Console.ForegroundColor = ConsoleColor.Cyan;
//                    Console.WriteLine("[" + name + "]");
//                    Console.ForegroundColor = ConsoleColor.White;
//                    pluginManager.Commands.OrderBy(c => c.Name).All(c => { System.Console.WriteLine(c.Name.ToLower().PadRight(20, ' ') + " " + c.Syntax.ToLower()); return true; });
//                    Console.WriteLine();
//                }
//            }
//            else
//            {
//                IRocketCommand cmd = commands.Where(c => (String.Compare(c.Name, command[0], true) == 0)).FirstOrDefault();
//                if (cmd != null)
//                {
//                    string commandName = cmd.GetType().Assembly.GetName().Name + " / " + cmd.Name;

//                    Console.ForegroundColor = ConsoleColor.Cyan;
//                    Console.WriteLine("[" + commandName + "]");
//                    Console.ForegroundColor = ConsoleColor.White;
//                    Console.WriteLine(cmd.Name + "\t\t" + cmd.Syntax);
//                    Console.WriteLine(cmd.Help);
//                }
//            }
//        }
//    }
//}