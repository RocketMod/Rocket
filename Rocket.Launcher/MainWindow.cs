using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rocket.API;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Rocket.API.Assets;
using Rocket.API.Serialisation;

namespace Rocket.Launcher
{
    public enum Mode { SingleInstance, MultiInstance };
    public partial class MainWindow : MetroFramework.Forms.MetroForm
    {
        private Service currentService = null;

        public Mode Mode = Mode.MultiInstance;
        public Process Game;
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
                    if (Game != null)
                    {
                        System.Threading.Thread.Sleep(2000);
                        try
                        {

                            Game.CloseMainWindow();
                            Game.Close();
                        }
                        catch (Exception)
                        {

                            //throw;
                        }
                    }
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void MainWindow_HandleCreated(object sender, EventArgs e)
        {
            try
            {
                /*if (File.Exists(Program.Executable))
                {
                    Mode = Mode.SingleInstance;
                    if (!Directory.Exists(Program.ServerPath)) Directory.CreateDirectory(Program.ServerPath);
                    settings = new XMLFileAsset<RocketSettings>(Path.Combine(Program.ServerPath, "Rocket.config.xml"));
                    if (!settings.Instance.RPC.Enabled) settings.Instance.RPC.Enabled = true;
                    settings.Save();


                    ProcessStartInfo info = new ProcessStartInfo(Program.Executable, Program.Arguments);
                    info.UseShellExecute = true;
#if DEBUG
                    info.CreateNoWindow = true;
                    info.WindowStyle = ProcessWindowStyle.Hidden;
#endif
                    Game = Process.Start(info);

                    AddService("localhost", settings.Instance.RPC.Port, settings.Instance.RPC.Username, settings.Instance.RPC.Password);
                    panel2.Hide();
                    Width -= panel2.Width;
                    panel1.Left-= panel2.Width;
                    panel1.Width += panel2.Width;
                }*/


                Text = "Rocket Launcher " + typeof(MainWindow).Assembly.GetName().Version;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddService(string host,ushort port,string username,string password)
        {
            Service s = new Service(host,port,username,password);
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
