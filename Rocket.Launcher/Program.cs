using Rocket.API.Assets;
using Rocket.Core;
using Rocket.Launcher;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rocket.Launcher
{
    class Program
    {
        public static string Instance;
        static void Main(params string[] parameters)
        {
            Instance = (parameters.Length == 0) ? "server" : parameters[0];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
