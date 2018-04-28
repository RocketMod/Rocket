using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.Commands
{
    public class CommandAttributeWrapper : ICommand
    {
        private readonly Type[] supportedCallers;
        public CommandAttribute Attribute { get; }

        public CommandAttributeWrapper(object instance, MethodBase method,
                                       CommandAttribute attribute,
                                       string[] aliases,
                                       Type[] supportedCallers)
        {
            this.supportedCallers = supportedCallers;
            Instance = instance;
            Method = method;
            Attribute = attribute;
            Aliases = aliases;
        }

        public string Name => Attribute.Name;
        public string Description => Attribute.Description;
        public string Permission => Attribute.Permission;
        public string Syntax => Attribute.Syntax;
        public ISubCommand[] ChildCommands { get; } //todo
        public string[] Aliases { get; }
        public object Instance { get; }
        public MethodBase Method { get; set; }

        public bool SupportsCaller(Type commandCaller)
        {
            if (supportedCallers.Length == 0)
                return true;

            return supportedCallers.Any(commandCaller.IsAssignableFrom);
        }

        public void Execute(ICommandContext context)
        {
            List<object> @params = new List<object>();

            int index = 0;
            foreach (var param in Method.GetParameters())
            {
                var type = param.ParameterType;
                if (type == typeof(ICommandContext))
                    @params.Add(context);

                else if (type == typeof(ICommandCaller))
                    @params.Add(context);

                else if (type == typeof(string[]))
                    @params.Add(context.Parameters.ToArray());

                else if (type == typeof(ICommandParameters))
                    @params.Add(context.Parameters);

                else if (type == typeof(IDependencyContainer))
                    @params.Add(context.Container);
                else
                {
                    @params.Add(context.Parameters.Get(index, type));
                    index++;
                }
            }

            Method.Invoke(Instance, @params.ToArray());
        }
    }
}