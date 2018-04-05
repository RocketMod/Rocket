using System.Threading;
using Rocket.API;

namespace Rocket.ConsoleImplementation {
    public class ConsoleHost {
        private Thread asyncThread;

        public IRuntime Runtime { get; private set; }

        public bool IsTestEnviroment { get; set; }

        public void Start() {
            Runtime = Rocket.Runtime.Bootstrap();

            if (!IsTestEnviroment) {
                asyncThread = new Thread(AsyncLoop);
                asyncThread.Start();

                SyncLoop();
            }
        }

        private void SyncLoop() {
            while (true) Thread.Sleep(20);
        }

        private void AsyncLoop() {
            while (true) Thread.Sleep(20);
        }
    }
}