using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using UnityEngine;
using Rocket.API;
using System.Text.RegularExpressions;
using System.Reflection;
using Rocket.Core.Utils;
using Rocket.Core.Logging;
using Rocket.Core.Serialization;

namespace Rocket.Core.Commands
{
    public class RocketCommandManager : MonoBehaviour
    {
        private readonly List<IRocketCommand> commands = new List<IRocketCommand>();
        public ReadOnlyCollection<IRocketCommand> Commands { get; internal set; }

        private void Awake()
        {
            Commands = commands.AsReadOnly();
        }

        private IRocketCommand GetCommand(IRocketCommand command)
        {
           return GetCommand(command.Name);
        }

        private IRocketCommand GetCommand(string command)
        {
            IRocketCommand foundCommand = commands.Where(c => c.Name.ToLower() == command.ToLower()).FirstOrDefault();
            if(foundCommand == null) commands.Where(c => c.Aliases.Select(a => a.ToLower()).Contains(command.ToLower())).FirstOrDefault();
            return foundCommand;
        }

        private string getCommandClass(IRocketCommand command)
        {
            if (command is RocketAttributeCommand)
            {
                return ((RocketAttributeCommand)command).Method.ReflectedType.FullName;
            }
            else
            {
                return command.GetType().FullName;
            }
        }

        public bool Register(IRocketCommand command)
        {

            string name = command.Name;
            string className = getCommandClass(command);

            CommandMapping commandMapping = R.Settings.Instance.CommandMappings.Where(m => m.Class == className).FirstOrDefault();
            if (commandMapping != null) { name = commandMapping.Name;}

            List<CommandMapping> otherEnabledCommandWithSameName = R.Settings.Instance.CommandMappings.Where(m => m.Name == name && m.Enabled).ToList();
            if (otherEnabledCommandWithSameName.Count > 1)
            {
                Logger.Log("Found multiple active CommandMappings for /"+ command.Name + ", using first and disabling the others...");
                R.Settings.Instance.CommandMappings.Where(m => m.Name == name && m.Enabled).Skip(1).All(m => m.Enabled = false);
                R.Settings.Save();
            }

            CommandMapping mapping = R.Settings.Instance.CommandMappings.Where(m => m.Name == name && m.Enabled && m.Class != className).FirstOrDefault();
            
            if (mapping != null)
            {
                Logger.Log("Not registering " + className + " (" + command.Name + ") because it has been replaced by " + mapping.Class);
                if (commandMapping == null)
                {
                    R.Settings.Instance.CommandMappings.Add(new CommandMapping(name, false, className));
                    R.Settings.Save();
                }
                return false;
            }
            else
            {
                if (!commandMapping.Enabled)
                {
                    Logger.Log("Not registering " + getCommandClass(command) + " (" + name + ") because it has been disabled");
                    return false;
                }
                Logger.Log("Registering " + getCommandClass(command) + " (" + name + ")");
                if (commandMapping == null)
                {
                    R.Settings.Instance.CommandMappings.Add(new CommandMapping(name, true, className));
                    R.Settings.Save();
                }

                if (command.Name != name)
                {
                    commands.Add(new RenamedRocketCommand(name, command));
                }
                else
                {
                    commands.Add(command);
                }
                return true;
            }
        }

        public void Deregister(IRocketCommand command)
        {
            Logger.Log("Deregister " + command.GetType().FullName + " as "  + command.Name);
            commands.Remove(command);
        }

        public bool Execute(IRocketPlayer player, string command)
        {
            string[] commandParts = Regex.Matches(command, @"[\""](.+?)[\""]|([^ ]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Cast<Match>().Select(m => m.Value.Trim('"').Trim()).ToArray();

            if (commandParts.Length != 0)
            {
                name = commandParts[0];
                string[] parameters = commandParts.Skip(1).ToArray();
                if (player == null) player = new ConsolePlayer();
                IRocketCommand rocketCommand = GetCommand(name);
                if (rocketCommand != null)
                {
                    if (rocketCommand.AllowedCaller == AllowedCaller.Player && player is ConsolePlayer)
                    {
                        Logger.Log("This command can't be called from console");
                        return false;
                    }
                    if (rocketCommand.AllowedCaller == AllowedCaller.Console && !(player is ConsolePlayer))
                    {
                        Logger.Log("This command can only be called from console");
                        return false;
                    }
                    try
                    {
                        rocketCommand.Execute(player, parameters);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("An error occured while executing " + rocketCommand.Name + " [" + String.Join(", ", parameters) + "]: " + ex.ToString());
                    }
                    return true;
                }
            }

            return false;
        }

        public void RegisterFromAssembly(Assembly assembly)
        {
            List<Type> commands = RocketHelper.GetTypesFromInterface(assembly, "IRocketCommand");
            foreach (Type commandType in commands)
            {
                if(commandType.GetConstructor(Type.EmptyTypes) != null)
                {
                    IRocketCommand command = (IRocketCommand)Activator.CreateInstance(commandType);
                    Register(command);
                }
            }

            Type plugin = R.Plugins.GetMainTypeFromAssembly(assembly);
            if (plugin != null)
            {
                MethodInfo[] methodInfos = plugin.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (MethodInfo method in methodInfos)
                {
                    RocketCommandAttribute commandAttribute = (RocketCommandAttribute)Attribute.GetCustomAttribute(method, typeof(RocketCommandAttribute));
                    RocketCommandAliasAttribute[] commandAliasAttributes = (RocketCommandAliasAttribute[])Attribute.GetCustomAttributes(method, typeof(RocketCommandAliasAttribute));
                    RocketCommandPermissionAttribute[] commandPermissionAttributes = (RocketCommandPermissionAttribute[])Attribute.GetCustomAttributes(method, typeof(RocketCommandPermissionAttribute));

                    if (commandAttribute != null)
                    {
                        List<string> Permissions = new List<string>();
                        List<string> Aliases = new List<string>();

                        if (commandAliasAttributes != null)
                        {
                            foreach (RocketCommandAliasAttribute commandAliasAttribute in commandAliasAttributes)
                            {
                                Aliases.Add(commandAliasAttribute.Name);
                            }
                        }

                        if (commandPermissionAttributes != null)
                        {
                            foreach (RocketCommandPermissionAttribute commandPermissionAttribute in commandPermissionAttributes)
                            {
                                Aliases.Add(commandPermissionAttribute.Name);
                            }
                        }

                        IRocketCommand command = new RocketAttributeCommand(commandAttribute.Name, commandAttribute.Help, commandAttribute.Syntax, commandAttribute.AllowedCaller, Permissions, Aliases, method);
                        Register(command);
                    }
                }
            }
        }



        internal class RenamedRocketCommand : IRocketCommand
        {
            private IRocketCommand originalCommand;
            private string name;

            public RenamedRocketCommand(string name,IRocketCommand command)
            {
                this.name = name;
                this.originalCommand = command;
            }

            public List<string> Aliases
            {
                get
                {
                    return originalCommand.Aliases;
                }
            }

            public AllowedCaller AllowedCaller
            {
                get
                {
                    return originalCommand.AllowedCaller;
                }
            }

            public string Help
            {
                get
                {
                    return originalCommand.Help;
                }
            }

            public string Name
            {
                get
                {
                    return name;
                }
            }

            public List<string> Permissions
            {
                get
                {
                    return originalCommand.Permissions;
                }
            }

            public string Syntax
            {
                get
                {
                    return originalCommand.Syntax;
                }
            }

            public void Execute(IRocketPlayer caller, string[] command)
            {
                originalCommand.Execute(caller, command);
            }
        }

        internal class RocketAttributeCommand : IRocketCommand
        {
            internal RocketAttributeCommand(string Name,string Help,string Syntax,AllowedCaller AllowedCaller,List<string>Permissions,List<string>Aliases,MethodInfo Method)
            {
                name = Name;
                help = Help;
                syntax = Syntax;
                permissions = Permissions;
                aliases = Aliases;
                method = Method;
                allowedCaller = AllowedCaller;
            }

            private List<string> aliases;
            public List<string> Aliases{ get { return aliases; } }

            private AllowedCaller allowedCaller;
            public AllowedCaller AllowedCaller { get { return allowedCaller; } }

            private string help;
            public string Help { get { return help; } }

            private string name;
            public string Name { get { return name; } }

            private string syntax;
            public string Syntax { get { return syntax; } }

            private List<string> permissions;
            public List<string> Permissions { get { return permissions; } }

            private MethodInfo method;
            public MethodInfo Method { get { return method; } }
            public void Execute(IRocketPlayer caller, string[] parameters)
            {
                ParameterInfo[] methodParameters = method.GetParameters();
                switch (methodParameters.Length)
                {
                    case 0:
                        method.Invoke(R.Plugins.GetPlugin(method.ReflectedType.Assembly), null);
                        break;
                    case 1:
                        if (methodParameters[0].ParameterType == typeof(IRocketPlayer))
                            method.Invoke(R.Plugins.GetPlugin(method.ReflectedType.Assembly), new object[] { caller });
                        else if (methodParameters[0].ParameterType == typeof(string[]))
                            method.Invoke(R.Plugins.GetPlugin(method.ReflectedType.Assembly), new object[] { parameters });
                        break;
                    case 2:
                        if (methodParameters[0].ParameterType == typeof(IRocketPlayer) && methodParameters[1].ParameterType == typeof(string[]))
                            method.Invoke(R.Plugins.GetPlugin(method.ReflectedType.Assembly), new object[] { caller, parameters });
                        else if (methodParameters[0].ParameterType == typeof(string[]) && methodParameters[1].ParameterType == typeof(IRocketPlayer))
                            method.Invoke(R.Plugins.GetPlugin(method.ReflectedType.Assembly), new object[] { parameters, caller });
                        break;
                }
            }
        }

        public void UnregisterFromAssembly(Assembly assembly)
        {
            foreach (IRocketCommand command in R.Commands.Commands.Where(c => c.GetType().Assembly == assembly ||c is RocketAttributeCommand && ((RocketAttributeCommand)c).Method.ReflectedType.Assembly == assembly).ToList())
            {
                Deregister(command);
            }
        }
    }
}
