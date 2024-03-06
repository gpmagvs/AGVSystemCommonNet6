using Microsoft.Extensions.Logging;

namespace AGVSystemCommonNet6.Log
{
    public class LogItem
    {
        public LogLevel level { get; } = LogLevel.Information;

        public string logMsg { get; } = string.Empty;

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

        public string logFullLine => $" |{level}|{logMsg}{(exception != null ? exception.StackTrace + (exception.InnerException == null ? "" : "\r\nInner exception:" + exception.InnerException.StackTrace) : "")}";

        public string Caller { get; internal set; }

        public Exception exception { get; set; }
    }
}
