using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
    public enum REMOTE_MODE : int
    {
        OFFLINE = 0,
        ONLINE = 1,
        /// <summary>
        /// 切換中
        /// </summary>
        SWITCHING = 2
    }

    public enum RESET_MODE : int
    {
        CYCLE_STOP,
        ABORT
    }
    public enum RESET_TYPE : int
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
        No_Response = 406,
        Status_Abnormal = 407,
        TASK_DOWNLOAD_DATA_ILLEAGAL = 408,
        No_Found_Reply_In_Store = 409,
        PROCESSING = 410,
        Current_Tag_Cannot_Online = 411,
        AGV_Need_Park_Above_Tag = 412,
        Cannot_Switch_Remote_Mode_When_Task_Executing = 413,
        AGV_Not_Initialized = 414,
        Busy = 415,
        Current_Tag_Cannot_Online_In_Equipment = 416,
        Current_Tag_Cannot_Online_At_Virtual_Point = 417,
        AGV_HasIDBut_No_Cargo = 418,
    }
    public enum PROCESS_RESULT
    {
        Processed = 1,
        UnProcessed = 2,
        LessTime = 3,
        OverTime = 4
    }
    public enum TASK_DOWNLOAD_RETURN_CODES : int
    {
        OK = 0,
        /// <summary>
        /// AGV狀態DOWN
        /// </summary>
        AGV_STATUS_DOWN = 1,
        /// <summary>
        /// AGV沒有在TAG上
        /// </summary>
        AGV_NOT_ON_TAG = 2,
        /// <summary>
        /// 工作站點不存在設定值
        /// </summary>
        WORKSTATION_NOT_SETTING_YET = 3,
        /// <summary>
        /// AGV低電量
        /// </summary>
        AGV_BATTERY_LOW_LEVEL = 4,
        /// <summary>
        /// AGV無法用一般移動的方式導航至工作站點內
        /// </summary>
        AGV_CANNOT_GO_TO_WORKSTATION_WITH_NORMAL_MOVE_ACTION = 5,
        /// <summary>
        /// 任務資料不合法
        /// </summary>
        TASK_DOWNLOAD_DATA_ILLEAGAL = 6,
        /// <summary>
        /// 系統例外
        /// </summary>
        SYSTEM_EXCEPTION = 7,
        /// <summary>
        /// 無任何路線供AGV導航
        /// </summary>
        NO_PATH_FOR_NAVIGATION = 8,
        OK_AGV_ALREADY_THERE = 100,
        TASK_CANCEL = 101,
        TASK_DOWN_LOAD_TIMEOUT = 102,
        TASK_DOWNLOAD_FAIL = 103
    }

    public enum TASK_RUN_STATUS : int
    {
        NO_MISSION = 0,
        NAVIGATING = 1,
        REACH_POINT_OF_TRAJECTORY = 2,
        ACTION_START = 3,
        ACTION_FINISH = 4,
        WAIT,
        FAILURE,
        CANCEL
    }

    public enum TRANSFER_PROCESS : int
    {
        NOT_START_YET,
        GO_TO_SOURCE_EQ,
        WORKING_AT_SOURCE_EQ,
        GO_TO_DESTINE_EQ,
        WORKING_AT_DESTINE_EQ,
        LEAVE_WORKSTATION,
        MOVING,
        GO_TO_CHARGE_STATION,
        PARK_IN_CHARGE_STATION,
        FINISH
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
        Unknown,
        NoAction = 999
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
        TrayEQ = 202,
        TrayEQ_LD = 211,
        TrayEQ_ULD = 221,
        Unknown = 9999
    }
    public enum CST_TYPE
    {
        None = 0,
        Tray = 200
    }
}
