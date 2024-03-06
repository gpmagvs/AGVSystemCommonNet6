using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Log
{
    public class LogBase
    {
        public string LogFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), LogFolderName);
        private string _LogFolderName = "GPM_AGV_LOG";
        internal string LogFolderName
        {
            get => _LogFolderName;
            set
            {
                _LogFolderName = value;
                if (!Directory.Exists(LogFolder))
                    Directory.CreateDirectory(LogFolder);
            }
        }
        private ConcurrentQueue<LogItem> logItemQueue = new ConcurrentQueue<LogItem>();
        private Task WriteLogToFileTask;

        public LogBase() { }
        public LogBase(string LogFolderName)
        {
            this.LogFolderName = LogFolderName;
        }

        public async Task LogAsync(LogItem logItem, string caller_class_name = "")
        {
            if (WriteLogToFileTask == null)
            {
                WriteLogToFileTask = Task.Run(() => WriteLogWorker());
            }
            logItem.Caller = caller_class_name;
            logItem.Time = DateTime.Now;
            logItemQueue.Enqueue(logItem);
        }

        private async void WriteLogWorker()
        {
            StreamWriter currentWriter = null;
            string currentFileName = "";
            string currentLogFolder = "";

            while (true)
            {
                await Task.Delay(10); // Reduce CPU usage by increasing delay

                if (!logItemQueue.TryDequeue(out var logItem))
                {
                    continue;
                }

                string subFolder = Path.Combine(LogFolder, DateTime.Now.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(subFolder))
                    Directory.CreateDirectory(subFolder);

                string fileName = Path.Combine(subFolder, $"{DateTime.Now.ToString("yyyy-MM-dd HH")}.log");
                if (logItem.WriteToNewFileLogName != "")
                {
                    fileName = Path.Combine(subFolder, $"{DateTime.Now.ToString("yyyy-MM-dd HH")}_{logItem.WriteToNewFileLogName}.log");
                }

                if (currentFileName != fileName || currentLogFolder != subFolder)
                {
                    currentWriter?.Dispose();
                    currentWriter = new StreamWriter(fileName, true);
                    currentFileName = fileName;
                    currentLogFolder = subFolder;
                }

                try
                {
                    currentWriter.WriteLine($"{logItem.Time:yyyy/MM/dd HH:mm:ss.ffff} {logItem.logFullLine}");
                    currentWriter.Flush(); // Ensure log is written even if the app crashes
                }
                catch (Exception)
                {
                    // Consider adding some logging for this exception.
                    continue;
                }

                if (logItem.show_console)
                {
                    DisplayLogInConsole(logItem);
                }
            }
        }

        private void DisplayLogInConsole(LogItem logItem)
        {
            ConsoleColor foreColor = ConsoleColor.White;
            switch (logItem.level)
            {
                case LogLevel.Trace:
                    foreColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Debug:
                    foreColor = ConsoleColor.White;
                    break;
                case LogLevel.Information:
                    foreColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.Warning:
                    foreColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    foreColor = ConsoleColor.Red;
                    break;
                case LogLevel.Critical:
                    foreColor = ConsoleColor.Magenta;
                    break;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"[{logItem.Time:yy-MM-dd HH:mm:ss.ffff}] ");
            Console.ForegroundColor = logItem.Color != ConsoleColor.White ? logItem.Color : foreColor;
            Console.WriteLine(logItem.logFullLine);
            Console.ResetColor(); // Reset to default colors to avoid color pollution
        }


    }
}
