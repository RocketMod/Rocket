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

        private static IDependencyContainer _container;
        public IRuntime Runtime { get; private set; }

        public bool IsTestEnviroment { get; set; }

        public void Start()
        {
            _container = new UnityDependencyContainer();
            _container.RegisterSingletonType<IImplementation, ConsoleImplementation>();
            Runtime = Rocket.Runtime.Bootstrap(_container);

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