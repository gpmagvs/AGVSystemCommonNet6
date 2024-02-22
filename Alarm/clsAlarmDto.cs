using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Alarm
{
    public class clsAlarmDto
    {
        [Key]
        public DateTime Time { get; set; }
        public ALARM_LEVEL Level { get; set; }
        public ALARM_SOURCE Source { get; set; }
        public int AlarmCode { get; set; }
        public string Description => $"{Description_En}({Description_Zh})";
        public string Description_Zh { get; set; } = "";
        public string Description_En { get; set; } = "";
        public string OccurLocation { get; set; } = "";
        public string Equipment_Name { get; set; } = "";
        public string Task_Name { get; set; } = "";
        /// <summary>
        /// 持續時間
        /// </summary>
        public int Duration { get; set; }
        public bool Checked { get; set; }
        public string ResetAalrmMemberName { get; set; } = "";
    }
}
