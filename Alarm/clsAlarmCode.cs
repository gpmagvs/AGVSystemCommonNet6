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
    }
}
