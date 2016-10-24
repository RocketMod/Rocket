using System;
using System.Diagnostics;
using System.ServiceModel;
using Logger = Rocket.API.Logging.Logger;
namespace Rocket.Core.IPC
{
    public class RocketServiceHost
    {
        private ServiceHost serviceHost = null;
        private Uri[] endpoints;

        private void open()
        {
            serviceHost = new ServiceHost(typeof(RocketService), endpoints);
            if(serviceHost.Description.Endpoints.Count == 0)
            {
                serviceHost.AddServiceEndpoint(typeof(IRocketService), new WSDualHttpBinding(), "");
            }
            serviceHost.Open();
        }

        public RocketServiceHost(ushort port)
        {
            endpoints = new Uri[] { new Uri(String.Format("http://localhost:{0}/", port)), new Uri(String.Format("net.tcp://localhost:{0}/", port)) };
            try
            {
                open();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                if (Environment.OperationSystem == Environment.OperationSystems.Windows)
                {
                    try
                    {
                        foreach(Uri endpoint in endpoints) {
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
                            Logger.Info("Starting IPC at " + endpoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal("Restart as admin please.", ex);
                    }
                }
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
