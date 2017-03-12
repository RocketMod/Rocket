using System;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// The result of a script execution
    /// </summary>
    public class ScriptResult
    {
        private ScriptExecutionResult fILE_NOT_FOUND;

        public ScriptResult(ScriptExecutionResult fILE_NOT_FOUND)
        {
            this.fILE_NOT_FOUND = fILE_NOT_FOUND;
        }

        public ScriptResult()
        {
        }

        /// <summary>
        /// See <see cref="ScriptExecutionResult"/>.
        /// </summary>
        public ScriptExecutionResult ExecutionResult { get; set; }

        /// <summary>
        /// Return value of the script
        /// </summary>
        public object Return { get; set; }

        /// <summary>
        /// True if the script returned a value
        /// </summary>
        public bool HasReturn { get; set; }
        
        /// <summary>
        /// Exception thrown by the script
        /// </summary>
        public Exception Exception { get; set; }
    }
}