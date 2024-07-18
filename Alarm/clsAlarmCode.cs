using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Alarm
{
    public class clsAlarmCode
    {
        public ALARMS AlarmCode { get; set; }
        public string Description => $"{Description_Zh}({Description_En})";
        public string Description_Zh { get; set; } = "";
        public string Description_En { get; set; } = "";

        public string En_TrobleShootingDescription { get; set; } = "";
        public string ZH_TrobleShootingDescription { get; set; } = "";
        public string TrobleShootingFilePath { get; set; } = "";
    }
}
