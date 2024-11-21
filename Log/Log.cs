using System.Diagnostics;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AGVSystemCommonNet6.Log
{
    public class LOG
    {
        private static LogBase _logger = new LogBase();
        public static string LogFolder => _logger.LogFolder;
        public static void SetLogFolderName(string logFolderName)
        {
            _logger.LogFolderName = logFolderName;
        }
        public static async Task TRACE(string info, string caller_class_name, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            await _logger.LogAsync(new LogItem(LogLevel.Trace, info, show_console, color, NewLogFileEndStr), caller_class_name);
        }
        public static async Task TRACE(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            await TRACE(info, caller_class_name, show_console, color);
        }
        public static async Task INFO(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            await _logger.LogAsync(new LogItem(LogLevel.Information, info, show_console, color, NewLogFileEndStr), caller_class_name);
        }

        public static async Task WARN(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            await _logger.LogAsync(new LogItem(LogLevel.Warning, info, show_console, color, NewLogFileEndStr), caller_class_name);
        }
        public static async Task ERROR(string info, Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            string msg = string.Format("{0}。Exception Message:{1}", info, ex.Message + "\r\n" + ex.StackTrace);
            await _logger.LogAsync(new LogItem(LogLevel.Error, msg, show_console, color, NewLogFileEndStr) { exception = ex }, caller_class_name);
        }
        public static async Task ERROR(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            await _logger.LogAsync(new LogItem(LogLevel.Error, string.Format("{0}", info, show_console, color, NewLogFileEndStr)), caller_class_name);
        }

        public static async Task ERROR(Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name;
            string msg = string.Format("Message:{0}。StackTrace:{1}", ex.Message, ex.StackTrace);
            await _logger.LogAsync(new LogItem(LogLevel.Error, msg, show_console, color, NewLogFileEndStr) { exception = ex }, caller_class_name);
        }

        public static async Task Critical(string msg, Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            string _msg = string.Format("{0}。Exception Message:{1}", msg, ex.Message + "\r\n" + ex.StackTrace);
            TRACE(_msg, caller_class_name);
            await _logger.LogAsync(new LogItem(LogLevel.Critical, _msg, show_console, color, NewLogFileEndStr) { exception = ex }, caller_class_name);
        }
        public static async Task Critical(Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            string _msg = string.Format("Message:{0}。StackTrace:{1}", ex.Message, ex.StackTrace);
            await _logger.LogAsync(new LogItem(LogLevel.Critical, _msg, show_console, color, NewLogFileEndStr) { exception = ex }, caller_class_name);
        }

        public static async Task Critical(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            await _logger.LogAsync(new LogItem(LogLevel.Critical, info, show_console, color, NewLogFileEndStr), caller_class_name);
        }

    }
}
