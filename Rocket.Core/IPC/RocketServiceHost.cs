using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using Logger = Rocket.API.Logging.Logger;
namespace Rocket.Core.IPC
{
    public class RocketServiceHost 
    {
        private ServiceHost serviceHost = null;
        private Uri endpoint;


        public RocketServiceHost(ushort port)
        {
            endpoint = new Uri(String.Format("http://localhost:{0}/", port));
            try
            {
                serviceHost = new ServiceHost(typeof(RocketService), endpoint);
                serviceHost.AddServiceEndpoint(typeof(IRocketService), new BasicHttpBinding(), "");
#if DEBUG
                serviceHost.Description.Behaviors.Add(new ServiceMetadataBehavior());
                serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
#endif
                serviceHost.Open();
                Logger.Info("Starting IPC at " + endpoint);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to start IPC at " + endpoint, e);
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
