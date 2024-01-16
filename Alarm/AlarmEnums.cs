﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Alarm
{
    public enum ALARM_LEVEL
    {
        WARNING,
        ALARM,
    }
    public enum ALARM_SOURCE
    {
        AGVS, EQP
    }

    public enum ALARMS
    {
        VMS_DISCONNECT = 2,
        AGV_DISCONNECT = 3,
        GET_ONLINE_REQ_BUT_AGV_DISCONNECT = 4,
        GET_OFFLINE_REQ_BUT_AGV_DISCONNECT = 5,
        GET_ONLINE_REQ_BUT_AGV_STATE_ERROR = 6,
        TRAFFIC_BLOCKED_NO_PATH_FOR_NAVIGATOR = 7,
        NO_AVAILABLE_CHARGE_PILE = 8,
        TRAFFIC_CONTROL_CENTER_EXCEPTION_WHEN_CHECK_NAVIGATOR_PATH = 9,
        GET_CHARGE_TASK_BUT_AGV_CHARGING_ALREADY = 10,
        CST_STATUS_CHECK_FAIL = 11,
        Endpoint_EQ_NOT_CONNECTED = 12,
        NONE = 13,
        EQ_LOAD_REQUEST_IS_NOT_ON = 14,
        EQ_UNLOAD_REQUEST_IS_NOT_ON = 15,
        TRANSFER_TASK_TO_VMS_BUT_ERROR_OCCUR = 16,
        DESTIN_TAG_IS_INVLID_FORMAT = 17,
        STATION_TYPE_CANNOT_USE_AUTO_SEARCH = 18,
        NO_AVAILABLE_PARK_STATION = 19,
        ERROR_WHEN_CREATE_NAVIGATION_JOBS_LINK = 20,
        EQ_UNLOAD_REQUEST_ON_BUT_TAG_NOT_EXIST_ON_MAP = 21,
        EQ_UNLOAD_REQUEST_ON_BUT_STATION_TYPE_SETTING_IS_NOT_EQ = 22,
        ERROR_WHEN_FIND_TAG_OF_STATION = 23,
        MAP_POINT_NO_TARGETS = 24,
        ERROR_WHEN_AGV_STATUS_WRITE_TO_DB = 25,
        TRAFFIC_ABORT = 26,
        GET_ONLINE_REQ_BUT_AGV_LOCATION_IS_TOO_FAR_FROM_POINT = 27,
        GET_ONLINE_REQ_BUT_AGV_LOCATION_IS_NOT_EXIST_ON_MAP = 28,
        GET_ONLINE_REQ_BUT_AGV_IS_NOT_REGISTED = 29,
        CANNOT_DISPATCH_MOVE_TASK_IN_WORKSTATION = 30,
        CANNOT_DISPATCH_LOAD_TASK_TO_NOT_EQ_STATION = 31,
        CANNOT_DISPATCH_LOAD_TASK_WHEN_AGV_NO_CARGO = 32,
        CANNOT_DISPATCH_UNLOAD_TASK_WHEN_AGV_HAS_CARGO = 33,
        CANNOT_DISPATCH_UNLOAD_TASK_TO_NOT_EQ_STATION = 34,
        CANNOT_DISPATCH_CHARGE_TASK_TO_NOT_CHARGABLE_STATION = 35,
        AGV_Task_Feedback_But_Task_Name_Not_Match = 36,
        SYSTEM_ERROR = 37,
        CANNOT_DISPATCH_INTO_WORKSTATION_TASK_WHEN_AGV_HAS_CARGO = 38,
        CANNOT_DISPATCH_NORMAL_MOVE_TASK_WHEN_DESTINE_IS_WORKSTATION = 39,
        EQ_TAG_NOT_EXIST_IN_CURRENT_MAP = 40,
        SubTask_Queue_Empty_But_Try_DownloadTask_To_AGV = 41,
        AGV_STATUS_DOWN = 42,
        CANNOT_DISPATCH_TASK_WITH_ILLEAGAL_STATUS = 43,
        AGV_BATTERY_LOW_LEVEL = 44,
        AGV_WORKSTATION_DATA_NOT_SETTING = 45,
        AGV_AT_UNKNON_TAG_LOCATION = 46,
        Task_Status_Cant_Save_To_Database = 47,
        Task_Cancel_Fail = 48,
        Task_Add_To_Database_Fail = 49,
        AGV_TCPIP_DISCONNECT = 50,
        Save_Measure_Data_to_DB_Fail = 51,
        Region_Has_No_Agv_To_Dispatch_Task = 52,
        CANNOT_DISPATCH_CARRY_TASK_WHEN_AGV_HAS_CARGO = 53,
        CANNOT_DISPATCH_LOCAL_AUTO_TRANSFER_TASK = 54,
        Source_Eq_Unload_Request_Off = 55,
        Destine_Eq_Load_Request_Off = 56,
        Source_Eq_Status_Down = 57,
        Destine_Eq_Status_Down = 58,
        Cannot_Auto_Parking_When_AGV_Has_Cargo = 59,
        Destine_Eq_Already_Has_Task_To_Excute = 60,
        Destine_EQ_Has_AGV = 61,
        Destine_EQ_Has_Registed = 62,
        UNLOAD_BUT_AGV_NO_CARGO_MOUNTED = 63,
        UNLOAD_BUT_CARGO_ID_EMPTY = 64,
        UNLOAD_BUT_CARGO_ID_NOT_MATCHED = 65,
        TASK_DOWNLOAD_DATA_ILLEAGAL = 66,
        AGV_TaskFeedback_ERROR = 67,
        EQ_Disconnect = 68,
        ERROR_WHEN_TASK_STATUS_CHAGE_DB = 69,
        Destine_Normal_Station_Has_Task_To_Reach = 70,
        Destine_Eq_Station_Has_Task_To_Park = 71,
        Destine_Charge_Station_Has_AGV = 72,
        PING_CHECK_FAIL = 73,
        Download_Task_To_AGV_Fail = 74,
        EQ_LOAD_REQUEST_ON_BUT_HAS_CARGO = 75,
        EQ_UNLOAD_REQUEST_ON_BUT_NO_CARGO = 76,
        EQ_UNLOAD_REQ_BUT_RACK_FULL_OR_EMPTY_IS_UNKNOWN = 77,
        EQ_LOAD_REQ_BUT_RACK_FULL_OR_EMPTY_IS_UNKNOWN = 78
    }

}
