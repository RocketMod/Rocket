using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.Core.Assets;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Rocket.Core.Plugins
{
    public class RocketPlugin<TConfiguration> : RocketPlugin, IRocketPlugin<TConfiguration> where TConfiguration : class, IRocketPluginConfiguration
    {
        private IAsset<TConfiguration> configuration;
        public IAsset<TConfiguration> Configuration { get { return configuration; } }

        public RocketPlugin() : base()
        {
            if (Core.R.Settings.Instance.WebConfigurations.Enabled)
            {
                string url = string.Format(Environment.WebConfigurationTemplate, Core.R.Settings.Instance.WebConfigurations.Url, Name, R.Implementation.InstanceId);
                configuration = new WebXMLFileAsset<TConfiguration>(url, null, (IAsset<TConfiguration> asset) => { base.LoadPlugin(); });
            }
            else
            {
                configuration = new XMLFileAsset<TConfiguration>(Directory + string.Format(Core.Environment.PluginConfigurationFileTemplate, Name));
            }
        }
    }

    public class RocketPlugin : MonoBehaviour, IRocketPlugin
    {
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

        private Assembly assembly;
        public Assembly Assembly { get { return assembly; } }

        private string directory = null;
        public string Directory
        {
            get { return directory; }
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

        public RocketPlugin()
        {
            assembly = GetType().Assembly;
            name = Assembly.GetName().Name;
            directory = String.Format(Core.Environment.PluginDirectory, Name);
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            if(DefaultTranslations != null | DefaultTranslations.Count() != 0)
                translations = new XMLFileAsset<TranslationList>(directory + String.Format(Environment.PluginTranslationFileTemplate, Name, R.Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(TranslationListEntry) }, DefaultTranslations);
        }

        public static bool IsDependencyLoaded(string plugin)
        {
            return Rocket.Core.R.Plugins.GetPlugin(plugin) != null;
        }

        public delegate void ExecuteDependencyCodeDelegate(IRocketPlugin plugin);
        public static void ExecuteDependencyCode(string plugin,ExecuteDependencyCodeDelegate a)
        {
            IRocketPlugin p = Rocket.Core.R.Plugins.GetPlugin(plugin);
            if (p != null) 
                a(p);
        }

        public string Translate(string translationKey, params object[] placeholder)
        {
            return Translations.Instance.Translate(translationKey,placeholder);
        }

        public void ReloadPlugin()
        {
            UnloadPlugin();
            LoadPlugin();
        }

        public void LoadPlugin()
        {
            Logger.Log("\n[loading] " + name, ConsoleColor.Cyan);
            R.Commands.RegisterFromAssembly(Assembly);

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
                    if (cancelLoading) {
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

        public void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            Logger.Log("\n[unloading] " + Name, ConsoleColor.Cyan);
            OnPluginUnloading.TryInvoke(this);
            R.Commands.DeregisterFromAssembly(Assembly);
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