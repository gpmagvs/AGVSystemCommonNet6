using AGVSystemCommonNet6.AGVDispatch.Messages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace AGVSystemCommonNet6.Log
{
    public class clsAGVSLogAnaylsis
    {
        public string logFolder;
        public List<RunningStatus> GetRunningStatusDto(DateTime[] timedt_range)
        {
            DateTime startDate = new DateTime(timedt_range[0].Year, timedt_range[0].Month, timedt_range[0].Day, 0, 0, 0);
            DateTime endDate = new DateTime(timedt_range[1].Year, timedt_range[1].Month, timedt_range[1].Day, 23, 59, 59);
            //2023-11-06
            string[] dateFolders = Directory.GetDirectories(logFolder);
            IEnumerable<string> folders_matched = dateFolders.Where(path => folderNameToDate(Path.GetFileNameWithoutExtension(path)) >= startDate && folderNameToDate(Path.GetFileNameWithoutExtension(path)) <= endDate);
            var files = folders_matched.SelectMany(folder => Directory.GetFiles(folder)).Where(file_path => fileNameToDate(file_path) >= timedt_range[0] && fileNameToDate(file_path) <= timedt_range[1]);

            return files.SelectMany(file => GetRunningStatusDto(file)).OrderBy(status => GetDateTime(status.Time_Stamp)).ToList();

            DateTime folderNameToDate(string folderName)
            {
                return DateTime.ParseExact(folderName, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces);
            }
            DateTime fileNameToDate(string file_path)
            {
                var fileName = Path.GetFileNameWithoutExtension(file_path);
                return DateTime.ParseExact(fileName, "yyyy-MM-dd HH", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces);
            }

            DateTime GetDateTime(string datetime_str)
            {
                //20231106 10:00:00
                return DateTime.ParseExact(datetime_str, "yyyyMMdd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces);
            }
        }
        public List<RunningStatus> GetRunningStatusDto(string logFilePath)
        {
            List<RunningStatus> outputs = new List<RunningStatus>();
            string[] lines = File.ReadAllLines(logFilePath);
            string _jsonstr = "";
            bool _jsonStart = false;
            foreach (string line in lines)
            {
                try
                {
                    if (line.Contains("TCP/IP"))
                    {
                        _jsonstr += "{";
                        _jsonStart = true;
                    }
                    else
                    {
                        if (line.Contains("}*"))
                        {
                            _jsonstr += "}";
                            _jsonStart = false;
                            if (_jsonstr.Contains("0105"))
                            {
                                var runningStatusRptMsg = JsonConvert.DeserializeObject<clsRunningStatusReportMessage>(_jsonstr);
                                RunningStatus running_status = runningStatusRptMsg.Header["0105"];
                                outputs.Add(running_status);
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

            return outputs;
        }

    }
}
