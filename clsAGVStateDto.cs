using AGVSystemCommonNet6;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.TASK;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6
{
    public class clsAGVStateDto
    {
        [Key]
        public string AGV_Name { get; set; }
        public bool Enabled { get; set; }
        public string AGV_Description { get; set; } = "";
        /// <summary>
        /// 所屬車隊
        /// </summary>
        public VMS_GROUP Group { get; set; }
        public AGV_MODEL Model { get; set; }
        public MAIN_STATUS MainStatus { get; set; }
        public ONLINE_STATE OnlineStatus { get; set; }
        public string CurrentLocation { get; set; } = "";
        public string CurrentCarrierID { get; set; } = "";
        public double BatteryLevel { get; set; }
        public bool Connected { get; set; }
        public string TaskName { get; set; } = "";

        public TASK_RUN_STATUS TaskRunStatus { get; set; }
        public ACTION_TYPE TaskRunAction { get; set; }
        /// <summary>
        /// 當前執行的任務動作
        /// </summary>
        public ACTION_TYPE CurrentAction { get; set; }

        public double Theta { get; set; }
    }
}
