using System;
using System.IO;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Configuration;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Extensions;

namespace Rocket.Core.Logging
{
    [DontAutoRegister]
    public class FileLogger : BaseLogger, IDisposable
    {
        public FileLogger(IDependencyContainer container) : base(container) { }
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
                    logFile = null;
                    return;
                }

                if (System.IO.File.Exists(value))
                {
                    Backup(value);
                }

                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }

                logFile = value;
            }
        }

        // renames old log files
        public void Backup(string file)
        {
            var directory = Path.GetDirectoryName(file);
            var fileName = Path.GetFileNameWithoutExtension(file);
            var extension = Path.GetFileName(file).Replace(fileName, string.Empty);

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
                    System.IO.File.Move(file, backupFile);
                }
                catch
                {
                    backupFile = backupFileBaseName + "-" + i;
                    i++;
                }
            }
        }

        public override void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null, params object[] arguments)
        {
            if (string.IsNullOrEmpty(logFile))
                throw new FileLoadException("File has not been set.");


            if (!IsEnabled(level))
                return;

            if (streamWriter == null)
            {
                try
                {
                    var parentDir = Path.GetDirectoryName(logFile);
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
            }

            string callingMethod = GetLoggerCallingMethod().GetDebugName();
            string formattedLine = $"[{DateTime.Now}] [{GetLogLevelPrefix(level)}] " + (RocketSettings?.Settings.IncludeMethodsInLogs ?? true ? $"[{callingMethod}] " : "") + $"{message}";
            if (streamWriter.BaseStream.CanWrite)
                streamWriter.WriteLine(formattedLine);
        }

        public void Dispose()
        {
            streamWriter?.Close();
            streamWriter?.Dispose();

            Backup(File);
        }
    }
}