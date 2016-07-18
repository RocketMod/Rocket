using Rocket.API;
using Rocket.API.Plugins;
using Rocket.Core.Extensions;
using Rocket.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Rocket.API.Commands;

namespace Rocket.Plugins.Native
{
    public sealed class NativeRocketPluginManager<T> : MonoBehaviour, IRocketPluginManager<T> where T : NativeRocketPlugin
    {
        public event PluginsLoaded OnPluginsLoaded;

        private static List<Assembly> pluginAssemblies;
        private static List<GameObject> plugins = new List<GameObject>();
        private Dictionary<string, string> libraries = new Dictionary<string, string>();

        private string pluginDirectory, librariesDirectory;

        public NativeRocketPluginManager(string pluginDirectory, string librariesDirectory)
        {
            this.pluginDirectory = pluginDirectory;
            this.librariesDirectory = librariesDirectory;
        }

        public List<T> GetPlugins()
        {
            return plugins.Select(g => g.GetComponent<T>()).Where(p => p != null).ToList<T>();
        }

        public T GetPlugin(string name)
        {
            return plugins.Select(g => g.GetComponent<NativeRocketPlugin>()).Where(p => p != null && p.GetType().Assembly.GetName().Name == name).FirstOrDefault();
        }

        public string GetPluginDirectory(string name)
        {
            return Path.Combine(pluginDirectory, name);
        }

        public List<IRocketCommand<T>> GetCommands(T plugin)
        {
            List<IRocketCommand<T>> commands = new List<IRocketCommand<T>>();
            foreach (Type commandType in plugin.Assembly.GetTypesFromInterface("IRocketCommand"))
            {
                if (commandType.GetConstructor(Type.EmptyTypes) != null)
                {
                    IRocketCommand<T> command = (IRocketCommand<T>)Activator.CreateInstance(commandType);
                    commands.Add(command);

                    foreach (string alias in command.Aliases)
                    {
                        commands.Add(command, alias);
                    }
                }
            }
            return commands;
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
            OnPluginsLoaded.TryInvoke();
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
                        Logger.GetLogger(typeof(NativeRocketPluginManager<T>)).Error("Invalid or outdated plugin assembly: " + assembly.GetName().Name);
                    }
                }
                catch (Exception ex)
                {
                    Logger.GetLogger(typeof(NativeRocketPluginManager<T>)).Error("Could not load plugin assembly: " + library.Name,ex);
                }
            }
            return assemblies;
        }


    }
}