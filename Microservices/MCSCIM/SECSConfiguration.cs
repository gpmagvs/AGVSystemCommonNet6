using AGVSystemCommonNet6.Alarm;
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

        public byte GetResultCode(ALARMS agvsInnerAlarm)
        {
            if (agvsInnerAlarm == ALARMS.UNLOAD_BUT_CARGO_ID_READ_FAIL)
                return ResultCodes.UnloadButCargoIDReadFailResultCode;
            else if (agvsInnerAlarm == ALARMS.UNLOAD_BUT_CARGO_ID_NOT_MATCHED)
                return ResultCodes.UnloadButCargoIDReadNotMatchedResultCode;
            else if (agvsInnerAlarm == ALARMS.EQ_UNLOAD_REQUEST_ON_BUT_NO_CARGO)
                return ResultCodes.EqUnloadButNoCargoResultCode;
            else if (agvsInnerAlarm == ALARMS.EQ_UNLOAD_REQUEST_IS_NOT_ON)
                return ResultCodes.SourceEqUnloadReqeustOff;
            else if (agvsInnerAlarm == ALARMS.EQ_UNLOAD_REQUEST_ON_BUT_POSE_NOT_UP)
                return ResultCodes.SourceEqMachechStateErrorResultCode;
            else if (agvsInnerAlarm == ALARMS.Source_Eq_Status_Down)
                return ResultCodes.SourceEqDownResultCode;

            else if (agvsInnerAlarm == ALARMS.EQ_LOAD_REQUEST_ON_BUT_HAS_CARGO)
                return ResultCodes.DestineEqHasCargoResultCode;
            else if (agvsInnerAlarm == ALARMS.EQ_LOAD_REQUEST_IS_NOT_ON)
                return ResultCodes.DestineEqLoadReqeustOff;
            else if (agvsInnerAlarm == ALARMS.EQ_LOAD_REQUEST_ON_BUT_POSE_NOT_DOWN)
                return ResultCodes.DestineEqMachechStateErrorResultCode;
            else if (agvsInnerAlarm == ALARMS.Destine_Eq_Status_Down)
                return ResultCodes.DestineEqDownResultCode;

            else if (agvsInnerAlarm == ALARMS.SourceRackPortNoCargo)
                return ResultCodes.EqUnloadButNoCargoResultCode;
            else if (agvsInnerAlarm == ALARMS.DestineRackPortHasCargo)
                return ResultCodes.DestineRackPortHasCargoResultCode;

            else
                return ResultCodes.OtherErrorsResultCode;
        }

        public class clsResultCodes
        {
            public byte OtherErrorsResultCode { get; set; } = 1;
            public byte ZoneIsFullResultCode { get; set; } = 2;
            public byte UnloadButCargoIDReadNotMatchedResultCode { get; set; } = 4;
            public byte UnloadButCargoIDReadFailResultCode { get; set; } = 5;
            public byte InterlockErrorResultCode { get; set; } = 64;
            public byte EqUnloadButNoCargoResultCode { get; set; } = 100;
            public byte AGVDownWhenLDULDWithCargoResultCode { get; set; } = 101;
            public byte AGVDownWhenLDWithoutCargoResultCode { get; set; } = 102;
            public byte AGVDownWhenULDWithoutCargoResultCode { get; set; } = 144;
            public byte AGVDownWhenMovingToDestineResultCode { get; set; } = 145;

            public byte DestineEqLoadReqeustOff { get; set; } = 110;
            public byte DestineEqHasCargoResultCode { get; set; } = 111;
            public byte DestineEqMachechStateErrorResultCode { get; set; } = 112;
            public byte DestineEqDownResultCode { get; set; } = 113;


            public byte SourceEqUnloadReqeustOff { get; set; } = 114;
            public byte SourceEqNotHasCargoResultCode { get; set; } = 115;
            public byte SourceEqMachechStateErrorResultCode { get; set; } = 116;
            public byte SourceEqDownResultCode { get; set; } = 117;

            public byte DestineRackPortHasCargoResultCode { get; set; } = 130;
            public byte SourceRackPortNotHasCargoResultCode { get; set; } = 131;

        }
    }

}
