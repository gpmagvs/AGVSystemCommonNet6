using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Availability
{
    public class clsPointPassInfo
    {
        [Key]
        public string DataKey { get; set; } = "";
        public DateTime Time { get; set; }
        public DateTime LeaveTime { get; set; }
        public DateTime StartWaitTrafficSolveTime { get; set; } = DateTime.MinValue;
        public DateTime EndWaitTrafficSolveTime { get; set; } = DateTime.MinValue;
        public int Tag { get; set; } = 0;
        public double Duration { get; set; }
        public string AGVName { get; set; } = "";
        public string TaskName { get; set; } = "";

        public int StageWhenReach { get; set; } = 0;

        public int StageWhenLeaving { get; set; } = 0;

        public bool IsWaitingTrafficControlSolve { get; set; } = false;

    }
}
