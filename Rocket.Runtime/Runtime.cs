using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.Core.Configuration;
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
            Container.RegisterSingletonType<ILogger, ConsoleLogger>("console_logger");
            Container.RegisterSingletonType<ILogger, ProxyLogger>("proxy_logger", null);
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(typeof(Runtime).Assembly.Location);

            var logger = Container.Resolve<ILogger>();
            string rocketInitializeMessage = "Initializing RocketMod " + versionInfo.FileVersion;
            logger.LogInformation(rocketInitializeMessage, ConsoleColor.DarkGreen);
            logger.LogInformation(@"                                    
									
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
                  (""     `""       `", ConsoleColor.Cyan);


            Container.Activate(typeof(RegistrationByConvention));
            
            if (!Container.IsRegistered<ILogger>("default_file_logger"))
            {
                var logsDirectory = Path.Combine(WorkingDirectory, "Logs");
                if (!Directory.Exists(logsDirectory))
                    Directory.CreateDirectory(logsDirectory);
                Container.RegisterSingletonType<ILogger, FileLogger>("default_file_logger");
                FileLogger fl = (FileLogger) Container.Resolve<ILogger>("default_file_logger");
                fl.File = Path.Combine(logsDirectory, "Rocket.log");
                fl.LogInformation(rocketInitializeMessage, ConsoleColor.DarkGreen);
            }

            var permissions = Container.Resolve<IPermissionProvider>();
            var impl = Container.Resolve<IImplementation>();

            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);

            permissions.Load(this);

            logger.LogInformation($"Initializing implementation: {impl.Name}", ConsoleColor.Green);
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
                var implDir = Container.Resolve<IImplementation>().WorkingDirectory;
                string dirName = new DirectoryInfo(implDir).Name;
                if (dirName != "Rocket")
                    return Path.Combine(implDir, "Rocket");
                return implDir;
            }
        }

        public string ConfigurationName { get; } = "Rocket";

        public static IRuntime Bootstrap() => new Runtime();
    }
}