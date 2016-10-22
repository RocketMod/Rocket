using System;
using System.Diagnostics;
using System.ServiceModel;
using Logger = Rocket.API.Logging.Logger;
namespace Rocket.Core.IPC
{
    public class RocketServiceHost
    {
        private ServiceHost serviceHost = null;
        public RocketServiceHost(string game, string instance)
        {
            string endpoint = "http://localhost:13378/";

            ServiceHost serviceHost;
            try
            {
                serviceHost = new ServiceHost(typeof(RocketService), new Uri(endpoint));
                serviceHost.AddServiceEndpoint(typeof(IRocketService), new BasicHttpBinding(), "");
                serviceHost.Open();
            }
            catch (AddressAccessDeniedException)
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

                    serviceHost = new ServiceHost(typeof(RocketService), new Uri(endpoint));
                    serviceHost.Open();
                }
                catch (Exception ex)
                {
                    Logger.Fatal("Restart as admin please.",ex);
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
