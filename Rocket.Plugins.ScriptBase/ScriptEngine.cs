using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// Implementation for a scripting language
    /// </summary>
    public abstract class ScriptEngine
    {
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

            return ExecuteFile(targetFile, ref context, true);
        }

        /// <summary>
        /// Executes the given script file
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="context">The script context. Can be null if there is none.</param>
        /// <param name="createPluginInstanceOnNull">Create a new plugin instance when <paramref name="context"/> is null?</param>
        /// <returns></returns>
        protected abstract ScriptResult ExecuteFile(string path, ref IScriptContext context, bool createPluginInstanceOnNull = false);


        /// <summary>
        /// Executes the given script file
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="context">The script context. Can be null if there is none.</param>
        /// <returns></returns>
        public virtual ScriptResult ExecuteFile(string path, ref IScriptContext context) =>
            ExecuteFile(path, ref context, false);
    }
}