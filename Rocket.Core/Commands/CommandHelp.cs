using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Rocket.Unturned.Commands
{
    public class CommandHelp : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Both;
            }
        }

        public string Name
        {
            get { return "help"; }
        }

        public string Help
        {
            get { return "Shows you a specific help"; }
        }

        public string Syntax
        {
            get { return "[command]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.help" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            ReadOnlyCollection<IRocketCommand> commands = R.Instance.GetAllCommands();
            if (command.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[Vanilla]");
                Console.ForegroundColor = ConsoleColor.White;
                commands.OrderBy(c => c.Name).All(c => { System.Console.WriteLine(c.Name.ToLower().PadRight(20, ' ') + " " + c.Syntax.Replace(c.Name, "").TrimStart().ToLower()); return true; });

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[Rocket]");
                Console.ForegroundColor = ConsoleColor.White;
                commands.Where(c => c.GetType().Assembly == Assembly.GetExecutingAssembly()).OrderBy(c => c.Name).All(c => { System.Console.WriteLine(c.Name.ToLower().PadRight(20, ' ') + " " + c.Syntax.ToLower()); return true; });

                Console.WriteLine();

                foreach (IRocketPluginManager pluginManager in R.Instance.PluginManagers)
                {
                    string name = pluginManager.GetType().Assembly.GetName().Name;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[" + name + "]");
                    Console.ForegroundColor = ConsoleColor.White;
                    pluginManager.Commands.OrderBy(c => c.Name).All(c => { System.Console.WriteLine(c.Name.ToLower().PadRight(20, ' ') + " " + c.Syntax.ToLower()); return true; });
                    Console.WriteLine();
                }
            }
            else
            {
                IRocketCommand cmd = commands.Where(c => (String.Compare(c.Name, command[0], true) == 0)).FirstOrDefault();
                if (cmd != null)
                {
                    string commandName = cmd.GetType().Assembly.GetName().Name + " / " + cmd.Name;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[" + commandName + "]");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(cmd.Name + "\t\t" + cmd.Syntax);
                    Console.WriteLine(cmd.Help);
                }
            }
        }
    }
}