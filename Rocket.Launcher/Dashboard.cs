using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rocket.API;
using Rocket.API.Logging;
using Rocket.API.Player;

namespace Rocket.Launcher
{
    public partial class Dashboard : UserControl
    {
        Service service;
        public Dashboard(Service service)
        {
            this.service = service;
            InitializeComponent();
            service.OnPlayerConnected += (RocketPlayerBase player) => { TellLog("New Player: " + player.DisplayName); };
            service.OnPlayerDisconnected += (RocketPlayerBase player) => { TellLog("Leaving Player: " + player.DisplayName); };
            service.OnShutdown += () => { TellLog("Shutdown"); };
            service.OnLog += (LogMessage message) => { TellLog(message.Message); };
        }

        public Dashboard()
        {
            InitializeComponent();
        }

        private void richTextBox1_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                richTextBox1.Text += textBox1.Text + System.Environment.NewLine;
                textBox1.Text = "";
            }
        }

        internal void TellLog(string message)
        {
            richTextBox1.Text += message + System.Environment.NewLine;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}
