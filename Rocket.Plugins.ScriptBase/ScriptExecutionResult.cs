namespace Rocket.Plugins.ScriptBase
{
    public enum ScriptExecutionResult
    {
        /// <summary>
        /// The script has been executed with no errors.
        /// </summary>
        SUCCESS,
        /// <summary>
        /// The script has thrown exceptions on load. The <see cref="ScriptEngine"/>.Exception property should not be null.
        /// </summary>
        FAILED_EXCEPTION,
        /// <summary>
        /// The script failed to execute, but no exceptions occured.
        /// </summary>
        FAILED_MISC,
        /// <summary>
        /// The script has failed to load (e.g. missing file permissions).
        /// </summary>
        LOAD_FAILED,
        /// <summary>
        /// The script file was not found.
        /// </summary>
        FILE_NOT_FOUND
    }
}