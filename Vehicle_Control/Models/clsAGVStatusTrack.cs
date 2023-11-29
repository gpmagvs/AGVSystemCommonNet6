using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.Vehicle_Control.Models
{
    public class clsAGVStatusTrack
    {
        public clsAGVStatusTrack() { }

        [PrimaryKey]
        public DateTime Time { get; set; } = DateTime.MinValue;

        public SUB_STATUS Status { get; set; } = SUB_STATUS.DOWN;

        public double BatteryLevel1 { get; set; } = 0;
        public double BatteryLevel2 { get; set; } = 0;
        public double BatteryVoltage1 { get; set; } = 0;
        public double BatteryVoltage2 { get; set; } = 0;

        public string ExecuteTaskName { get; set; } = "";
        public string ExecuteTaskSimpleName { get; set; } = "";
    }
}
