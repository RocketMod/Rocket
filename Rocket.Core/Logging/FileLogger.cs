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
        private string file;

        public virtual string File
        {
            get => file;
            set
            {
                if (value != null && value.Equals(file, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (value == null)
                {
                    file = value;
                    return;
                }

                if (System.IO.File.Exists(value))
                {
                    var directory = Path.GetDirectoryName(value);
                    var fileName = Path.GetFileNameWithoutExtension(value);
                    var extension = Path.GetFileName(value).Replace(fileName, string.Empty);

                    string ver = $"{DateTime.Now:yyyyMMddTHHmmss}"; //ISO 8601
                    var backupFile = Path.Combine(directory, fileName + "." + ver + extension);

                    System.IO.File.Move(value, backupFile);
                }

                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }

                file = value;
                streamWriter = System.IO.File.AppendText(file);
                streamWriter.AutoFlush = true;
            }
        }


        public override void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null, ConsoleColor? color = null,
                        params object[] bindings)
        {
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