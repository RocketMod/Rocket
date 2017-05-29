using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Rocket.API.Commands;
using Rocket.API.Providers.Logging;
using Rocket.API.Providers.Plugins;
using Rocket.Core;
using Rocket.Core.Commands;

namespace Rocket.Plugins.Native
{
    public sealed class NativeRocketPluginProvider : IRocketPluginProvider
    {
        public static readonly string PluginDirectory = "Plugins/{0}/";
        public static readonly string PluginTranslationFileTemplate = "{0}.{1}.translation.xml";
        public static readonly string PluginConfigurationFileTemplate = "{0}.configuration.xml";
        public static NativeRocketPluginProvider Instance { get; private set; }
        private static List<Assembly> pluginAssemblies;
        private static List<NativeRocketPlugin> plugins = new List<NativeRocketPlugin>();
        private Dictionary<string, string> libraries = new Dictionary<string, string>();

        public ReadOnlyCollection<IRocketPlugin> GetPlugins()
        {
            return plugins.Select(g => g.GetComponent<NativeRocketPlugin>()).Where(p => p != null).Select(p => (IRocketPlugin)p).ToList().AsReadOnly();
        }

        public IRocketPlugin GetPlugin(string name)
        {
            return plugins.Select(g => g.GetComponent<NativeRocketPlugin>()).FirstOrDefault(p => p != null && p.GetType().Assembly.GetName().Name == name);
        }

        public string GetPluginDirectory(string name)
        {
            return Path.Combine(PluginsDirectory, name) + "/";
        }

        public string PluginsDirectory { get; private set; }

        public ReadOnlyCollection<Type> Providers => new List<Type>().AsReadOnly();

        string librariesDirectory;
        public void Load(string pluginDirectory, string languageCode, string librariesDirectory)
        {
            PluginsDirectory = pluginDirectory;
            this.librariesDirectory = librariesDirectory;
            loadPlugins();
        }

        public List<IRocketCommand> GetCommandTypesFromAssembly(Assembly assembly, Type plugin)
        {
            List<IRocketCommand> commands = new List<IRocketCommand>();
            List<Type> commandTypes = GetTypesFromInterface(assembly, "IRocketCommand");
            foreach (Type commandType in commandTypes)
            {
                if (commandType.GetConstructor(Type.EmptyTypes) != null)
                {
                    IRocketCommand command = (IRocketCommand)Activator.CreateInstance(commandType);
                    commands.Add(command);
                }
            }

            if (plugin != null)
            {
                MethodInfo[] methodInfos = plugin.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (MethodInfo method in methodInfos)
                {
                    RocketCommandAttribute commandAttribute = (RocketCommandAttribute)Attribute.GetCustomAttribute(method, typeof(RocketCommandAttribute));
                    RocketCommandAliasAttribute[] commandAliasAttributes = (RocketCommandAliasAttribute[])Attribute.GetCustomAttributes(method, typeof(RocketCommandAliasAttribute));
                    RocketCommandPermissionAttribute[] commandPermissionAttributes = (RocketCommandPermissionAttribute[])Attribute.GetCustomAttributes(method, typeof(RocketCommandPermissionAttribute));

                    if (commandAttribute != null)
                    {
                        List<string> Permissions = new List<string>();
                        List<string> Aliases = new List<string>();

                        if (commandAliasAttributes != null)
                        {
                            foreach (RocketCommandAliasAttribute commandAliasAttribute in commandAliasAttributes)
                            {
                                Aliases.Add(commandAliasAttribute.Name);
                            }
                        }

                        if (commandPermissionAttributes != null)
                        {
                            foreach (RocketCommandPermissionAttribute commandPermissionAttribute in commandPermissionAttributes)
                            {
                                Aliases.Add(commandPermissionAttribute.Name);
                            }
                        }

                        IRocketCommand command = new RocketAttributeCommand(this, commandAttribute.Name, commandAttribute.Help, commandAttribute.Syntax, commandAttribute.AllowedCaller, Permissions, Aliases, method);
                        commands.Add(command);
                    }
                }
            }
            return commands;
        }

        private void loadPlugins()
        {
            libraries = GetAssembliesFromDirectory(librariesDirectory);
            pluginAssemblies = LoadAssembliesFromDirectory(PluginsDirectory);

            foreach (Assembly pluginAssembly in pluginAssemblies)
            {
                List<Type> pluginImplemenations = GetTypesFromInterface(pluginAssembly, "IRocketPlugin");

                foreach (Type pluginType in pluginImplemenations)
                {
                    //gameObject.TryAddComponent(pluginType);
                    CommandProvider.AddRange(GetCommandTypesFromAssembly(pluginAssembly, pluginType));
                }
            }
        }

        private void unloadPlugins()
        {
            for (int i = plugins.Count; i > 0; i--)
            {
                //Destroy(plugins[i - 1]);
            }
            plugins.Clear();
        }

        private static Dictionary<string, string> GetAssembliesFromDirectory(string directory, string searchPattern = "*.dll")
        {
            Dictionary<string, string> l = new Dictionary<string, string>();
            IEnumerable<FileInfo> libraries = new DirectoryInfo(directory).GetFiles(searchPattern, SearchOption.AllDirectories);
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

        public void AddCommands(IEnumerable<IRocketCommand> commands)
        {
            CommandProvider.AddRange(commands.AsEnumerable());
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

                    if (GetTypesFromInterface(assembly, "IRocketPlugin").Count == 1)
                    {
                        assemblies.Add(assembly);
                    }
                    else
                    {
                        R.Logger.Log(LogLevel.ERROR, "Invalid or outdated plugin assembly: " + assembly.GetName().Name);
                    }
                }
                catch (Exception ex)
                {
                    R.Logger.Log(LogLevel.ERROR, "Could not load plugin assembly: " + library.Name, ex);
                }
            }
            return assemblies;
        }

        public void Load(bool isReload = false)
        {
            try
            {
                Instance = this;
                CommandProvider = new RocketCommandList(this);
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
            catch (Exception ex)
            {
                R.Logger.Log(LogLevel.FATAL, ex);
            }
        }

        public static List<Type> GetTypesFromInterface(Assembly assembly, string interfaceName)
        {
            List<Type> allTypes = new List<Type>();
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            foreach (Type type in types.Where(t => t != null))
            {
                if (type.GetInterface(interfaceName) != null)
                {
                    allTypes.Add(type);
                }
            }
            return allTypes;
        }

        public RocketCommandList CommandProvider { get; set; }

        public void Unload(bool isReload)
        {
            unloadPlugins();
        }

        public ReadOnlyCollection<IRocketPlugin> Plugins => GetPlugins();
        public ReadOnlyCollection<Type> LoadProviders()
        {
            return Providers;
        }
    }
}