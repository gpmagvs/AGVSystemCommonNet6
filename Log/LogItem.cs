using Microsoft.Extensions.Logging;

namespace AGVSystemCommonNet6.Log
{
    public class LogItem : IDisposable
    {

        private Dictionary<LogLevel, string> LevelDisplayTextMap = new Dictionary<LogLevel, string>
        {
            { LogLevel.Error, "ERRO" },
            { LogLevel.Warning, "WARN" },
            { LogLevel.Information, "INFO" },
            { LogLevel.Trace, "TRACE" },
            { LogLevel.Critical, "CRITI" },
            { LogLevel.Debug, "DEBUG" },
        };
        private bool disposedValue;

        public LogLevel level { get; } = LogLevel.Information;

        public string logMsg { get; private set; } = string.Empty;

        public bool show_console { get; private set; }

        public DateTime Time { get; internal set; }

        public ConsoleColor Color { get; set; } = ConsoleColor.White;

        public string WriteToNewFileLogName { get; set; } = "";

        public LogItem(LogLevel level, string logMsg, bool show_console = true, ConsoleColor color = ConsoleColor.White, string WriteToNewFileLogName = "")
        {
            this.level = level;
            this.logMsg = logMsg;
            this.show_console = show_console;
            this.Color = color;
            this.WriteToNewFileLogName = WriteToNewFileLogName;
        }
        public LogItem(string logMsg)
        {
            this.logMsg = logMsg;
        }

        public string logFullLine
        {
            get
            {
                string msg = $"{logMsg}{(exception != null ? exception.StackTrace + (exception.InnerException == null ? "" : "\r\nInner exception:" + exception.InnerException.StackTrace) : "")}";
                string logEntry = "|{0,-5}|{1,-15}| {2}";
                string console_display = string.Format(logEntry, LevelDisplayTextMap[level], Caller, msg);
                return console_display;
            }
        }

        public string Caller { get; internal set; }

        public Exception exception { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                }
                WriteToNewFileLogName = Caller = logMsg = null;
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~LogItem()
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
    }
}
