using AGVSystemCommonNet6.AGVDispatch.Messages;
using SQLite;
using System.ComponentModel;

namespace AGVSystemCommonNet6.Vehicle_Control.Models
{
    public class LDULDRecord
    {
        [PrimaryKey]
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime EQActionFinishTime { get; set; }
        public ACTION_TYPE Action { get; set; } = ACTION_TYPE.Load;
        public int WorkStationTag { get; set; }
        public string TaskName { get; set; } = "";
        public string CargoID_FromAGVS { get; set; } = "";
        public string CargoID_Reader { get; set; } = "";
        public bool ExistSensor1_State = false;
        public bool ExistSensor2_State = false;


        public string ExistSensorState
        {
            get
            {
                return string.Join(",", new int[2] { ExistSensor1_State ? 1 : 0, ExistSensor2_State ? 1 : 0 });
            }
            set
            {
                var splited = value.Split(',');
                ExistSensor1_State = splited.Length >= 1 ? splited[0] == "1" : false;
                ExistSensor2_State = splited.Length >= 2 ? splited[1] == "1" : false;
            }
        }
        public double ParkLocX { get; set; }
        public double ParkLocY { get; set; }
    }
}
