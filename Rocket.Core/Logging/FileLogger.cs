using System;
using System.IO;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    [DontAutoRegister]
    public class FileLogger : BaseLogger, IDisposable
    {
        private string logFile;
        private StreamWriter streamWriter;
        public FileLogger(IDependencyContainer container) : base(container) { }

        public virtual string File
        {
            get => logFile;
            set
            {
                if (value != null && value.Equals(logFile, StringComparison.OrdinalIgnoreCase)) return;

                if (value == null)
                {
                    logFile = null;
                    return;
                }

                if (System.IO.File.Exists(value)) Backup(value);

                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }

                logFile = value;
            }
        }

        public void Dispose()
        {
            streamWriter?.Close();
            streamWriter?.Dispose();

            Backup(File);
        }

        // renames old log files
        public void Backup(string file)
        {
            string directory = Path.GetDirectoryName(file);
            string fileName = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetFileName(file).Replace(fileName, string.Empty);

            string ver = $"{DateTime.Now:yyyyMMddTHHmmss}"; //ISO 8601
            string backupFileBaseName = Path.Combine(directory, fileName + "." + ver + extension);

            string backupFile = backupFileBaseName;
            int i = 0;
            while (true)
            {
                if (i > 20)
                    break;

                try
                {
                    System.IO.File.Move(file, backupFile);
                }
                catch
                {
                    backupFile = backupFileBaseName + "-" + i;
                    i++;
                }
            }
        }

        public override void OnLog(object message, LogLevel level = LogLevel.Information, Exception exception = null,
                                   params object[] bindings)
        {
            if (string.IsNullOrEmpty(logFile))
                throw new FileLoadException("File has not been set.");

            if (!IsEnabled(level))
                return;

            if (streamWriter == null)
                try
                {
                    string parentDir = Path.GetDirectoryName(logFile);
                    if (!Directory.Exists(parentDir))
                        Directory.CreateDirectory(parentDir);

                    streamWriter = System.IO.File.AppendText(logFile);
                    streamWriter.AutoFlush = true;
                }
                catch
                {
                    //todo: show error
                    return;
                }


            if (message != null)
            {
                WriteLine(level, message.ToString(), bindings);
            }

            if (exception != null)
            {
                WriteLine(level, exception.ToString());
            }
        }

        private void WriteLine(LogLevel level, string message, params object[] bindings)
        {
            string callingMethod = GetLoggerCallingMethod().GetDebugName();

            string formattedLine = $"[{DateTime.Now}] [{GetLogLevelPrefix(level)}] "
                + (LogSettings.IncludeMethods ? $"[{callingMethod}] " : "")
                + $"{string.Format(message, bindings)}";

            if (streamWriter.BaseStream.CanWrite)
                streamWriter.WriteLine(formattedLine);
        }

        public override string ServiceName => "FileLogger";
    }
}