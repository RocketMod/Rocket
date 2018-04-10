using System;
using System.Collections.Generic;
using Rocket.API;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleImplementation : IImplementation
    {
        public string Name => "ConsoleHost";

        public string WorkingDirectory => Environment.CurrentDirectory;

        public void Load(IRuntime runtime)
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

        public bool IsAlive => true;
    }
}