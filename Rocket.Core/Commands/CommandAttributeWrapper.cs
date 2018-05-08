using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.Core.DependencyInjection;

namespace Rocket.Core.Commands
{
    [DontAutoRegister]
    public class CommandAttributeWrapper : ICommand
    {
        private readonly Type[] supportedCallers;

        public CommandAttributeWrapper(object instance, MethodBase method,
                                       CommandAttribute attribute,
                                       string[] aliases,
                                       Type[] supportedCallers)
        {
            this.supportedCallers = supportedCallers;
            Instance = instance;
            Method = method;
            Attribute = attribute;
            Name = attribute?.Name ?? method.Name;
            Syntax = attribute?.Syntax ?? BuildSyntaxFromMethod();

            Aliases = aliases;
        }

        private string BuildSyntaxFromMethod()
        {
            List<ParameterInfo> parameters = (from param in Method.GetParameters()
                                              let type = param.ParameterType
                                              where type != typeof(ICommandContext)
                                              where type != typeof(ICommandCaller)
                                              where type != typeof(string[])
                                              where type != typeof(ICommandParameters)
                                              where type != typeof(IDependencyContainer)
                                              select param).ToList();

            return string.Join(" ", parameters.Select(c => $"<{c.Name}>").ToArray());
        }

        public CommandAttribute Attribute { get; }
        public object Instance { get; }
        public MethodBase Method { get; set; }

        public string Name { get; }
        public string Summary => Attribute?.Summary;
        public string Description => Attribute?.Description;
        public string Permission => Attribute?.Permission;
        public string Syntax { get; }
        public ISubCommand[] ChildCommands { get; } //todo
        public string[] Aliases { get; }

        public bool SupportsCaller(Type commandCaller)
        {
            if (supportedCallers.Length == 0)
                return true;

            return supportedCallers.Any(c => c.IsAssignableFrom(commandCaller));
        }

        public void Execute(ICommandContext context)
        {
            List<object> @params = new List<object>();

            int index = 0;
            foreach (ParameterInfo param in Method.GetParameters())
            {
                Type type = param.ParameterType;
                if (type == typeof(ICommandContext))
                {
                    @params.Add(context);
                }

                else if (type == typeof(ICommandCaller))
                {
                    @params.Add(context);
                }

                else if (type == typeof(string[]))
                {
                    @params.Add(context.Parameters.ToArray());
                }

                else if (type == typeof(ICommandParameters))
                {
                    @params.Add(context.Parameters);
                }

                else if (type == typeof(IDependencyContainer))
                {
                    @params.Add(context.Container);
                }
                else
                {
                    try
                    {
                        @params.Add(context.Parameters.Get(index, type));
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new CommandWrongUsageException();
                    }
                    index++;
                }
            }

            Method.Invoke(Instance, @params.ToArray());
        }
    }
}