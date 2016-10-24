using Gtk;
using Rocket.Launcher.R;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Launcher
{
    class Program
    {
        static void Main()
        {
            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
			win.DeleteEvent+=(o, args) => Application.Quit();
            Application.Run();
        }
    }
}
