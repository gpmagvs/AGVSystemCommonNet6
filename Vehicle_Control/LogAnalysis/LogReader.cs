using AGVSystemCommonNet6.Vehicle_Control.LogAnalysis.Models;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Vehicle_Control.LogAnalysis
{
    public class LogReader
    {
        private readonly string folder;

        public LogReader(string folder)
        {
            this.folder = folder;
        }

        public List<clsIOLog> QueryIO(DateTime from, DateTime to)
        {
            List<string> filesMatched = SearchFiles(from, to);
            List<clsIOLog> outputs = new List<clsIOLog>();
            foreach (string file in filesMatched)
            {
                Dictionary<DateTime, string> loglines = QueryLogLines(file, from, to);
                Dictionary<DateTime, string> ioLogLines = loglines.Where(log => log.Value.Contains("[IO]")).ToDictionary(kp => kp.Key, kp => kp.Value);
                if (ioLogLines.Count > 0)
                {
                    var io_logs = ioLogLines.Select(kp => ParseLogLineToIOObject(kp.Value)).ToList();
                    if (io_logs.Count() > 0)
                        outputs.AddRange(io_logs);
                }
            }
            return outputs;
        }

        private clsIOLog ParseLogLineToIOObject(string msg)
        {
            clsIOLog.DIRECTION _direction = msg.Split('-')[1].Substring(1, 1) == "X" ? clsIOLog.DIRECTION.INPUT : clsIOLog.DIRECTION.OUTPUT;
            string _name = msg.Replace(" to : ", "-").Split('-')[2];
            var _timestamp_str = msg.Substring(0, 24);
            var _Value = msg.Replace(" to : ", "-").Split('-')[3] == "0" ? 0 : 1;
            if (DateTime.TryParseExact(_timestamp_str, "yyyy/MM/dd HH:mm:ss.ffff", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var timestamp))
            {

                return new clsIOLog
                {
                    Time = timestamp,
                    IO_Direction = _direction,
                    IO_Name = _name,
                    Value = _Value
                };
            }
            else
                return null;
        }
        private Dictionary<DateTime, string> QueryLogLines(string filename, DateTime from, DateTime to)
        {
            using StreamReader sr = new StreamReader(filename);
            string _line = "";
            Dictionary<DateTime, string> lines = new Dictionary<DateTime, string>();
            while ((_line = sr.ReadLine()) != null)
            {
                try
                {
                    if (_line.Length < 24)
                        continue;
                    var _timestamp_str = _line.Substring(0, 24);
                    if (DateTime.TryParseExact(_timestamp_str, "yyyy/MM/dd HH:mm:ss.ffff", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var timestamp))
                    {
                        if (timestamp < from)
                            continue;
                        if (timestamp > to)
                            break;
                        if (timestamp >= from && timestamp <= to)
                        {
                            lines.Add(timestamp, _line);
                        }
                    }
                    else
                        continue;
                }
                catch (Exception ex)
                {
                    continue;
                }
                
            }
            return lines;
        }
        private List<string> SearchFiles(DateTime from, DateTime to)
        {
            var _from = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0);
            var newTo = to.AddDays(1);
            var _to = new DateTime(newTo.Year, newTo.Month, newTo.Day, 0, 0, 0);
            //2023-11-22 05.log
            return Directory.GetFiles(folder, "", SearchOption.AllDirectories).Where(filePath => FileNameToDateTime(filePath) >= _from && FileNameToDateTime(filePath) <= _to).ToList();
        }
        private DateTime FileNameToDateTime(string fileName, string format = "yyyy-MM-dd HH")
        {
            var fileNameWithOutExt = Path.GetFileNameWithoutExtension(fileName);
            if (DateTime.TryParseExact(fileNameWithOutExt, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var date))
                return date;
            else
                return DateTime.MinValue;
        }
    }
}
