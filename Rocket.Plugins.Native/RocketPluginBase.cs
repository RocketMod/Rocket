using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Event;
using Rocket.API.Event.Plugin;
using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.API.Providers;
using Rocket.API.Serialisation;
using UnityEngine;
using Environment = Rocket.API.Environment;
using Logger = Rocket.API.Logging.Logger;
using Object = UnityEngine.Object;

namespace Rocket.Plugins.Native
{
    public class RocketPluginBase<T> : RocketPluginBase, IRocketPlugin<T> where T : class, IRocketPluginConfiguration
    {
        public IAsset<T> Configuration { get; private set; }
        public void Initialize()
        {
            base.Initialize(false);

            string configurationFile = Path.Combine(WorkingDirectory, string.Format(API.Environment.PluginConfigurationFileTemplate, Name));
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

        public Assembly Assembly { get { return GetType().Assembly; } }

        public virtual TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList();
            }
        }

        public virtual void Initialize(bool loadPlugin = true)
        {
            WorkingDirectory = PluginManager.GetPluginDirectory(Name);
            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);

            if (DefaultTranslations != null | DefaultTranslations.Count() != 0)
            {
                Translations = new XMLFileAsset<TranslationList>(Path.Combine(WorkingDirectory, String.Format(Environment.PluginTranslationFileTemplate, Name, Environment.LanguageCode)), new Type[] { typeof(TranslationList), typeof(PropertyListEntry) }, DefaultTranslations);
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
            R.Logger.Info("\n[loading] " + name);
            Translations.Load();

            try
            {
                Load();
            }
            catch (Exception ex)
            {
                R.Logger.Fatal("Failed to load " + Name + ", unloading now...", ex);
                try
                {
                    UnloadPlugin(PluginState.Failure);
                    return;
                }
                catch (Exception ex1)
                {
                    R.Logger.Fatal("Failed to unload " + Name, ex1);
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
                    R.Logger.Fatal("Failed to unload " + Name, ex1);
                }
            }

            State = PluginState.Loaded;

            PluginLoadedEvent loadedEvent = new PluginLoadedEvent(this);
            EventManager.Instance.CallEvent(loadedEvent);
        }

        public virtual void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            R.Logger.Info("\n[Unloading] " + Name);
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
    }
}