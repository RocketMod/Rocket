using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Entities;
using Rocket.API.Logging;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;

namespace Rocket.Examples.HelloWorldPlugin
{
    public class HelloWorldPlugin : Plugin
    {
        public IChatManager ChatManager { get; }

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

        public HelloWorldPlugin(IDependencyContainer container, 
                                IChatManager chatManager) : base("HelloWorldPlugin", container)
        {
            ChatManager = chatManager;

            RegisterCommandsFromObject(this);
        }

        protected override void OnLoad(bool isReload)
        {
            Logger.LogInformation("Hello world!");
        }

        protected override void OnUnload()
        {
            Logger.LogInformation("Good bye!");
        }
    }
}