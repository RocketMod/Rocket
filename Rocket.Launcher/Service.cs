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

        public void Connect()
        {
                pause();

                try
                {
                    BasicHttpBinding binding = new BasicHttpBinding();
                    binding.CloseTimeout = new TimeSpan(0, 10, 0);
                    binding.OpenTimeout = new TimeSpan(0, 10, 0);
                    binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                    binding.SendTimeout = new TimeSpan(0, 10, 0);

                    new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            R = new RocketServiceClient(binding, new EndpointAddress(String.Format("http://localhost:{0}/", serverPort)));
                            R.Open();
                            if (R.Test())
                            {
                                InvokePlease(() =>
                                {
                                    connected = true;
                                    startThreads();
                                    activate();
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Disconnect(ex);
                        }
                    })).Start();
                   
                }
                catch (Exception ex)
                {
                    Disconnect(ex);
                }
         
        }

        private Thread OnPlayerConnectedThread = null;
        public PlayerConnected OnPlayerConnected;

        private Thread OnPlayerDisconnectedThread = null;
        public PlayerDisconnected OnPlayerDisconnected;

        private Thread OnLogThread = null;
        public Logger.Log OnLog;

        private Thread OnImplementationShutdownThread = null;
        public ImplementationShutdown OnImplementationShutdown;


        private void prepareThreads()
        {
            OnPlayerConnectedThread = new Thread(new ThreadStart(() =>
            {
                while (connected)
                {
                    RocketPlayer player = null;
                    try
                    {
                        player = R.OnPlayerConnected();
                        if (player != null)
                            InvokePlease(() => { OnPlayerConnected?.Invoke(player); });
                    }
                    catch (TimeoutException)
                    {
                        //
                    }
                    catch (ThreadAbortException)
                    {
                        //
                    }
                    catch (Exception ce)
                    {
                        Disconnect(ce);
                    }
                }
            }));


            OnPlayerDisconnectedThread = new Thread(new ThreadStart(() =>
            {
                while (connected)
                {
                    RocketPlayer player = null;
                    try
                    {
                        player = R.OnPlayerDisconnected();
                        if (player != null)
                            InvokePlease(() => { OnPlayerDisconnected?.Invoke(player); });
                    }
                    catch (TimeoutException)
                    {
                        //
                    }
                    catch (ThreadAbortException)
                    {
                        //
                    }
                    catch (Exception ce)
                    {
                        Disconnect(ce);
                    }
                }
            }));

            OnLogThread = new Thread(new ThreadStart(() =>
            {
                while (connected)
                {
                    LogMessage message = null;
                    try
                    {
                        message = R.OnLog();
                        if (message != null)
                            InvokePlease(() => { OnLog?.Invoke(message); });
                    }
                    catch (TimeoutException)
                    {
                        //
                    }
                    catch (ThreadAbortException)
                    {
                        //
                    }
                    catch (Exception ce)
                    {
                        Disconnect(ce);
                    }
                }
            }));

            OnImplementationShutdownThread = new Thread(new ThreadStart(() =>
            {
                while (connected)
                {
                    bool shutdown = false;
                    try
                    {
                        shutdown = R.OnShutdown();
                        InvokePlease(() => { OnImplementationShutdown?.Invoke(); });
                    }
                    catch (TimeoutException)
                    {
                        //
                    }
                    catch (ThreadAbortException)
                    {
                        //
                    }
                    catch (Exception ce)
                    {
                        Disconnect(ce);
                    }
                }
            }));
        }

        public void startThreads()
        {
            prepareThreads();
            OnPlayerConnectedThread.Start();
            OnPlayerDisconnectedThread.Start();
            OnLogThread.Start();
            OnImplementationShutdownThread.Start();
        }

        public void stopThreads()
        {
            OnPlayerConnectedThread?.Abort();
            OnPlayerDisconnectedThread?.Abort();
            OnLogThread?.Abort();
            OnImplementationShutdownThread?.Abort();
        }

        private void activate()
        {
            Dashboard.Enabled = true;
            cbInstance.Enabled = true;

            ignoreCheckBoxChanged = true;
            cbInstance.Checked = true;
            ignoreCheckBoxChanged = false;
        }

        private void pause()
        {
            Dashboard.Enabled = false;
            cbInstance.Enabled = false;
            ignoreCheckBoxChanged = true;
            cbInstance.Checked = true;
            ignoreCheckBoxChanged = false;
        }

        private void deactviate()
        {
            Dashboard.Enabled = false;
            cbInstance.Enabled = true;

            ignoreCheckBoxChanged = true;
            cbInstance.Checked = false;
            ignoreCheckBoxChanged = false;
        }

        public void InvokePlease(MethodInvoker invoker)
        {
            if (IsHandleCreated && InvokeRequired)
                Invoke(invoker);
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
                stopThreads();
            });
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
