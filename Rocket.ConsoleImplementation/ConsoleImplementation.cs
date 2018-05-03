using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Plugins;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleImplementation : IImplementation
    {
        private readonly ConsoleCommandCaller consoleCommandCaller = new ConsoleCommandCaller();

        public IEnumerable<string> Capabilities => new List<string>();
        public string Name => "ConsoleHost";

        public string WorkingDirectory => Environment.CurrentDirectory;

        public void Init(IRuntime runtime)
        {
            Console.WriteLine("Loading...");
            runtime.Container.Resolve<IPluginManager>().Init();
            ICommandHandler cmdHandler = runtime.Container.Resolve<ICommandHandler>();

            string line;
            while (!(line = Console.ReadLine())?.Equals("exit", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                if(!cmdHandler.HandleCommand(ConsoleCommandCaller, line, ""))
                    Console.WriteLine("Failed to execute: " + line);
            }
        }

        public void Shutdown()
        {
            Environment.Exit(0);
        }

        public string InstanceId => "console";

        public void Reload() { }

        public IConsoleCommandCaller ConsoleCommandCaller => consoleCommandCaller;

        public bool IsAlive => true;

        public string ConfigurationName => "ConsoleHost";
    }
}