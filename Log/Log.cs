using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using WebSocketSharp;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AGVSystemCommonNet6.Log
{
    public class LOG
    {
        public static bool TraceShow = true;
        public static bool InfoShow = true;
        public static bool WarningShow = true;
        public static bool ErrorShow = true;
        public static bool CriticalShow = true;

        private static LogBase _logger = new LogBase();
        public static string LogFolder => _logger.LogFolder;
        public static void SetLogFolderName(string logFolderName)
        {
            _logger.LogFolderName = logFolderName;
        }
        public static void TRACE(string info, string caller_class_name, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            _logger.Log(new LogItem(LogLevel.Trace, info, TraceShow && show_console, color, NewLogFileEndStr), caller_class_name);
        }
        public static void TRACE(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = GetCallerClassName();
            TRACE(info, caller_class_name, TraceShow && show_console, color, NewLogFileEndStr: NewLogFileEndStr);
        }
        public static void INFO(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = GetCallerClassName();
            _logger.Log(new LogItem(LogLevel.Information, info, InfoShow && show_console, color, NewLogFileEndStr), caller_class_name);
        }

        public static void WARN(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = GetCallerClassName();
            _logger.Log(new LogItem(LogLevel.Warning, info, WarningShow && show_console, color, NewLogFileEndStr), caller_class_name);
        }
        public static void ERROR(string info, Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = GetCallerClassName();
            string msg = string.Format("{0}。Exception Message:{1}", info, ex.Message + "\r\n" + ex.StackTrace);
            _logger.Log(new LogItem(LogLevel.Error, msg, ErrorShow && show_console, color, NewLogFileEndStr) { exception = ex }, caller_class_name);
        }
        public static void ERROR(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = GetCallerClassName();
            _logger.Log(new LogItem(LogLevel.Error, info, ErrorShow && show_console, color, NewLogFileEndStr), caller_class_name);
        }

        public static void ERROR(Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = GetCallerClassName();
            string msg = string.Format("Message:{0}。StackTrace:{1}", ex.Message, ex.StackTrace);
            _logger.Log(new LogItem(LogLevel.Error, msg, ErrorShow && show_console, color, NewLogFileEndStr) { exception = ex }, caller_class_name);
        }

        public static void Critical(string msg, Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = GetCallerClassName();
            string _msg = string.Format("{0}。Exception Message:{1}", msg, ex.Message + "\r\n" + ex.StackTrace);
            TRACE(_msg, caller_class_name);
            _logger.Log(new LogItem(LogLevel.Critical, _msg, CriticalShow && show_console, color, NewLogFileEndStr) { exception = ex }, caller_class_name);
        }
        public static void Critical(Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = GetCallerClassName();
            string _msg = string.Format("Message:{0}。StackTrace:{1}", ex.Message, ex.StackTrace);
            _logger.Log(new LogItem(LogLevel.Critical, _msg, CriticalShow && show_console, color, NewLogFileEndStr) { exception = ex }, caller_class_name);
        }

        public static void Critical(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White, string NewLogFileEndStr = "")
        {
            var caller_class_name = GetCallerClassName();
            _logger.Log(new LogItem(LogLevel.Critical, info, CriticalShow && show_console, color, NewLogFileEndStr), caller_class_name);
        }
        public static string GetCallerClassName()
        {
            var caller_class_declaring = new StackTrace().GetFrame(2).GetMethod().DeclaringType;
            if (caller_class_declaring == null || caller_class_declaring.DeclaringType == null|| caller_class_declaring.DeclaringType.Name.Contains("<>c__DisplayClass"))
                return "ClassClass";
            return caller_class_declaring.DeclaringType.Name;
        }
    }
}
