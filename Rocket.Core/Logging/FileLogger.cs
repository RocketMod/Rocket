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
                    while (true)
                    {
                        if (i > 20)
                            break;

                        try
                        {
                            System.IO.File.Move(value, backupFile);
                        }
                        catch
                        {
                            backupFile = backupFileBaseName + "-" + i;
                            i++;
                        }
                    }
                }

                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }

                logFile = value;

                try
                {
                    streamWriter = System.IO.File.AppendText(logFile);
                    streamWriter.AutoFlush = true;
                }
                catch 
                {
                    //todo 
                }
            }
        }


        public override void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null, ConsoleColor? color = null,
                        params object[] bindings)
        {
            if (string.IsNullOrEmpty(logFile))
                throw new FileLoadException("File has not been set.");

            if (!IsEnabled(level) || streamWriter == null)
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