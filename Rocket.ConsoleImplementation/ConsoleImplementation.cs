using Rocket.API;
using System;
using System.Collections.Generic;
using Rocket.API.Commands;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleImplementation : IImplementation
    {
        public string Name => "ConsoleHost";

        public string WorkingDirectory => Environment.CurrentDirectory;

        private ConsoleCaller consoleCaller;

        public void Init(IRuntime runtime)
        {
            Console.WriteLine("Loading...");
        }

        public IEnumerable<string> Capabilities => new List<string>();

        public void Shutdown()
        {
            Environment.Exit(0);
        }

        public string InstanceId => "console";

        public void Reload() { }
        public IConsoleCommandCaller GetConsoleCaller()
        {
            return consoleCaller;
        }

        public bool IsAlive => true;

        public string ConfigurationName => "ConsoleHost";
    }
}