using System;
using System.Diagnostics;
using System.ServiceModel;
using Logger = Rocket.API.Logging.Logger;
namespace Rocket.Core.IPC
{
    public class RocketServiceHost
    {
        private ServiceHost serviceHost = null;
        private string endpoint;

        private void open()
        {
            serviceHost = new ServiceHost(typeof(RocketService), new Uri(endpoint));
            if(serviceHost.Description.Endpoints.Count == 0)
                serviceHost.AddServiceEndpoint(typeof(IRocketService), new BasicHttpBinding(), "");
            serviceHost.Open();
        }

        public RocketServiceHost(ushort port)
        {
            endpoint = String.Format("http://localhost:{0}/", port);
            try
            {
                open();
            }
            catch (AddressAccessDeniedException)
            {
                if (Environment.OperationSystem == Environment.OperationSystems.Windows)
                {
                    try
                    {
                        Process p = new Process();
                        p.StartInfo = new ProcessStartInfo("netsh", string.Format(@"http add urlacl url={0} user={1}\{2}", endpoint, System.Environment.UserDomainName, System.Environment.UserName))
                        {
                            Verb = "runas",
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        };

                        p.Start();

                        string output = p.StandardOutput.ReadToEnd();
                        Logger.Fatal(output);
                        p.WaitForExit();
                        open();
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal("Restart as admin please.", ex);
                    }
                }
            }


            Logger.Info("IPC hosting at " + endpoint);
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
