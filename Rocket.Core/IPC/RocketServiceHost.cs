using System;
using System.Diagnostics;
using System.ServiceModel;
using Logger = Rocket.API.Logging.Logger;
namespace Rocket.Core.IPC
{
    public class RocketServiceHost
    {
        private ServiceHost serviceHost = null;
        private Uri endpoint;

        private void open()
        {
            serviceHost = new ServiceHost(typeof(RocketService), endpoint);
            if(serviceHost.Description.Endpoints.Count == 0)
            {
                serviceHost.AddServiceEndpoint(typeof(IRocketService), new NetTcpBinding(), "");
            }
            serviceHost.Open();
        }

        public RocketServiceHost(ushort port)
        {
            endpoint = new Uri(String.Format("net.tcp://localhost:{0}/", port));
            try
            {
                open();
                Logger.Info("Starting IPC at " + endpoint);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to start IPC at "+endpoint,e);
            }
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
