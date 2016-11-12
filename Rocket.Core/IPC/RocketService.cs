using Rocket.API;
using Rocket.API.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using Logger = Rocket.API.Logging.Logger;
using System.Linq;

namespace Rocket.Core.IPC
{
    public class RocketService : IRocketService
    {
        public void Disconnect(bool shutdown)
        {
            if (shutdown) R.Implementation.Shutdown();
        }

        public void Execute(string command)
        {
           R.Execute(new ConsolePlayer(), command);
        }

        public bool Test()
        {
            return true;
        }

        public bool OnShutdown()
        {
            return RocketServiceHost.OnShutdown.Request();
        }
        public RocketPlayer OnPlayerConnected()
        {
            return RocketServiceHost.OnPlayerConnected.Request();
        }

        public RocketPlayer OnPlayerDisconnected()
        {
            return RocketServiceHost.OnPlayerDisconnected.Request();
        }

        public Queue<LogMessage> OnLog()
        {
            return RocketServiceHost.OnLog.RequestAll();
        }
    }

}
