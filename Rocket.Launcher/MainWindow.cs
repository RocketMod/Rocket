using System;
namespace Rocket.Launcher
{
	public partial class MainWindow : Gtk.Window
	{
		public MainWindow() :
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();
		}
	}
}
