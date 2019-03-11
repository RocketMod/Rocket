using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.DependencyInjection;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;

namespace Rocket.Examples.HelloWorld
{
    public class HelloWorldPlugin : Plugin
    {
        public HelloWorldPlugin(IDependencyContainer container) : base("Hello World Plugin", container)
        {
        }

        public override object DefaultConfiguration => new
        {
            MyConfigField = "MyFieldValue",
            NestedSection = new
            {
                NestedConfigField = "MyNestedFieldValue"
            }
        };

        public override Dictionary<string, string> DefaultTranslations => new Dictionary<string, string>
        {
            {"some_translatable_message", "This is some translatable / replaceable text!"}
        };

        protected override async Task OnActivate(bool isFromReload)
        {
            Logger.LogInformation("Hello world!");
        }

        protected override async Task OnDeactivate()
        {
            Logger.LogInformation("Good bye!");
        }
    }
}