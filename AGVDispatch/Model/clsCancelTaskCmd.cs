using AGVSystemCommonNet6.AGVDispatch.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch.Model
{
    public class clsCancelTaskCmd
    {
        public DateTime TimeStamp { get; set; }
        public RESET_MODE ResetMode { get; set; } = RESET_MODE.CYCLE_STOP;
        public string Task_Name { get; set; } = "";

        public string CancelReasonMessage { get; set; } = "";
    }
}
