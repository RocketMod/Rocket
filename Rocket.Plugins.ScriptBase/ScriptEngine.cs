using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rocket.Core;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// Implementation for a scripting language
    /// </summary>
    public abstract class ScriptEngine
    {
        /// <summary>
        /// List of all scripting contexts
        /// </summary>
        public List<IScriptContext> Contexts { get; protected set; } = new List<IScriptContext>();

        /// <summary>
        /// Full name of the scripting language (e.g. "JavaScript")
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// File types associated with the scripting language (e.g. ".js", ".javascript")
        /// </summary>
        public abstract List<string> FileTypes { get; }

        /// <summary>
        /// Short name of the scripting language (e.g. "js")
        /// </summary>
        public abstract string ShortName { get; }

        /// <summary>
        /// Base path to the scripts directory
        /// </summary>
        public string BaseDir => Path.Combine("Scripts", Name);

        /// <summary>
        /// Path to the script plugins directory
        /// </summary>
        public string PluginsDir => Path.Combine(BaseDir, "Plugins");

        /// <summary>
        /// Path to the shared libraries of the scripting engine
        /// </summary>
        public string LibrariesDir => Path.Combine(BaseDir, "Libraries");

        /// <summary>
        /// The associated plugin manager
        /// </summary>
        public ScriptRocketPluginProvider PluginProvider { get; private set; }

        public void Load()
        {
            PluginProvider = new ScriptRocketPluginProvider(this);

            if (!Directory.Exists(PluginsDir))
                Directory.CreateDirectory(PluginsDir);

            if (!Directory.Exists(LibrariesDir))
                Directory.CreateDirectory(LibrariesDir);

            OnLoad();
        }

        protected virtual void OnLoad()
        {
            
        }

        /// <summary>
        /// <p>Searches for the "Plugin.[filetype]" (e.g. Plugin.js) and runs it</p>
        /// <p>Will generate a new plugin instance if context is null</p>
        /// </summary>
        /// <param name="path">The path to the directory containing the script files</param>
        /// <param name="context">The script context. Can be null if there is none.</param>
        public virtual ScriptResult LoadPluginFromDirectory(string path, ref IScriptContext context)
        {
            var files = Directory.GetFiles(path);
            if (files.Length == 0)
            {
                return new ScriptResult(ScriptExecutionResult.FILE_NOT_FOUND);
            }

            ScriptPluginMeta meta = GetPluginMeta(path);
            string targetFile = null;
            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if(fileName == null)
                    continue;

                string ext = Path.GetExtension(file);
                if(ext == null)
                    continue;

                if (fileName.Equals("plugin", StringComparison.OrdinalIgnoreCase) &&
                    FileTypes.Any(c => c.Equals(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    targetFile = file;
                }
            }

            if(targetFile == null)
                return new ScriptResult(ScriptExecutionResult.FILE_NOT_FOUND);

            return ExecuteFile(targetFile, meta.EntryPoint, ref context, meta, true);
        }

        /// <summary>
        /// Get information about the plugin in the given path.
        /// </summary>
        /// <param name="path">The path of the plugin.</param>
        /// <returns></returns>
        public virtual ScriptPluginMeta GetPluginMeta(string path)
        {
            //todo get this from a meta file (e.g. PluginInfo.xml)
            return new ScriptPluginMeta
            {
                Author = string.Empty,
                Name = new DirectoryInfo(path).Name,
                EntryPoint = "Load"
            };
        }

        /// <summary>
        /// Registers the context to the scripting engine.
        /// </summary>
        /// <param name="context">The script context.</param>
        public void RegisterContext(IScriptContext context)
        {
            Contexts.Add(context);
            var engine = context.ScriptEngine;
            engine.RegisterType("R", typeof(R), context);
            //engine.RegisterType("Logger", typeof(Logger), context);
            //engine.RegisterType("UnityLogger", typeof(UnityEngine.Logger), context);
            //todo auto register unturneds & rockets stuff?

            //todo bad code:
            var uType = Type.GetType("Rocket.Unturned.U");
            if (uType != null)
            {
                context.ScriptEngine.RegisterType("U", uType, context);
            }
        }

        /// <summary>
        /// Executes the given script file
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="entryPoint">The entry function to call.</param>
        /// <param name="context">The script context. Can be null if there is none.</param>
        /// <param name="createPluginInstanceOnNull">Create a new plugin instance when <paramref name="context"/> is null?</param>
        /// <param name="meta">Metadata for the plugin if <paramref name="createPluginInstanceOnNull"/> is true and <paramref name="context"/> is null. Can be null if <paramref name="createPluginInstanceOnNull"/> is false.</param>
        /// <returns>The result of the script execution.</returns>
        protected abstract ScriptResult ExecuteFile(string path, string entryPoint, ref IScriptContext context, ScriptPluginMeta meta, bool createPluginInstanceOnNull = false);

        public ScriptInitHelper GetScriptIniter(IScriptContext ctx)
        {
            return new ScriptInitHelper(this, ctx);
        }

        /// <summary>
        /// Executes the given script file
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="entryPoint">The entry function to call.</param>
        /// <param name="context">The script context. Can be null if there is none.</param>
        /// <returns>The result of the script execution.</returns>
        public virtual ScriptResult ExecuteFile(string path, string entryPoint, ref IScriptContext context) =>
            ExecuteFile(path, entryPoint, ref context, null, false);

        public abstract void RegisterType(string name, Type type, IScriptContext context);
    }
}