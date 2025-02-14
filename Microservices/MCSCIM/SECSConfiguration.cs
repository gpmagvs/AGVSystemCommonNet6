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
    public class clsReturnCodes
    {
        public byte Control_State_Not_At_Online_Remote { get; set; } = 0x70;    // 派車非 Online/Remote 模式
        public byte Cannot_Create_Command_By_Exception { get; set; } = 0x71;         // 因異常無法創建命令
        public byte Not_Assign_Source_Port { get; set; } = 0x72;           // 未指定來源站點
        public byte Cannot_Find_Source_Por { get; set; } = 0x73;     // 找不到來源站點
        public byte Not_Assign_Destination_Port { get; set; } = 0x74;            // 未指定目標站點
        public byte Cannot_Find_Destination_Port { get; set; } = 0x75;         // 找不到目標站點
        public byte Cannot_Perform_Unload_For_Source_Port { get; set; } = 0x76;      // 無法在來源站點執行卸載
        public byte Cannot_Perform_Load_For_Destination_Port { get; set; } = 0x77;     // 無法在目標站點執行裝載
        public byte Save_Command_Data_To_Database_Fail { get; set; } = 0x78;          // 保存命令數據到數據庫失敗
        public byte Source_Port_Assign_Wrong_AGV { get; set; } = 0x79;             // 來源站點分配了錯誤的AGV
        public byte AGV_Cargo_Status_Mismatch_With_Command { get; set; } = 0x7A;           // AGV貨物狀態與命令不匹配
        public byte Wrong_Command_Data_Cause_Exception { get; set; } = 0x7B;            // 錯誤的命令數據導致異常
        public byte Signal_Is_Not_Load_Request_For_Destination_Port { get; set; } = 0x7C;    // 目標站點無裝載請求信號
        public byte Signal_Is_Not_Unload_Request_For_Source_Port { get; set; } = 0x7D;       // 來源站點無卸載請求信號
        public byte Cannot_Detect_Carrier_Exist_For_Source_Port { get; set; } = 0x7E;// 無法檢測到來源站點的載具存在
         public byte Equipment_Status_Is_Down { get; set; } = 0x80;                      // 設備狀態為停機
        public byte Detect_Carrier_Exist_For_Destination_Port { get; set; } = 0x81;            // 檢測到目標站點已有載具
        public byte Equipment_Load_Request_And_Unload_Request_Both_ON { get; set; } = 0x82; // 設備裝載和卸載請求信號同時開啟
        public byte Equipment_Load_Request_And_Unload_Request_Both_OFF { get; set; } = 0x83; // 設備裝載和卸載請求信號同時關閉
        public byte Unknown_Conditions { get; set; } = 0x84;                                // 未知條件
        public byte Cannot_Find_The_Carrier_ID_In_Rack { get; set; } = 0x85;           // 在料架中找不到指定載具ID
        public byte Cannot_Find_Seat_For_The_Carrier_In_Rack { get; set; } = 0x86;         // 在料架中找不到載具的位置
        public byte Not_Assign_Vehicle_For_Abnormal_Transfer { get; set; } = 0x87;   // 異常搬運未指定車輛
        public byte Source_Port_Assign_AGV_But_Already_Exist_Same_Command { get; set; } = 0x88;// 來源站點已分配AGV但存在相同命令
        public byte Not_Assign_Carrier_ID_For_Command { get; set; } = 0x89;          // 命令未指定載具ID
        public byte Rack_Source_Port_Already_Has_Task { get; set; } = 0x8A;        // 料架來源站點已有任務
        public byte Rack_Source_Port_Position_Is_Disable { get; set; } = 0x8B;      // 料架來源站點位置已禁用
        public byte Rack_Source_Port_Exist_Multiple_Same_Carrier_Id { get; set; } = 0x8C; // 料架來源站點存在多個相同載具ID
          public byte Rack_Destination_Port_Already_Has_Task { get; set; } = 0x8D;        // 料架目標站點已有任務
        public byte Rack_Destination_Port_Position_Is_Disable { get; set; } = 0x8F;      // 料架目標站點位置已禁用
        public byte Rack_Destination_Port_Already_Has_Data { get; set; } = 0x90;      // 料架目標站點已有數據
        public byte Source_Port_Unload_Request_Is_OFF { get; set; } = 0x91;         // 來源站點卸載請求為關閉狀態
        public byte Destination_Port_Load_Request_Is_OFF { get; set; } = 0x92;            // 目標站點裝載請求為關閉狀態
        public byte Source_Port_Is_Disconnecte { get; set; } = 0x93;                  // 來源站點已斷開連接
        public byte Destination_Port_Is_Disconnected { get; set; } = 0x94;             // 目標站點已斷開連接
        public byte Rack_Sensor_Is_Error { get; set; } = 0x95;

    }
}
