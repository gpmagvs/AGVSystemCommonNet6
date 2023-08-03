using Newtonsoft.Json;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
    public enum REMOTE_MODE : int
    {
        OFFLINE = 0, ONLINE = 1
    }

    public enum RESET_MODE : int
    {
        CYCLE_STOP,
        ABORT
    }

    public enum RETURN_CODE : int
    {
        OK = 0,
        NG = 1,
        System_Error = 404,
        Connection_Fail = 405,
        No_Response = 406
    }

    public enum TASK_RUN_STATUS : int
    {
        NO_MISSION = 0,
        NAVIGATING = 1,
        REACH_POINT_OF_TRAJECTORY = 2,
        ACTION_START = 3,
        ACTION_FINISH = 4,
        WAIT,
        FAILURE
    }

    public enum ACTION_TYPE
    {
        None,
        Unload,
        LoadAndPark,
        Forward,
        Backward,
        FaB,
        Measure,
        Load,
        Charge,
        Carry,
        Discharge,
        Escape,
        Park,
        Unpark,
        ExchangeBattery,
        Hold,
        Break,

    }
    public enum STATION_TYPE : int
    {
        Normal = 0,
        EQ = 1,
        STK = 2,
        Charge = 3,
        Buffer = 4,
        Charge_Buffer = 5,
        Charge_STK = 6,
        Escape = 8,
        EQ_LD = 11,
        STK_LD = 12,
        EQ_ULD = 21,
        STK_ULD = 22,
        Fire_Door = 31,
        Fire_EQ = 32,
        Auto_Door = 33,
        Elevator = 100,
        Elevator_LD = 201,
        TrayEQ= 201,
        TrayEQ_LD = 211,
        TrayEQ_ULD = 221,
        Unknown = 9999
    }
    public abstract class MessageBase
    {
        public string SID { get; set; }
        public string EQName { get; set; }

        [JsonProperty("System Bytes")]
        public int SystemBytes { get; set; }
        public string OriJsonString;

        public Dictionary<string, MessageHeader> Header { get; set; }

    }

}
