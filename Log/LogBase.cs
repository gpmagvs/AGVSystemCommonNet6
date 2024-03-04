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
    public class LogBase : IDisposable
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

        private Dictionary<LogLevel, clsLogConsoleStyle> StyleMap = new Dictionary<LogLevel, clsLogConsoleStyle>()
        {
            { LogLevel.Information, new clsLogConsoleStyle( ConsoleColor.Cyan, ConsoleColor.Black) },
            { LogLevel.Warning, new clsLogConsoleStyle( ConsoleColor.Yellow, ConsoleColor.Black) },
            { LogLevel.Error, new clsLogConsoleStyle( ConsoleColor.Red, ConsoleColor.Black) },
            { LogLevel.Critical, new clsLogConsoleStyle( ConsoleColor.White, ConsoleColor.Red) },
            { LogLevel.Trace, new clsLogConsoleStyle( ConsoleColor.Gray, ConsoleColor.Black) },
        };

        private ConcurrentQueue<LogItem> logItemQueue = new ConcurrentQueue<LogItem>();
        private Task WriteLogToFileTask;
        private bool disposedValue;

        public LogBase() { }
        public LogBase(string LogFolderName)
        {
            this.LogFolderName = LogFolderName;
        }

        public void Log(LogItem logItem, string caller_class_name = "")
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

            while (!disposedValue)
            {
                try
                {
                    if (logItemQueue.TryDequeue(out LogItem? logItem))
                    {
                        WriteLog(logItem);
                        logItem.Dispose();
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
                catch (Exception ex)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(ex.Message);

                }
                
            }
        }
        public void WriteLog(LogItem logItem)
        {
            string subFolder = Path.Combine(LogFolder, DateTime.Now.ToString("yyyy-MM-dd"));

            if (!Directory.Exists(subFolder))
                Directory.CreateDirectory(subFolder);

            string fileName = Path.Combine(subFolder, $"{DateTime.Now.ToString("yyyy-MM-dd HH")}.log");
            if (logItem.WriteToNewFileLogName != "")
            {
                string NewLogfileName = Path.Combine(subFolder, $"{DateTime.Now.ToString("yyyy-MM-dd HH")}_{logItem.WriteToNewFileLogName}.log");
                try
                {
                    using StreamWriter writer = new StreamWriter(NewLogfileName, true);
                    writer.WriteLine(string.Format("{0} {1}", logItem.Time.ToString("yyyy/MM/dd HH:mm:ss.ffff"), logItem.logFullLine));
                }
                catch (Exception)
                {
                    return;
                }
            }
            try
            {
                using StreamWriter writer = new StreamWriter(fileName, true);
                writer.WriteLine(string.Format("{0} {1}", logItem.Time.ToString("yyyy/MM/dd HH:mm:ss.ffff"), logItem.logFullLine));
            }
            catch (Exception)
            {
                return;
            }

            if (logItem.show_console)
            {
                DisplayLogInConsole(logItem);
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
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                }
                logItemQueue.Clear();
                logItemQueue = null;
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~LogBase()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        public class clsLogConsoleStyle
        {
            public ConsoleColor ForeColor { get; set; } = ConsoleColor.White;
            public ConsoleColor BgColor { get; set; } = ConsoleColor.Black;

            public clsLogConsoleStyle(ConsoleColor foreColor, ConsoleColor bgColor)
            {
                this.ForeColor = foreColor;
                this.BgColor = bgColor;
            }
        }

    }
}
