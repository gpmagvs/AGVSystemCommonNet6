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
        private static LogBase _logger = new LogBase();
        public static string LogFolder => _logger.LogFolder;
        public static void SetLogFolderName(string logFolderName)
        {
            _logger.LogFolderName = logFolderName;
        }
        public static void TRACE(string info, string caller_class_name, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            _logger.Log(new LogItem(LogLevel.Trace, info, show_console, color), caller_class_name);
        }
        public static void TRACE(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            TRACE(info, caller_class_name, show_console, color);
        }
        public static void INFO(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            _logger.Log(new LogItem(LogLevel.Information, info, show_console, color), caller_class_name);
        }

        public static void WARN(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            _logger.Log(new LogItem(LogLevel.Warning, info, show_console, color), caller_class_name);
        }
        public static void ERROR(string info, Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            string msg = string.Format("{0}。Exception Message:{1}", info, ex.Message + "\r\n" + ex.StackTrace);
            _logger.Log(new LogItem(LogLevel.Error, msg, show_console, color) { exception = ex }, caller_class_name);
        }
        public static void ERROR(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            _logger.Log(new LogItem(LogLevel.Error, string.Format("{0}", info, show_console, color)), caller_class_name);
        }

        public static void ERROR(Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name;
            string msg = string.Format("Message:{0}。StackTrace:{1}", ex.Message, ex.StackTrace);
            _logger.Log(new LogItem(LogLevel.Error, msg, show_console, color) { exception = ex }, caller_class_name);
        }

        public static void Critical(string msg, Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            string _msg = string.Format("{0}。Exception Message:{1}", msg, ex.Message + "\r\n" + ex.StackTrace);
            TRACE(_msg, caller_class_name);
            _logger.Log(new LogItem(LogLevel.Critical, _msg, show_console, color) { exception = ex }, caller_class_name);
        }
        public static void Critical(Exception ex, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            string _msg = string.Format("Message:{0}。StackTrace:{1}", ex.Message, ex.StackTrace);
            _logger.Log(new LogItem(LogLevel.Critical, _msg, show_console, color) { exception = ex }, caller_class_name);
        }

        public static void Critical(string info, bool show_console = true, ConsoleColor color = ConsoleColor.White)
        {
            var caller_class_name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name; ;
            _logger.Log(new LogItem(LogLevel.Critical, info, show_console, color), caller_class_name);
        }

    }
}
