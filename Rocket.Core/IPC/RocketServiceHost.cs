using Rocket.API;
using Rocket.API.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using Logger = Rocket.API.Logging.Logger;
namespace Rocket.Core.IPC
{
    public class RocketServiceHost 
    {
        private ServiceHost serviceHost = null;
        private Uri endpoint;

        internal static ManualResetEvent OnPlayerConnectedReset = new ManualResetEvent(false);
        internal static ManualResetEvent OnPlayerDisconnectedReset = new ManualResetEvent(false);
        internal static ManualResetEvent OnLogReset = new ManualResetEvent(false);
        internal static ManualResetEvent OnShutdownReset = new ManualResetEvent(false);

        internal static Queue<RocketPlayer> OnPlayerConnectedQueue = new Queue<RocketPlayer>();
        internal static Queue<RocketPlayer> OnPlayerDisconnectedQueue = new Queue<RocketPlayer>();
        internal static Queue<LogMessage> OnLogQueue = new Queue<LogMessage>();


        public RocketServiceHost(ushort port)
        {
            endpoint = new Uri(String.Format("http://localhost:{0}/", port));
            try
            {
                serviceHost = new ServiceHost(typeof(RocketService), endpoint);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.CloseTimeout = new TimeSpan(0, 10, 0);
                binding.OpenTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                binding.SendTimeout = new TimeSpan(0, 10, 0);

                serviceHost.AddServiceEndpoint(typeof(IRocketService), binding, "");
#if DEBUG
                serviceHost.Description.Behaviors.Add(new ServiceMetadataBehavior());
                serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
#endif
                serviceHost.Open();

                if (R.Implementation != null)
                {
                    R.Implementation.OnPlayerConnected += Implementation_OnPlayerConnected;
                    R.Implementation.OnPlayerDisconnected += Implementation_OnPlayerDisconnected;
                    R.Implementation.OnShutdown += Implementation_OnShutdown;
                }
                Logger.OnLog += Implemenation_OnLog;

                Logger.Info("Starting IPC at " + endpoint);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to start IPC at " + endpoint, e);
            }
        }


        private void Implementation_OnPlayerConnected(IRocketPlayer player)
        {
            lock (OnPlayerConnectedQueue)
            {
                OnPlayerConnectedQueue.Enqueue((RocketPlayer)player);
            }
            OnPlayerConnectedReset.Set();
        }

        private void Implementation_OnPlayerDisconnected(IRocketPlayer player)
        {
            lock (OnPlayerDisconnectedQueue)
            {
                OnPlayerDisconnectedQueue.Enqueue((RocketPlayer)player);
            }
            OnPlayerDisconnectedReset.Set();
        }

        private void Implemenation_OnLog(LogMessage message)
        {
            try
            {
                lock (OnLogQueue)
                {
                    OnLogQueue.Enqueue(message);
                }
                OnLogReset.Set();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void Implementation_OnShutdown()
        {
            OnShutdownReset.Set();
        }

        public void Stop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }

    }
}
