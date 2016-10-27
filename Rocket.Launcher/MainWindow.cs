using Rocket.Core.IPC;
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

namespace Rocket.Launcher
{
    public partial class MainWIndow : Form, IRocketServiceCallback
    {
        public MainWIndow()
        {
            InitializeComponent();
            
            EndpointAddress address = new EndpointAddress("net.tcp://localhost:13378/");
            R.RocketServiceClient R = new R.RocketServiceClient(new InstanceContext(this),new NetTcpBinding(), address);
     

        }

        public void NotifyPlayerConnected(IRocketPlayer player)
        {
            throw new NotImplementedException();
        }

        public void NotifyPlayerDisconnected(IRocketPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
