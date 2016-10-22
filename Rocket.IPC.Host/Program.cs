using Rocket.Core.IPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace Rocket.IPC.Host
{
    class Program
    {
        static void Main(string[] args)
        {

            string endpoint = "http://localhost:13378/";

            ServiceHost serviceHost;
            try
            {
                serviceHost = new ServiceHost(typeof(RocketService), new Uri(endpoint));
                serviceHost.Open();
            }
            catch (AddressAccessDeniedException)
            {
                try
                {
                    Process p = new Process();
                    p.StartInfo = new ProcessStartInfo("netsh", string.Format(@"http add urlacl url={0} user={1}\{2}", endpoint, Environment.UserDomainName, Environment.UserName))
                    {
                        Verb = "runas",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };

                    p.Start();

                    string output = p.StandardOutput.ReadToEnd();
                    Console.WriteLine(output);
                    p.WaitForExit();

                    serviceHost = new ServiceHost(typeof(RocketService), new Uri(endpoint));
                    serviceHost.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Restart as admin please");
                }
            }


            Console.ReadKey();
        }
    }
}
