using Microsoft.Extensions.Logging;

namespace AGVSystemCommonNet6.Log
{
    public class LogItem
    {
        public LogLevel level { get; } = LogLevel.Information;

        public string logMsg { get; } = string.Empty;

        public bool show_console { get; private set; }

        public DateTime Time { get; internal set; }

        public LogItem(LogLevel level, string logMsg, bool show_console = true)
        {
            this.level = level;
            this.logMsg = logMsg;
            this.show_console = show_console;
        }
        public LogItem(string logMsg)
        {
            this.logMsg = logMsg;
        }

        public string logFullLine => $" [{level}][{Caller}] {logMsg}{(exception != null ? exception.StackTrace : "")}";

        public string Caller { get; internal set; }

        public Exception exception { get; set; }
    }
}
