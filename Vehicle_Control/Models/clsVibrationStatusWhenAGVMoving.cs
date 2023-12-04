using Newtonsoft.Json;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Vehicle_Control.Models
{
    public class clsVibrationStatusWhenAGVMoving : TaskStatusModelBase
    {
        public double MaxAccX { get; set; } = 0;
        public double MaxAccY { get; set; } = 0;

        public string VirbrationRecordsJsonString { get; set; } = "";

        public List<clsVibrationRecord> VibrationRecords =>
            VirbrationRecordsJsonString == null ? new List<clsVibrationRecord>() :
            JsonConvert.DeserializeObject<List<clsVibrationRecord>>(this.VirbrationRecordsJsonString);
        public clsVibrationStatusWhenAGVMoving() : base()
        {

        }
        public clsVibrationStatusWhenAGVMoving(List<clsVibrationRecord> VibrationRecords)
        {
            if (VibrationRecords.Count == 0)
                return;
            VirbrationRecordsJsonString = VibrationRecords.ToJson(Newtonsoft.Json.Formatting.None);
            MaxAccX = VibrationRecords.Select(v => Math.Abs(v.AccelermetorValue.x)).Max();
            MaxAccY = VibrationRecords.Select(v => Math.Abs(v.AccelermetorValue.y)).Max();
        }

    }
    public class clsVibrationRecord
    {
        public double LocX { get; set; }
        public double LocY { get; set; }
        public double Theta { get; set; }
        public DateTime Time { get; set; } = DateTime.MinValue;
        public string Time_str => Time.ToString("yyyy/MM/dd HH:mm:ss.ffff");
        public Vector3 AccelermetorValue { get; set; } = new Vector3(0, 0, 0);
    }
}
