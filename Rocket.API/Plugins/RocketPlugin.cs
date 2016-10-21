using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.Core.Extensions;
using Rocket.Logging;
using System;
using System.Linq;
using UnityEngine;

namespace Rocket.API.Plugins
{
    public class RocketPluginBase<T> : RocketPluginBase, IRocketPlugin<T> where T : class, IRocketPluginConfiguration
    {
        public IAsset<T> Configuration { get; private set; }

        protected RocketPluginBase(IRocketPluginManager manager, string name) : base(manager, name) 
        {
            Configuration = (IAsset<T>)manager.GetPluginConfiguration(this,typeof(T));
        }

        public override void LoadPlugin()
        {
            Configuration.Load();
            base.LoadPlugin();
        }
    }

    public class RocketPluginBase : MonoBehaviour, IRocketPlugin
    {
        public string WorkingDirectory { get; internal set; }



        public event RocketPluginUnloading OnPluginUnloading;
        public event RocketPluginUnloaded OnPluginUnloaded;

        public event RocketPluginLoading OnPluginLoading;
        public event RocketPluginLoaded OnPluginLoaded;

        public static event RocketPluginUnloading OnPluginsUnloading;
        public static event RocketPluginUnloaded OnPluginsUnloaded;

        public static event RocketPluginLoading OnPluginsLoading;
        public static event RocketPluginLoaded OnPluginsLoaded;

        public IRocketPluginManager PluginManager { get; private set; }
        public IAsset<TranslationList> Translations { get ; private set; }
        public PluginState State { get; private set; } = PluginState.Unloaded;
        public string Name { get; private set; }



        public virtual TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList();
            }
        }

        protected RocketPluginBase(IRocketPluginManager manager, string name)
        {
            this.PluginManager = manager;
            this.name = name;

            WorkingDirectory = manager.GetPluginDirectory(name);

            if (!System.IO.Directory.Exists(WorkingDirectory))
                System.IO.Directory.CreateDirectory(WorkingDirectory);
        }
        
        public string Translate(string translationKey, params object[] placeholder)
        {
            return Translations.Instance.Translate(translationKey, placeholder);
        }

        public void ReloadPlugin()
        {
            UnloadPlugin();
            LoadPlugin();
        }

        public virtual void LoadPlugin()
        {
            Logging.Logger.Info("\n[loading] " + name);
            Translations.Load();

            try
            {
                Load();
            }
            catch (Exception ex)
            {
                Logging.Logger.Fatal("Failed to load " + Name+ ", unloading now...", ex);
                try
                {
                    UnloadPlugin(PluginState.Failure);
                    return;
                }
                catch (Exception ex1)
                {
                    Logging.Logger.Fatal("Failed to unload " + Name ,ex1);
                }
            }

            bool doCancelLoading = false;
            bool cancelLoading = false;
            if (OnPluginLoading != null)
            {
                foreach (var handler in OnPluginLoading.GetInvocationList().Cast<RocketPluginLoading>())
                {
                    try
                    {
                        handler(this, ref cancelLoading);
                        if (cancelLoading) doCancelLoading = true;
                    }
                    catch (Exception ex)
                    {
                        Logging.Logger.Fatal(ex);
                    }
                }
            }

            if (OnPluginsLoading != null)
            {
                foreach (var handler in OnPluginsLoading.GetInvocationList().Cast<RocketPluginLoading>())
                {
                    try
                    {
                        handler(this, ref cancelLoading);
                        if (cancelLoading) doCancelLoading = true;
                    }
                    catch (Exception ex)
                    {
                        Logging.Logger.Fatal(ex);
                    }
                }
            }

            if (doCancelLoading)
            {
                try
                {
                    UnloadPlugin(PluginState.Cancelled);
                    return;
                }
                catch (Exception ex1)
                {
                    Logging.Logger.Fatal("Failed to unload " + Name, ex1);
                }
            }

            State = PluginState.Loaded;
            OnPluginLoaded.TryInvoke(this);
            OnPluginsLoaded.TryInvoke(this);
        }

        public virtual void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            Logging.Logger.Info("\n[unloading] " + Name);
            OnPluginUnloading.TryInvoke(this);
            OnPluginsUnloading.TryInvoke(this);
            Unload();
            State = state;
            OnPluginUnloaded.TryInvoke(this);
            OnPluginsUnloaded.TryInvoke(this);
        }

        private void OnEnable()
        {
            LoadPlugin();
        }

        private void OnDisable()
        {
            UnloadPlugin();
        }

        protected virtual void Load()
        {
        }

        protected virtual void Unload()
        {
        }
    }
}