using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleImplementation : IImplementation
    {
        private ConsoleCaller consoleCaller;

        public IEnumerable<string> Capabilities => new List<string>();
        public string Name => "ConsoleHost";

        public string WorkingDirectory => Environment.CurrentDirectory;

        public void Init(IRuntime runtime)
        {
            Console.WriteLine("Loading...");
        }

        public void Shutdown()
        {
            Environment.Exit(0);
        }

        public string InstanceId => "console";

        public void Reload() { }

        public IConsoleCommandCaller GetConsoleCaller() => consoleCaller;

        public bool IsAlive => true;

        public string ConfigurationName => "ConsoleHost";
    }
}