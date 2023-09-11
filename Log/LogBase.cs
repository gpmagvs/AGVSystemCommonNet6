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
        internal string LogFolderName = "VMS LOG";
        private ConcurrentQueue<LogItem> logItemQueue = new ConcurrentQueue<LogItem>();
        private Task WriteLogToFileTask;


        public void Log(LogItem logItem, string caller_class_name = "")
        {
            if (WriteLogToFileTask == null)
            {
                WriteLogToFileTask = Task.Factory.StartNew(() => WriteLogWorker());
            }
            logItem.Caller = caller_class_name;
            logItem.Time = DateTime.Now;
            logItemQueue.Enqueue(logItem);
        }

        private async void WriteLogWorker()
        {
            if (!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);

            while (true)
            {
                await Task.Delay(1);

                if (logItemQueue.TryDequeue(out var logItem))
                {
                    //2023-09-12/Info/2023-09-12-12.log
                    string subFolder = Path.Combine(LogFolder, DateTime.Now.ToString("yyyy-MM-dd"));

                    if (!Directory.Exists(subFolder))
                        Directory.CreateDirectory(subFolder);

                    string fileName = Path.Combine(subFolder, $"{DateTime.Now.ToString("yyyy-MM-dd HH")}.log");

                    try
                    {
                        using (StreamWriter writer = new StreamWriter(fileName, true))
                        {
                            writer.WriteLine(string.Format("{0} {1}", logItem.Time.ToString("yyyy/MM/dd HH:mm:ss.ffff"), logItem.logFullLine));
                        }
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
                                foreColor = ConsoleColor.White;
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

                        if (logItem.level != LogLevel.Trace)
                        {
                            Console.Write(" ");
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(logItem.Time + " ");
                            Console.ForegroundColor = logItem.Color != ConsoleColor.White ? logItem.Color : foreColor;
                            Console.BackgroundColor = backColor;
                            Console.WriteLine(logItem.logFullLine);
                            Console.WriteLine(" ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }

                    }
                }
            }
            }

        }
    }
