using System;
using System.Windows.Forms;
using System.ServiceModel;
using Rocket.Launcher.IPC;
using System.Threading;
using Rocket.API.Logging;
using Rocket.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rocket.Launcher
{
    public partial class Service : UserControl
    {
        public Dashboard Dashboard { get; private set; }
        private bool ignoreCheckBoxChanged = false;
        private bool connected = false;
        public Service(ushort serverPort)
        {
            this.serverPort = serverPort;
            Dashboard = new Dashboard(this);
            InitializeComponent();
            Application.ApplicationExit += (object sender, EventArgs e) => { Disconnect(); };
        }

        public RocketServiceClient R = null;
        private ushort serverPort;
        
        private int errorCount = 0;

        public void Connect(bool retry = true)
        {
            Task.Factory.StartNew(() =>
            {
                
                pause();
                try
                {
                    R = new RocketServiceClient(new BasicHttpBinding(), new EndpointAddress(String.Format("http://localhost:{0}/", serverPort)));

                    R.Open();

                    R.InnerChannel.OperationTimeout = new TimeSpan(0, 1, 0);
                    R.Endpoint.Binding.CloseTimeout = new TimeSpan(0, 1, 0);
                    R.Endpoint.Binding.OpenTimeout = new TimeSpan(0, 1, 0);
                    R.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 1, 0);
                    R.Endpoint.Binding.SendTimeout = new TimeSpan(0, 1, 0);

                    if (R.Test())
                    {
                        connected = true;
                        InvokePlease(()=>
                        {
                            getLog();
                            getPlayerConnected();
                            getPlayerDisconnected();
                            getShutdown();
                        });
                        activate();
                    }
                }
                catch (Exception ex)
                {
                    if (retry && errorCount < 10)
                    {
                        
                        Thread.Sleep(1000);
                        Connect();
                        return;
                    }
                    errorCount = 0;
                    Disconnect(ex);
                }
            });
        }

        public delegate void Log(LogMessage message);
        public Log OnLog;
        private async void getLog()
        {
            try
            {
                Queue<LogMessage> messages = await R.OnLogAsync();
                while (messages.Count > 0)
                {
                    OnLog?.Invoke(messages.Dequeue());
                }
            }
            catch (TimeoutException) { }
            catch (ThreadAbortException) {  }
            catch (Exception ce)
            {
                Disconnect(ce);
            }
            getLog();
        }


        public delegate void PlayerConnected(RocketPlayer message);
        public PlayerConnected OnPlayerConnected;
        private async void getPlayerConnected()
        {
            try
            {
                RocketPlayer player = await R.OnPlayerConnectedAsync();
                if (player != null)
                    OnPlayerConnected?.Invoke(player);
            }
            catch (TimeoutException) { }
            catch (ThreadAbortException) { }
            catch (Exception ce)
            {
                Disconnect(ce);
            }
            getPlayerConnected();
        }


        public delegate void PlayerDisconnected(RocketPlayer message);
        public PlayerDisconnected OnPlayerDisconnected;
        private async void getPlayerDisconnected()
        {
            try
            {
                RocketPlayer player = await R.OnPlayerDisconnectedAsync();
                if (player != null)
                    OnPlayerDisconnected?.Invoke(player);
            }
            catch (TimeoutException) { }
            catch (ThreadAbortException) { }
            catch (Exception ce)
            {
                Disconnect(ce);
            }
            getPlayerDisconnected();
        }


        public delegate void Shutdown();
        public Shutdown OnShutdown;
        private async void getShutdown()
        {
            try
            {
                bool shutdown = await R.OnShutdownAsync();
                if (shutdown)
                     OnShutdown?.Invoke(); 
            }
            catch (TimeoutException) { }
            catch (ThreadAbortException) { }
            catch (Exception ce)
            {
                Disconnect(ce);
            }
            getShutdown();
        }

        private void activate()
        {
            InvokePlease(() =>
            {
                Dashboard.Enabled = true;
                cbInstance.Enabled = true;
                ignoreCheckBoxChanged = true;
                cbInstance.Checked = true;
                ignoreCheckBoxChanged = false;
            });
        }

        private void pause()
        {
            InvokePlease(() =>
            {
                Dashboard.Enabled = false;
                cbInstance.Enabled = false;
                ignoreCheckBoxChanged = true;
                cbInstance.Checked = true;
                ignoreCheckBoxChanged = false;
            });
        }

        private void deactviate()
        {
            InvokePlease(() =>
            {
                Dashboard.Enabled = false;
                cbInstance.Enabled = true;
                ignoreCheckBoxChanged = true;
                cbInstance.Checked = false;
                ignoreCheckBoxChanged = false;
            });
        }

        public void InvokePlease(MethodInvoker invoker)
        {
            if (InvokeRequired)
                BeginInvoke(invoker);
            else
                invoker();
        }

        public void Disconnect(Exception ex = null)
        {
            InvokePlease(() => {
                if(ex != null && !connected)
                    MessageBox.Show(ex.ToString());
                connected = false;
                deactviate();
            });
            try
            {
                if (R != null && R.State == CommunicationState.Opened)
                {
                    R.Disconnect(true);
                    R.Close();
                }
                if(R.State == CommunicationState.Created)
                {
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
            if (ignoreCheckBoxChanged) return;
            if (cbInstance.Checked)
            {
                Connect();
            }
            else
            {
                Disconnect();
            }
        }
    }
}
