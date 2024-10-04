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

        public clsAlarmCode()
        {

        }
        public clsAlarmCode(ALARMS alarmCode, string description_Zh, string description_En, string en_TrobleShootingDescription, string zH_TrobleShootingDescription, string trobleShootingFilePath)
        {
            AlarmCode = alarmCode;
            Description_Zh = description_Zh;
            Description_En = description_En;
            En_TrobleShootingDescription = en_TrobleShootingDescription;
            ZH_TrobleShootingDescription = zH_TrobleShootingDescription;
            TrobleShootingFilePath = trobleShootingFilePath;
        }
    }
}
