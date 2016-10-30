using Rocket.API;
using Rocket.API.Logging;
using System;
using System.ServiceModel;
using Logger = Rocket.API.Logging.Logger;

namespace Rocket.Core.IPC
{

    public class RocketService : IRocketService
    {
        public void Connect(ushort port)
        {
            EndpointAddress address = new EndpointAddress(String.Format("http://localhost:{0}/", port));
            R.Implementation.OnPlayerConnected += Implementation_OnPlayerConnected;
            R.Implementation.OnPlayerDisconnected += Implementation_OnPlayerDisconnected;
            R.Implementation.OnShutdown += Implementation_OnShutdown;
            Logger.OnLog += Implemenation_OnLog;
        }

        private void Implemenation_OnLog(LogLevel level, object message, Exception exception)
        {
            //
        }

        private void Implementation_OnShutdown()
        {
            //
        }

        private void Implementation_OnPlayerDisconnected(IRocketPlayer player)
        {
            //
        }

        private void Implementation_OnPlayerConnected(IRocketPlayer player)
        {
            //
        }

        public void Disconnect(bool shutdown)
        {
            R.Implementation.OnPlayerConnected -= Implementation_OnPlayerConnected;
            R.Implementation.OnPlayerDisconnected -= Implementation_OnPlayerDisconnected;
            R.Implementation.OnShutdown -= Implementation_OnShutdown;
            Logger.OnLog -= Implemenation_OnLog;
            if (shutdown) R.Implementation.Shutdown();
        }

        public void Execute(string command)
        {
           R.Instance.Execute(new ConsolePlayer(), command);
        }

        public bool Test()
        {
            return true;
        }
    }

}
