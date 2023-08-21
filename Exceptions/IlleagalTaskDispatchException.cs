using AGVSystemCommonNet6.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Exceptions
{
    public class IlleagalTaskDispatchException : VMSExceptionAbstract
    {
        public IlleagalTaskDispatchException(ALARMS alarm_code)
        {
            Alarm_Code = alarm_code;
        }
        public override ALARMS Alarm_Code { get; set; }
    }
}
