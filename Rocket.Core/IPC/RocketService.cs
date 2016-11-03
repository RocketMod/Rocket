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

        public RocketPlayer OnPlayerConnected()
        {
            lock (RocketServiceHost.OnPlayerConnectedQueue)
            {
                if (RocketServiceHost.OnPlayerConnectedQueue.Count != 0) return RocketServiceHost.OnPlayerConnectedQueue.Dequeue();
            }
            RocketServiceHost.OnPlayerConnectedReset.WaitOne();
            RocketServiceHost.OnPlayerConnectedReset.Reset();
            lock (RocketServiceHost.OnPlayerConnectedQueue)
            {
                if (RocketServiceHost.OnPlayerConnectedQueue.Count != 0) return RocketServiceHost.OnPlayerConnectedQueue.Dequeue();
            }

            return null;
        }
        public RocketPlayer OnPlayerDisconnected()
        {
            lock (RocketServiceHost.OnPlayerDisconnectedQueue)
            {
                if (RocketServiceHost.OnPlayerDisconnectedQueue.Count != 0) return RocketServiceHost.OnPlayerDisconnectedQueue.Dequeue();
            }
            RocketServiceHost.OnPlayerDisconnectedReset.WaitOne();
            RocketServiceHost.OnPlayerDisconnectedReset.Reset();
            lock (RocketServiceHost.OnPlayerDisconnectedQueue)
            {
                if (RocketServiceHost.OnPlayerDisconnectedQueue.Count != 0) return RocketServiceHost.OnPlayerDisconnectedQueue.Dequeue();
            }
            return null;
        }

        public LogMessage OnLog()
        {
            try
            {
                Console.WriteLine("HEY");
                lock (RocketServiceHost.OnLogQueue)
                {
                    if (RocketServiceHost.OnLogQueue.Count != 0) return RocketServiceHost.OnLogQueue.Dequeue();
                }
                RocketServiceHost.OnLogReset.WaitOne();
                RocketServiceHost.OnLogReset.Reset();
                lock (RocketServiceHost.OnLogQueue)
                {
                    if (RocketServiceHost.OnLogQueue.Count != 0) return RocketServiceHost.OnLogQueue.Dequeue();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public bool OnShutdown()
        {
            RocketServiceHost.OnShutdownReset.WaitOne();
            return true;
        }
    }

}
