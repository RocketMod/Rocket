using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.Core.Extensions;
using Rocket.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Rocket.API.Plugins
{
    public class RocketPlugin<T,RocketPluginConfiguration> : RocketPlugin<T>, IRocketPlugin<RocketPluginConfiguration> where RocketPluginConfiguration : class, IRocketPluginConfiguration where T : IRocketPlugin
    {
        private IAsset<RocketPluginConfiguration> configuration;
        public IAsset<RocketPluginConfiguration> Configuration { get { return configuration; } }

        public delegate void LoadConfiguration(string pluginName,out IAsset<RocketPluginConfiguration> configuration);
        public event LoadConfiguration OnLoadConfiguration;

        internal RocketPlugin(IRocketPluginManager<T> manager, string name) : base(manager,name)
        {
            IAsset<RocketPluginConfiguration> config = null;
            OnLoadConfiguration?.Invoke(Name,out config);
            configuration = config;
        }

        public override void LoadPlugin()
        {
            configuration.Load();
            base.LoadPlugin();
        }
    }

    public class RocketPlugin<T> : MonoBehaviour, IRocketPlugin where T : IRocketPlugin
    {
        public IRocketPluginManager<T> pluginManager;
        public IRocketPluginManager<T> PluginManager { get { return pluginManager; } }

        private string directory;

        public delegate void PluginUnloading(IRocketPlugin plugin);

        public static event PluginUnloading OnPluginUnloading;

        public delegate void PluginLoading(IRocketPlugin plugin, ref bool cancelLoading);

        public static event PluginLoading OnPluginLoading;

        private XMLFileAsset<TranslationList> translations;
        public IAsset<TranslationList> Translations { get { return translations; } }

        private PluginState state = PluginState.Unloaded;

        public PluginState State
        {
            get
            {
                return state;
            }
        }

        private new string name;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public virtual TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList();
            }
        }

        internal RocketPlugin(IRocketPluginManager<T> manager, string name)
        {
            this.pluginManager = manager;
            this.name = name;

            directory = manager.GetPluginDirectory(name);

            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
        }

        public bool IsDependencyLoaded(string plugin)
        {
            return pluginManager.GetPlugin(plugin) != null;
        }

        public delegate void ExecuteDependencyCodeDelegate(IRocketPlugin plugin);

        public void ExecuteDependencyCode(string plugin, ExecuteDependencyCodeDelegate a)
        {
            IRocketPlugin p = pluginManager.GetPlugin(plugin);
            if (p != null)
                a(p);
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
            this.GetLogger().Info("\n[loading] " + name);
            translations.Load();

            R.Commands.AddRange(pluginManager.GetCommands(this));

            try
            {
                Load();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to load " + Name + ", unloading now... :" + ex.ToString());
                try
                {
                    UnloadPlugin(PluginState.Failure);
                    return;
                }
                catch (Exception ex1)
                {
                    Logger.LogError("Failed to unload " + Name + ":" + ex1.ToString());
                }
            }

            bool cancelLoading = false;
            if (OnPluginLoading != null)
            {
                foreach (var handler in OnPluginLoading.GetInvocationList().Cast<PluginLoading>())
                {
                    try
                    {
                        handler(this, ref cancelLoading);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                    if (cancelLoading)
                    {
                        try
                        {
                            UnloadPlugin(PluginState.Cancelled);
                            return;
                        }
                        catch (Exception ex1)
                        {
                            Logger.LogError("Failed to unload " + Name + ":" + ex1.ToString());
                        }
                    }
                }
            }
            state = PluginState.Loaded;
        }

        public virtual void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            Logger.Log("\n[unloading] " + Name, ConsoleColor.Cyan);
            OnPluginUnloading.TryInvoke(this);
            R.Commands.RemoveRange(pluginManager.GetCommands(this));
            Unload();
            this.state = state;
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

        public T TryAddComponent<T>() where T : Component
        {
            return gameObject.TryAddComponent<T>();
        }

        public void TryRemoveComponent<T>() where T : Component
        {
            gameObject.TryRemoveComponent<T>();
        }
    }
}