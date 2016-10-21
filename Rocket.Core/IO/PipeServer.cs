using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Rocket.Core.IO
{
    class PipeServer
    {
        public PipeServer()
        {
            ServiceHost serviceHost = new ServiceHost
               (typeof(Service), new Uri[] { new Uri("net.pipe://localhost/") });
            serviceHost.AddServiceEndpoint(typeof(IService), new NetNamedPipeBinding(), "helloservice");
            serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,MetadataExchangeBindings.CreateMexHttpBinding(),"mex");
            serviceHost.Open();

            Console.WriteLine("Service started. Available in following endpoints");
            foreach (var serviceEndpoint in serviceHost.Description.Endpoints)
            {
                Console.WriteLine(serviceEndpoint.ListenUri.AbsoluteUri);
            }
        }
    }

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void HelloWorld();
    }

    public class Service : IService
    {
        public void HelloWorld()
        {
            //Hello World
        }
    }
}
