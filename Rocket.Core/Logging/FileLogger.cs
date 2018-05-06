using System;
using System.Collections.Generic;
using System.IO;
using Rocket.API.Logging;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    [DontAutoRegister]
    public class FileLogger : BaseLogger, IDisposable
    {
        private StreamWriter streamWriter;
        private string logFile;

        public virtual string File
        {
            get => logFile;
            set
            {
                if (value != null && value.Equals(logFile, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (value == null)
                {
                    logFile = value;
                    return;
                }

                if (System.IO.File.Exists(value))
                {
                    var directory = Path.GetDirectoryName(value);
                    var fileName = Path.GetFileNameWithoutExtension(value);
                    var extension = Path.GetFileName(value).Replace(fileName, string.Empty);

                    string ver = $"{DateTime.Now:yyyyMMddTHHmmss}"; //ISO 8601
                    var backupFileBaseName = Path.Combine(directory, fileName + "." + ver + extension);

                    string backupFile = backupFileBaseName;
                    int i = 0;
                    while (System.IO.File.Exists(backupFile))
                    {
                        backupFile = backupFileBaseName + "-" + i;
                        i++;
                    }

                    System.IO.File.Move(value, backupFile);
                }

                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }

                logFile = value;
                streamWriter = System.IO.File.AppendText(logFile);
                streamWriter.AutoFlush = true;
            }
        }


        public override void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null, ConsoleColor? color = null,
                        params object[] bindings)
        {
            if(string.IsNullOrEmpty(logFile))
                throw new FileLoadException("File has not been set.");

            if (!IsEnabled(level))
                return;

            string callingMethod = GetLoggerCallingMethod().GetDebugName();
            string formattedLine = $"[{DateTime.Now}] [{GetLogLevelPrefix(level)}] [{callingMethod}] {message}";
            streamWriter.WriteLine(formattedLine);
        }

        public void Dispose()
        {
            streamWriter?.Close();
            streamWriter?.Dispose();
        }
    }
}