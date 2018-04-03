using System.Threading;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Scheduler;
using Rocket.Core.DependencyInjection;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleHost
    {
        private Thread _asyncThread;

        public IRuntime Runtime { get; private set; }

        public bool IsTestEnviroment { get; set; }

        public void Start()
        {
            Runtime = Rocket.Runtime.Bootstrap();

            if (!IsTestEnviroment)
            {
                _asyncThread = new Thread(AsyncLoop);
                _asyncThread.Start();

                SyncLoop();
            }
        }

        private void SyncLoop()
        {
            while (true)
            {

                Thread.Sleep(20);
            }
        }

        private void AsyncLoop()
        {
            while (true)
            {

                Thread.Sleep(20);
            }
        }
    }
}