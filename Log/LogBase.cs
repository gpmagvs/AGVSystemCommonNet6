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
                await Task.Delay(20);
                try
                {
                    if (logItemQueue.TryDequeue(out LogItem? logItem))
                    {
                        WriteLog(logItem);
                        logItem.Dispose();
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

                ConsoleColor foreColor = ConsoleColor.White;
                ConsoleColor backColor = ConsoleColor.Black;
                switch (logItem.level)
                {
                    case LogLevel.Trace:
                        foreColor = ConsoleColor.Gray;
                        break;
                    case LogLevel.Debug:
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
                        foreColor = ConsoleColor.White;
                        backColor = ConsoleColor.Red;
                        break;
                    case LogLevel.None:
                        break;
                    default:
                        break;
                }

                void SetTimeStyle(LogLevel level)
                {
                    if (level != LogLevel.Critical)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                }

                SetTimeStyle(logItem.level);
                Console.Write("[");
                Console.Write(logItem.Time.ToString("MM/dd HH:mm:ss.ff") + "] ");

                clsLogConsoleStyle styles = StyleMap[logItem.level];

                Console.ForegroundColor = styles.ForeColor;
                Console.BackgroundColor = styles.BgColor;

                Console.WriteLine(logItem.logFullLine);
                Console.WriteLine(" ");

                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
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
