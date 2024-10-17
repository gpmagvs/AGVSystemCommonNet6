using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Alarm
{
    [Index(nameof(Time))]
    [Index(nameof(Checked))]
    public class clsAlarmDto
    {
        [Key]
        public DateTime Time { get; set; }
        public ALARM_LEVEL Level { get; set; }
        public ALARM_SOURCE Source { get; set; }
        private int _Alarmcode = 0;
        public int AlarmCode
        {
            get { return _Alarmcode; }
            set
            {
                _Alarmcode = value;
                if (AlarmManagerCenter.AGVsTrobleShootings.Keys.Count > 0)
                {
                    string TrobleShooting = ((ALARMS)_Alarmcode).ToString();
                    if (AlarmManagerCenter.AGVsTrobleShootings.ContainsKey(TrobleShooting))
                    {
                        TrobleShootingMethod = $"{AlarmManagerCenter.AGVsTrobleShootings[TrobleShooting].EN_TrobleShootingDescription}({AlarmManagerCenter.AGVsTrobleShootings[TrobleShooting].ZH_TrobleShootingDescription})";
                        TrobleShootingReference = AlarmManagerCenter.AGVsTrobleShootings[TrobleShooting].TrobleShootingFilePath;
                    }
                }
            }
        }
        public string Description => $"{Description_Zh}({Description_En})";
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

        private string _TrobleShootingMethod = "Reboot System(重啟系統)";
        public string TrobleShootingMethod
        {
            get { return _TrobleShootingMethod; }
            set
            {
                _TrobleShootingMethod = value;
            }
        }

        private string _TrobleShootingReference = "/AOI_SOP_000_AGV 當機處理.pdf";
        public string TrobleShootingReference
        {
            get { return _TrobleShootingReference; }
            set
            {
                _TrobleShootingReference = value;
            }
        }
    }
}
