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
            {"broadcast", "[{0:Name}] {1}"}
        };

        private readonly ILogger logger;

        public HelloWorldPlugin(IDependencyContainer container, 
                                IChatManager chatManager, 
                                ILogger logger) : base("HelloWorldPlugin",
            container)
        {
            ChatManager = chatManager;
            this.logger = logger;

            RegisterCommandsFromObject(this);
        }

        protected override void OnLoad(bool isReload)
        {
            logger.LogInformation("Hello world!");
        }

        protected override void OnUnload() { }

        [Command]
        public void KillPlayer(ICommandCaller sender, IOnlinePlayer target)
        {
            if(target is ILivingEntity)
                ((ILivingEntity)target).Kill(sender);
            else
                sender.SendMessage("Target could not be killed :(");
        }

        [Command]
        public void Broadcast(ICommandCaller sender, string[] args)
        {
            string message = string.Join(" ", args);
            ChatManager.BroadcastLocalized(Translations, message, sender, message);
        }
     }
}