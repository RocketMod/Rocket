using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Event;
using Rocket.API.Event.Plugin;
using Rocket.API.Providers.Logging;
using Rocket.API.Providers.Plugins;
using Rocket.API.Providers.Translations;
using Rocket.API.Serialization;
using Rocket.Core.Assets;
using UnityEngine;

namespace Rocket.Core.Providers.Plugin.Native
{
    public class RocketPluginBase<T> : RocketPluginBase, IRocketPlugin<T> where T : class, IRocketPluginConfiguration
    {
        public IAsset<T> Configuration { get; private set; }
        public void Initialize(string workingDirectory)
        {
            base.Initialize(workingDirectory, false);

            string configurationFile = Path.Combine(WorkingDirectory, string.Format(NativeRocketPluginProvider.PluginConfigurationFileTemplate, Name));
            string url = null;
            if (File.Exists(configurationFile))
                url = File.ReadAllLines(configurationFile).First().Trim();

            Uri uri;
            if (url != null && Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                Configuration = new WebXMLFileAsset<T>(uri, null, (IAsset<T> config) => { LoadPlugin(); });
            }
            else
            {
                Configuration = new XMLFileAsset<T>(configurationFile);
                LoadPlugin();
            }
        }
        public override void LoadPlugin()
        {
            base.LoadPlugin();
            Configuration.Load();
        }
    }

    public class RocketPluginBase : MonoBehaviour, IRocketPlugin
    {
        public string WorkingDirectory { get; internal set; }

        public IRocketPluginProvider PluginManager { get; protected set; }
        public IAsset<TranslationList> Translations { get; private set; }
        public PluginState State { get; private set; } = PluginState.Unloaded;
        public string Name { get; protected set; }


        public bool IsDependencyLoaded(string plugin)
        {
            return PluginManager.GetPlugin(plugin) != null;
        }

        public delegate void ExecuteDependencyCodeDelegate(IRocketPlugin plugin);

        public void ExecuteDependencyCode(string plugin, ExecuteDependencyCodeDelegate a)
        {
            IRocketPlugin p = PluginManager.GetPlugin(plugin);
            if (p != null)
                a(p);
        }

        public Assembly Assembly => GetType().Assembly;

        public virtual TranslationList DefaultTranslations => new TranslationList();

        public virtual void Initialize(string workingDirectory, bool loadPlugin = true)
        {
            WorkingDirectory = workingDirectory;
            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);

            if (DefaultTranslations != null && DefaultTranslations.Count() != 0)
            {
                var language = R.Providers.GetProvider<IRocketTranslationDataProvider>().GetCurrentLanguage();
                Translations = new XMLFileAsset<TranslationList>(Path.Combine(WorkingDirectory, String.Format(NativeRocketPluginProvider.PluginTranslationFileTemplate, Name, language)), new Type[] { typeof(TranslationList), typeof(PropertyListEntry) }, DefaultTranslations);
                Translations.AddUnknownEntries(DefaultTranslations);
            }

            if (loadPlugin)
                LoadPlugin();
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
            R.Logger.Log(LogLevel.INFO, "\n[loading] " + name);
            Translations.Load();

            try
            {
                Load();
            }
            catch (Exception ex)
            {
                R.Logger.Log(LogLevel.FATAL, "Failed to load " + Name + ", unloading now...", ex);
                try
                {
                    UnloadPlugin(PluginState.Failure);
                    return;
                }
                catch (Exception ex1)
                {
                    R.Logger.Log(LogLevel.FATAL, "Failed to unload " + Name, ex1);
                }
            }



            PluginLoadingEvent loadingEvent = new PluginLoadingEvent(this);
            EventManager.Instance.CallEvent(loadingEvent);

            if (loadingEvent.IsCancelled)
            {
                try
                {
                    UnloadPlugin(PluginState.Cancelled);
                    return;
                }
                catch (Exception ex1)
                {
                    R.Logger.Log(LogLevel.FATAL, "Failed to unload " + Name, ex1);
                }
            }

            State = PluginState.Loaded;

            PluginLoadedEvent loadedEvent = new PluginLoadedEvent(this);
            EventManager.Instance.CallEvent(loadedEvent);
        }

        public virtual void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            R.Logger.Log(LogLevel.INFO, "\n[Unloading] " + Name);
            PluginUnloadingEvent unloadingEvent = new PluginUnloadingEvent(this);
            EventManager.Instance.CallEvent(unloadingEvent);
            Unload();
            State = state;

            PluginUnloadedEvent unloadedEvent = new PluginUnloadedEvent(this);
            EventManager.Instance.CallEvent(unloadedEvent);
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

        public void DestroyPlugin()
        {
            Destroy(this);
        }

        public bool Enabled { get; set; }
    }
}