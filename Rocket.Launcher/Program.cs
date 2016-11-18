using Rocket.API.Assets;
using Rocket.Core;
using Rocket.Launcher;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Rocket.Launcher
{
    class Program
    {
        public static string Instance;
        
        public static string Executable;
        public static string Arguments;
        public static string ServerPath;
        public static string RocketPath;
        
        static void Main(params string[] parameters)
        {
            Instance = (parameters.Length == 0) ? "server" : parameters[0];
            Executable = ConfigurationManager.AppSettings["Executable"];
            RocketPath = ConfigurationManager.AppSettings["RocketPath"];
            ServerPath = String.Format(ConfigurationManager.AppSettings["ServerPath"], Instance);
            Arguments = String.Format(ConfigurationManager.AppSettings["Arguments"], Instance);

            Dictionary<string, string> dependencies = new Dictionary<string, string>();
            IEnumerable<FileInfo> libraries = new DirectoryInfo(RocketPath).GetFiles("*.dll", SearchOption.AllDirectories);
            foreach (FileInfo library in libraries)
            {
                try
                {
                    AssemblyName name = AssemblyName.GetAssemblyName(library.FullName);
                    dependencies.Add(name.FullName, library.FullName);
                }
                catch { }
            }

            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                string file;
                if (dependencies.TryGetValue(args.Name, out file))
                {
                    return Assembly.Load(File.ReadAllBytes(file));
                }
                else
                {
                    MessageBox.Show("Could not find dependency: " + args.Name);
                }
                return null;
            };
            Initialize();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Initialize()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
