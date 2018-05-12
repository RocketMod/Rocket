using System.Diagnostics;
using System.Drawing;
using System.IO;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Logging;

namespace Rocket
{
    public class Runtime : IRuntime
    {
        private Runtime()
        {
            Container = new UnityDependencyContainer();
            Container.RegisterInstance<IRuntime>(this);
            Container.RegisterSingletonType<ILogger, NullLogger>();

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(typeof(Runtime).Assembly.Location);
            Container.Activate(typeof(RegistrationByConvention));

            IImplementation impl = Container.Resolve<IImplementation>();

            string rocketInitializeMessage = "Initializing RocketMod " + versionInfo.FileVersion;
            impl.Console.WriteLine(rocketInitializeMessage, Color.DarkGreen);
            impl.Console.WriteLine(@"                                    
									
                                                           ,:
                                                         ,' |
                                                        /   :
                                                     --'   /
                                                     \/ />/
                                                     / <//_\        
                                                  __/   /           
                                                  )'-. /
                                                  ./  :\
                                                   /.' '
                                                 '/'
                                                 +
                                                '
                                              `.
                                          .-"" -
                                         (    |
                                      . .- '  '.
                                     ((.   )8:
                                 .'    / (_  )
                                  _. :(.   )8P  `
                              .  (  `-' (  `.   .
                               .  :  (   .a8a)
                              / _`(""a `a. )""'
                          ((/  .  ' )=='
                         ((    )  .8""   +
                           (`'8a.( _(   (
                        ..-. `8P) `  ) +
                      -'   (      -ab:  )
                    '    _  `    (8P""Ya
                  _((    )b -`.  ) +
                 (8)(_.aP"" _a   \( \   *
               +  ) / (8P(88))
                  (""     `""       `", Color.Cyan);


            if (!Container.IsRegistered<ILogger>("default_file_logger"))
            {
                string logsDirectory = Path.Combine(WorkingDirectory, "Logs");
                if (!Directory.Exists(logsDirectory))
                    Directory.CreateDirectory(logsDirectory);
                Container.RegisterSingletonType<ILogger, FileLogger>("default_file_logger");
                FileLogger fl = (FileLogger) Container.Resolve<ILogger>("default_file_logger");
                fl.File = Path.Combine(logsDirectory, "Rocket.log");
                fl.LogInformation(rocketInitializeMessage, Color.DarkGreen);
            }

            IPermissionProvider permissions = Container.Resolve<IPermissionProvider>();

            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);

            permissions.Load(this);

            Container.Resolve<ILogger>().LogInformation($"Initializing implementation: {impl.Name}", Color.Green);
            impl.Init(this);
        }

        public IDependencyResolver Resolver { get; private set; }

        public IDependencyContainer Container { get; }

        public bool IsAlive => true;
        public string Name => "Rocket.Runtime";

        public string WorkingDirectory
        {
            get
            {
                string implDir = Container.Resolve<IImplementation>().WorkingDirectory;
                string dirName = new DirectoryInfo(implDir).Name;
                if (dirName != "Rocket")
                    return Path.Combine(implDir, "Rocket");
                return implDir;
            }
        }

        public string ConfigurationName { get; } = "Rocket";

        public static IRuntime Bootstrap() => new Runtime();

        public void Shutdown()
        {
            Container.Dispose();
        }
    }
}