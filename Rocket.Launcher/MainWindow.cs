using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rocket.API;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using Rocket.API.Assets;
using Rocket.Core;

namespace Rocket.Launcher
{
    public enum Mode { SingleInstance, MultiInstance };
    public partial class MainWindow : MetroFramework.Forms.MetroForm
    {
        private Service currentService = null;

        public Mode Mode = Mode.MultiInstance;
        public Process Game;
        private string executable;
        private string arguments;
        private string serverPath;
        XMLFileAsset<RocketSettings> settings;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                StyleManager = msmMain;
                HandleCreated += MainWindow_HandleCreated;
                Application.ApplicationExit += (object sender, EventArgs e) =>
                {
                    foreach (Service s in flpServiceContainer.Controls)
                    {
                        s.Disconnect();
                    }
                    System.Threading.Thread.Sleep(2000);
                    Game.CloseMainWindow();
                    Game.Close();
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void MainWindow_HandleCreated(object sender, EventArgs e)
        {
            try
            {
                executable = ConfigurationManager.AppSettings["Executable"];
                arguments = String.Format(ConfigurationManager.AppSettings["Arguments"], Program.Instance);
                serverPath = String.Format(ConfigurationManager.AppSettings["ServerPath"], Program.Instance);

                if (File.Exists(executable))
                {
                    Mode = Mode.SingleInstance;
                    if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);
                    settings = new XMLFileAsset<RocketSettings>(Path.Combine(serverPath, "Rocket.config.xml"));
                    if (!settings.Instance.RPC.Enabled) settings.Instance.RPC.Enabled = true;
                    settings.Save();


                    ProcessStartInfo info = new ProcessStartInfo(executable, arguments);
                    info.UseShellExecute = true;
                    //info.CreateNoWindow = true;
                    //info.WindowStyle = ProcessWindowStyle.Hidden;
                    Game = Process.Start(info);

                    AddService(settings.Instance.RPC.Port);
                    splitContainer1.Panel1.Hide();
                }


                Text = "Rocket Launcher " + typeof(MainWindow).Assembly.GetName().Version;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void AddService(ushort port)
        {
            Service s = new Service(port);
            s.Click += (object sender, EventArgs e) =>
            {
                currentService = (Service)sender;
                pDashboardContainer.Controls.Clear();
                pDashboardContainer.Controls.Add(currentService.Dashboard);
                currentService.Dashboard.Dock = DockStyle.Fill;
            };
            flpServiceContainer.Controls.Add(s);

            if (currentService == null)
            {
                currentService =s;
                pDashboardContainer.Controls.Clear();
                pDashboardContainer.Controls.Add(currentService.Dashboard);
                currentService.Dashboard.Dock = DockStyle.Fill;
            }
            s.Connect();
        }

        
    }
}
