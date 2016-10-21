using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rocket.Launcher
{
    public partial class MainWindow : Window
    {
        private string Game = "Unturned";
        private string Arguments = "+secureserver/test";

        public MainWindow()
        {
            InitializeComponent();
            Process process = new Process();
            process.StartInfo.FileName = Game+".exe";
            process.StartInfo.Arguments = @"-nographics -batchmode -silent-crashes "+ Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = false;
         
        }
    }
}
