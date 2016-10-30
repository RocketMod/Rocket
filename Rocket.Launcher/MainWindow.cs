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

namespace Rocket.Launcher
{
    public partial class MainWindow : MetroFramework.Forms.MetroForm
    {
        private Service currentService = null;

        public MainWindow()
        {
            InitializeComponent();
            this.StyleManager = msmMain;
            this.Text = "Rocket Launcher " + typeof(MainWindow).Assembly.GetName().Version;

            AddService(27115);
        }

        public void AddService(ushort port)
        {
            Service s = new Service(port);
            s.Click += (object sender, EventArgs e) =>
            {
                currentService = (Service)sender;
                pDashboardContainer.Controls.Clear();
                pDashboardContainer.Controls.Add(currentService.Dashboard);
            };
            flpServiceContainer.Controls.Add(s);

            if (currentService == null)
            {
                currentService =s;
                pDashboardContainer.Controls.Clear();
                pDashboardContainer.Controls.Add(currentService.Dashboard);
            }
            s.Connect();
        }

        
    }
}
