using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.MCSCIM
{
    public class SECSConfiguration
    {
        public string DeviceID { get; set; } = "2F_AGVC02";
        public string CarrierLOCPrefixName { get; set; } = "ABFRACK005";
        public string SystemID { get; set; } = "022";

        public int UnknowTrayIDFlowNumberUsed { get; set; } = 0;
        public int UnknowRackIDFlowNumberUsed { get; set; } = 0;
        public int DoubleUnknowDFlowNumberUsed { get; set; } = 0;
        public int DoubleUnknowRackIDFlowNumberUsed { get; set; } = 0;
        public int MissMatchTrayIDFlowNumberUsed { get; set; } = 0;
        public int MissMatchRackIDFlowNumberUsed { get; set; } = 0;

    }

    public class SECSAlarmConfiguration
    {
        public enum ALARM_TABLE_VERSION
        {
            GPM = 0,
            KGS = 1,
        }

        public ALARM_TABLE_VERSION Version { get; set; }


    }
}
