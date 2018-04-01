using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Scheduler;

namespace Rocket.ConsoleImplementation
{
    class ConsoleImplementation : IImplementation
    {
        public void Load(IRuntime runtime)
        {
            Console.WriteLine("Loading...");

            /* Register implementations here which have dependencies on stuff registered in Runtime */
            runtime.Container.RegisterSingletonType<ITaskScheduler, SimpleTaskScheduler>();
        }

        public IEnumerable<string> Capabilities => new List<string>();

        public void Shutdown()
        {
            Environment.Exit(0);
        }

        public string InstanceId => "console";
        public void Reload()
        {

        }

        public string Name => "ConsoleHost";
        public bool IsAlive => true;
    }
}