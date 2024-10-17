using AGVSystemCommonNet6.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.ResponseModel
{
    public class clsAGVSTaskReportResponse : clsResponseBase
    {
        public clsAGVSTaskReportResponse()
        { }

        public clsAGVSTaskReportResponse(bool confirm, ALARMS AlarmCode, string message, string message_en) : base(confirm, AlarmCode, message, message_en)
        { }
    }
}
