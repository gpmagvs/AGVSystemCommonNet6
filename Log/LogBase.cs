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

            while (true)
            {
                Thread.Sleep(1);

                if (logItemQueue.TryDequeue(out var logItem))
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
                            continue;
                        }
                    }
                    try
                    {
                        using StreamWriter writer = new StreamWriter(fileName, true);
                        writer.WriteLine(string.Format("{0} {1}", logItem.Time.ToString("yyyy/MM/dd HH:mm:ss.ffff"), logItem.logFullLine));
                    }
                    catch (Exception)
                    {
                        continue;
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
                                foreColor = ConsoleColor.Red;
                                break;
                            case LogLevel.None:
                                break;
                            default:
                                break;
                        }
                        Console.Write("[");
                        //Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(logItem.Time.ToString("yy-MM-dd HH:mm:ss.ffff") + "] ");
                        Console.ForegroundColor = logItem.Color != ConsoleColor.White ? logItem.Color : foreColor;
                        Console.BackgroundColor = backColor;
                        Console.WriteLine(logItem.logFullLine);
                        Console.WriteLine(" ");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
            }
        }

    }
}
