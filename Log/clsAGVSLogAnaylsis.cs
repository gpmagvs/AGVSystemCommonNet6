using AGVSystemCommonNet6.AGVDispatch.Messages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Concurrent;

namespace AGVSystemCommonNet6.Log
{
    public class clsAGVSLogAnaylsis
    {
        public string logFolder;
        public (List<clsTaskDownloadData>, List<RunningStatus>, List<FeedbackData>) GetDatas(DateTime[] timedt_range)
        {
            DateTime startDate = new DateTime(timedt_range[0].Year, timedt_range[0].Month, timedt_range[0].Day, 0, 0, 0);
            DateTime endDate = new DateTime(timedt_range[1].Year, timedt_range[1].Month, timedt_range[1].Day, 23, 59, 59);
            //2023-11-06
            string[] dateFolders = Directory.GetDirectories(logFolder);
            IEnumerable<string> folders_matched = dateFolders.Where(path => folderNameToDate(Path.GetFileNameWithoutExtension(path)) >= startDate && folderNameToDate(Path.GetFileNameWithoutExtension(path)) <= endDate);
            var files = folders_matched.SelectMany(folder => Directory.GetFiles(folder)).Where(file_path => fileNameToDate(file_path) >= timedt_range[0] && fileNameToDate(file_path) <= timedt_range[1]);
            ConcurrentBag<clsTaskDownloadData> tkdList = new ConcurrentBag<clsTaskDownloadData>();
            ConcurrentBag<RunningStatus> runnList = new ConcurrentBag<RunningStatus>();
            ConcurrentBag<FeedbackData> feedbackDatas = new ConcurrentBag<FeedbackData>();
            List<Task> tasks = new List<Task>();
            foreach (var file in files)
            {
                var _tk = Task.Factory.StartNew(() =>
                {
                    (List<clsTaskDownloadData> tkd, List<RunningStatus> runningstat, List<FeedbackData> feedback) dataset = GetDatas(file);
                    for (int i = 0; i < dataset.tkd.Count; i++)
                    {
                        tkdList.Add(dataset.tkd[i]);
                    }
                    for (int i = 0; i < dataset.runningstat.Count; i++)
                    {
                        runnList.Add(dataset.runningstat[i]);
                    }
                    for (int i = 0; i < dataset.feedback.Count; i++)
                    {
                        feedbackDatas.Add(dataset.feedback[i]);
                    }

                });
                tasks.Add(_tk);
            }
            Task.WaitAll(tasks.ToArray());
            return (tkdList.ToList(), runnList.OrderBy(t => t.Time_Stamp_dt).ToList(), feedbackDatas.OrderBy(t => GetDateTime(t.TimeStamp)).ToList());
        }

        public (List<clsTaskDownloadData>, List<RunningStatus>, List<FeedbackData>) GetDatas(string logFilePath)
        {
            List<clsTaskDownloadData> outputs_task_download = new List<clsTaskDownloadData>();
            List<RunningStatus> outputs_runningStatus = new List<RunningStatus>();
            List<FeedbackData> outputs_feedback = new List<FeedbackData>();
            string[] lines = File.ReadAllLines(logFilePath);
            string _jsonstr = "";
            bool _jsonStart = false;
            foreach (string line in lines)
            {
                try
                {
                    bool isLineContain0301 = line.Contains("Header\": {\"0301\"");

                    if (line.Contains("TCP/IP") | isLineContain0301)
                    {
                        if (isLineContain0301)
                        {
                            var jsonStartIndex = line.IndexOf('{');
                            var jsonStr = line.Substring(jsonStartIndex, line.Length - jsonStartIndex);
                            var runningStatusRptMsg = JsonConvert.DeserializeObject<clsTaskDownloadMessage>(jsonStr);
                            clsTaskDownloadData running_status = runningStatusRptMsg.Header["0301"];
                            outputs_task_download.Add(running_status);
                            _jsonStart = false;
                            _jsonstr = "";
                            continue;
                        }
                        _jsonstr += "{";
                        _jsonStart = true;
                    }
                    else
                    {
                        if (line.Contains("}*"))
                        {
                            _jsonstr += "}";
                            _jsonStart = false;
                            if (_jsonstr.Contains("Header\": {\"0303"))
                            {
                                var runningStatusRptMsg = JsonConvert.DeserializeObject<clsTaskFeedbackMessage>(_jsonstr);
                                FeedbackData running_status = runningStatusRptMsg.Header["0303"];
                                outputs_feedback.Add(running_status);
                            }
                            if (_jsonstr.Contains("Header\": {\"0105"))
                            {
                                var runningStatusRptMsg = JsonConvert.DeserializeObject<clsRunningStatusReportMessage>(_jsonstr);
                                RunningStatus running_status = runningStatusRptMsg.Header["0105"];
                                outputs_runningStatus.Add(running_status);
                            }
                            _jsonstr = "";
                            //parse
                        }
                        else if (_jsonStart)
                        {
                            _jsonstr += line.Trim();
                        }

                    }
                }
                catch (Exception ex)
                {
                    _jsonStart = false;
                    _jsonstr = "";
                    Console.WriteLine($"发生错误: {ex.Message}");
                }

            }

            return (outputs_task_download, outputs_runningStatus, outputs_feedback);
        }
        public DateTime folderNameToDate(string folderName)
        {
            return DateTime.ParseExact(folderName, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces);
        }
        public DateTime fileNameToDate(string file_path)
        {
            var fileName = Path.GetFileNameWithoutExtension(file_path);
            return DateTime.ParseExact(fileName, "yyyy-MM-dd HH", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces);
        }

        public DateTime GetDateTime(string datetime_str, string format = "yyyyMMdd HH:mm:ss")
        {
            //20231106 10:00:00
            return DateTime.ParseExact(datetime_str, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces);
        }

        public List<clsTransferResult> AnalysisTransferTasks(DateTime[] timedt_range)
        {
            (List<clsTaskDownloadData> taskDownload, List<RunningStatus> runningStatus, List<FeedbackData> feedback) dataSet = GetDatas(timedt_range);
            var AgvStatus = dataSet.runningStatus;
            var taskFeedbackDatas = dataSet.feedback.OrderBy(t => GetDateTime(t.TimeStamp));
            var downloaddata = dataSet.taskDownload;
            var taskNameList = taskFeedbackDatas.Select(d => d.TaskName).Distinct().ToList();
            ConcurrentBag<clsTransferResult> results = new ConcurrentBag<clsTransferResult>();
            List<Task> taskList = new List<Task>();
            foreach (var task_name in taskNameList)
            {
                var _tk = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var downloads = downloaddata.Where(t => t.Task_Name == task_name).ToList();
                        if (downloads != null)
                        {
                            var actions = downloads.Where(d => d.Action_Type != ACTION_TYPE.None);
                            if (actions.Count() == 0)
                                return;
                            if (actions.Any(t => t.Action_Type == ACTION_TYPE.Load))
                            {
                                var start_feedback = taskFeedbackDatas.FirstOrDefault(t => t.TaskName == task_name && t.TaskStatus == TASK_RUN_STATUS.NAVIGATING);
                                var end_feedback = taskFeedbackDatas.LastOrDefault(t => t.TaskName == task_name && t.TaskStatus == TASK_RUN_STATUS.ACTION_FINISH);
                                var source = actions.FirstOrDefault(a => a.Action_Type == ACTION_TYPE.Unload);
                                if (source != null)
                                {
                                    var destine = actions.Last(a => a.Action_Type == ACTION_TYPE.Load);
                                    var AgvStatusStart = AgvStatus.FirstOrDefault(st => (st.Time_Stamp_dt - GetDateTime(start_feedback.TimeStamp, "yyyyMMdd HH:mm:ss")).TotalSeconds > 0.1);
                                    var AgvStatusEnd = AgvStatus.FirstOrDefault(st => (st.Time_Stamp_dt - GetDateTime(end_feedback.TimeStamp, "yyyyMMdd HH:mm:ss")).TotalSeconds > 0.1);
                                    var transfer_record = new clsTransferResult
                                    {
                                        TaskName = task_name,
                                        StartTime = GetDateTime(start_feedback.TimeStamp),
                                        EndTime = GetDateTime(end_feedback.TimeStamp),
                                        From = source.Destination,
                                        To = destine.Destination,
                                        StartLoc = start_feedback.LastVisitedNode,
                                        StartStatus = AgvStatusStart,
                                        EndStatus = AgvStatusEnd
                                    };
                                    results.Add(transfer_record);
                                }
                                else
                                {

                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                    }

                });
                taskList.Add(_tk);
            }
            Task.WaitAll(taskList.ToArray());
            var _results = results.OrderBy(t => t.StartTime).ToList();
            return _results;

        }
        public class clsTransferResult
        {
            public string TaskName { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public double TimeSpend { get => (EndTime - StartTime).TotalSeconds; }
            public int From { get; set; }
            public int To { get; set; }
            public int StartLoc { get; set; }
            public double BatLvStart { get => StartStatus.Electric_Volume[0]; }
            public double BatLvEnd { get => EndStatus.Electric_Volume[0]; }
            public double BatLoss { get => BatLvStart - BatLvEnd; }
            public double Odometry { get => EndStatus.Odometry - StartStatus.Odometry; }
            public RunningStatus StartStatus { get; set; }
            public RunningStatus EndStatus { get; set; }

        }
    }
}
