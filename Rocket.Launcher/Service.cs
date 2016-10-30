using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using Rocket.Launcher.IPC;
using System.Threading;
using Rocket.API;
using System.ServiceModel.Description;
using Rocket.API.Logging;

namespace Rocket.Launcher
{
    public partial class Service : UserControl
    {
        public Dashboard Dashboard { get; private set; }

        public Service(ushort serverPort)
        {
            this.serverPort = serverPort;
            Dashboard = new Dashboard(this);
            InitializeComponent();
        }

        public RocketServiceClient R = null;
        private ushort serverPort;

        public void Connect()
        {
            cbInstance.Enabled = false;
            Dashboard.Enabled = false;
            
            Application.ApplicationExit += (object sender, EventArgs e) => { Disconnect(); };
            try
            {
                R = new RocketServiceClient(new BasicHttpBinding(), new EndpointAddress(String.Format("http://localhost:{0}/", serverPort)));
                R.Open();
                R.Test();
                cbInstance.Checked = true;
                cbInstance.Enabled = true;
                Dashboard.Enabled = true;
            }
            catch (Exception ex)
            {
                cbInstance.Enabled = true;
                Dashboard.Enabled = false;
                cbInstance.Checked = false;
                //MessageBox.Show(ex.ToString());
            }
        }

        public void Disconnect()
        {
            try
            {
                if (R != null && R.State == CommunicationState.Opened)
                {
                    R.Disconnect(false);
                    R.Close();
                }
            }
            catch (Exception)
            {
                //
            }
        }

        private void cbInstance_CheckedChanged(object sender, EventArgs e)
        {
            if (cbInstance.Checked)
            {
                Connect();
            }else
            {
                Disconnect();
            }
        }
    }
}
