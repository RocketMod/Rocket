using Rocket.API;
using Rocket.API.Plugins;
using Rocket.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Rocket.API.Commands;
using Rocket.API.Assets;
using Logger = Rocket.API.Logging.Logger;

namespace Rocket.Plugins.Native
{
    public sealed class NativeRocketPluginManager : MonoBehaviour, IRocketPluginManager
    {
        public static NativeRocketPluginManager Instance { get; private set; }
        private static List<Assembly> pluginAssemblies;
        private static List<GameObject> plugins = new List<GameObject>();
        private Dictionary<string, string> libraries = new Dictionary<string, string>();

        private string pluginDirectory, librariesDirectory;

        public RocketCommandList Commands { get; private set; }

        public NativeRocketPluginManager(string pluginDirectory, string librariesDirectory)
        {
            Instance = this;
            Commands = new RocketCommandList(this);
            this.pluginDirectory = pluginDirectory;
            this.librariesDirectory = librariesDirectory;
        }

        public List<IRocketPlugin> GetPlugins()
        {
            return plugins.Select(g => g.GetComponent<NativeRocketPlugin>()).Where(p => p != null).Select(p => (IRocketPlugin)p).ToList();
        }

        public IRocketPlugin GetPlugin(string name)
        {
            return plugins.Select(g => g.GetComponent<NativeRocketPlugin>()).Where(p => p != null && p.GetType().Assembly.GetName().Name == name).FirstOrDefault();
        }

        public string GetPluginDirectory(string name)
        {
            return Path.Combine(pluginDirectory, name);
        }

        private void Awake()
        {
            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                string file;
                if (libraries.TryGetValue(args.Name, out file))
                {
                    return Assembly.Load(File.ReadAllBytes(file));
                }
                return null;
            };
        }

        private void Start()
        {
            loadPlugins();
        }

        private Type GetMainTypeFromAssembly(Assembly assembly)
        {
            return assembly.GetTypesFromInterface("IRocketPlugin").FirstOrDefault();
        }

        private void loadPlugins()
        {
            libraries = GetAssembliesFromDirectory(librariesDirectory);
            pluginAssemblies = LoadAssembliesFromDirectory(pluginDirectory);
            List<Type> pluginImplemenations = pluginAssemblies.GetTypesFromInterface("IRocketPlugin");
            foreach (Type pluginType in pluginImplemenations)
            {
                GameObject plugin = new GameObject(pluginType.Name, pluginType);
                DontDestroyOnLoad(plugin);
                plugins.Add(plugin);
            }
        }

        private void unloadPlugins()
        {
            for (int i = plugins.Count; i > 0; i--)
            {
                Destroy(plugins[i - 1]);
            }
            plugins.Clear();
        }

        public void Reload()
        {
            unloadPlugins();
            loadPlugins();
        }

        private static Dictionary<string, string> GetAssembliesFromDirectory(string directory, string extension = "*.dll")
        {
            Dictionary<string, string> l = new Dictionary<string, string>();
            IEnumerable<FileInfo> libraries = new DirectoryInfo(directory).GetFiles(extension, SearchOption.AllDirectories);
            foreach (FileInfo library in libraries)
            {
                try
                {
                    AssemblyName name = AssemblyName.GetAssemblyName(library.FullName);
                    l.Add(name.FullName, library.FullName);
                }
                catch { }
            }
            return l;
        }

        public List<IRocketCommand> GetCommands(IRocketPlugin plugin)
        {
            List<IRocketCommand> commands = new List<IRocketCommand>();
            //foreach (Type commandType in plugin.Assembly.GetTypesFromInterface("IRocketCommand"))
            //{
            //    if (commandType.GetConstructor(Type.EmptyTypes) != null)
            //    {
            //        commands.Add((IRocketCommand)Activator.CreateInstance(commandType));

            //    }
            //}
            return commands;
        }

        private static List<Assembly> LoadAssembliesFromDirectory(string directory, string extension = "*.dll")
        {
            List<Assembly> assemblies = new List<Assembly>();
            IEnumerable<FileInfo> pluginsLibraries = new DirectoryInfo(directory).GetFiles(extension, SearchOption.TopDirectoryOnly);

            foreach (FileInfo library in pluginsLibraries)
            {
                try
                {
                    Assembly assembly = Assembly.Load(File.ReadAllBytes(library.FullName));

                    if (assembly.GetTypesFromInterface("IRocketPlugin").Count == 1)
                    {
                        assemblies.Add(assembly);
                    }
                    else
                    {
                        Logger.Error("Invalid or outdated plugin assembly: " + assembly.GetName().Name);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Could not load plugin assembly: " + library.Name, ex);
                }
            }
            return assemblies;
        }

        public void LoadPlugin(IRocketPlugin rocketPlugin)
        {
            throw new NotImplementedException();
        }

        public IAsset<IRocketPluginConfiguration> GetPluginConfiguration(IRocketPlugin plugin,Type configuration, string name = "")
        {
            throw new NotImplementedException();
        }

        public IAsset<IRocketPluginConfiguration> GetPluginTranslation(IRocketPlugin plugin)
        {
            throw new NotImplementedException();
        }
    }
}