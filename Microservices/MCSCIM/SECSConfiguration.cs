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


    public class TransferReportConfiguration
    {

        public clsResultCodes ResultCodes { get; set; } = new clsResultCodes();

        public class clsResultCodes
        {
            public byte UnloadButCargoIDReadNotMatchedResultCode { get; set; } = 4;
            public byte UnloadButCargoIDReadFailResultCode { get; set; } = 5;
            public int EqUnloadButNoCargoResultCode { get; set; } = 100;
            public byte AGVDownWhenLDULDWithCargoResultCode { get; set; } = 101;
            public byte AGVDownWhenLDWithoutCargoResultCode { get; set; } = 102;
            public byte AGVDownWhenULDWithoutCargoResultCode { get; set; } = 144;
            public byte AGVDownWhenMovingToDestineResultCode { get; set; } = 145;
        }
    }

}
