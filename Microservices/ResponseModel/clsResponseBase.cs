using AGVSystemCommonNet6.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.ResponseModel
{
    public class clsResponseBase
    {
        public clsResponseBase() { }
        public clsResponseBase(bool confirm, ALARMS AlarmCode = ALARMS.NONE, string message = "")
        {
            this.confirm = confirm;
            this.message = message;
        }
        public bool confirm { get; set; } = false;
        public string message { get; set; } = "";

        public ALARMS AlarmCode { get; set; } = ALARMS.NONE;
        public int alarmcode_int { get; set; } = 0;
    }
}
