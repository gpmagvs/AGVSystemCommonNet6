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
        NONE = 0,
        AGVS_DISCONNECT_WITH_VMS = 1003,
        GET_ONLINE_REQ_BUT_AGV_DISCONNECT = 1004,
        GET_OFFLINE_REQ_BUT_AGV_DISCONNECT = 1005,
        GET_ONLINE_REQ_BUT_AGV_STATE_ERROR = 1006,
        TRAFFIC_BLOCKED_NO_PATH_FOR_NAVIGATOR = 1007,
        NO_AVAILABLE_CHARGE_PILE = 1008,
        TRAFFIC_CONTROL_CENTER_EXCEPTION_WHEN_CHECK_NAVIGATOR_PATH = 1009,
        GET_CHARGE_TASK_BUT_AGV_CHARGING_ALREADY = 1010,
        CST_STATUS_CHECK_FAIL = 1011,
        Endpoint_EQ_NOT_CONNECTED = 1012,
        EQ_LOAD_REQUEST_IS_NOT_ON = 1014,
        EQ_UNLOAD_REQUEST_IS_NOT_ON = 1015,
        TRANSFER_TASK_TO_VMS_BUT_ERROR_OCCUR = 1016,
        DESTIN_TAG_IS_INVLID_FORMAT = 1017,
        STATION_TYPE_CANNOT_USE_AUTO_SEARCH = 1018,
        NO_AVAILABLE_PARK_STATION = 1019,
        ERROR_WHEN_CREATE_NAVIGATION_JOBS_LINK = 1020,
        EQ_UNLOAD_REQUEST_ON_BUT_TAG_NOT_EXIST_ON_MAP = 1021,
        EQ_UNLOAD_REQUEST_ON_BUT_STATION_TYPE_SETTING_IS_NOT_EQ = 1022,
        ERROR_WHEN_FIND_TAG_OF_STATION = 1023,
        MAP_POINT_NO_TARGETS = 1024,
        ERROR_WHEN_AGV_STATUS_WRITE_TO_DB = 1025,
        TRAFFIC_ABORT = 1026,
        GET_ONLINE_REQ_BUT_AGV_LOCATION_IS_TOO_FAR_FROM_POINT = 1027,
        GET_ONLINE_REQ_BUT_AGV_LOCATION_IS_NOT_EXIST_ON_MAP = 1028,
        GET_ONLINE_REQ_BUT_AGV_IS_NOT_REGISTED = 3029,
        CANNOT_DISPATCH_MOVE_TASK_IN_WORKSTATION = 3030,
        CANNOT_DISPATCH_LOAD_TASK_TO_NOT_EQ_STATION = 3031,
        CANNOT_DISPATCH_LOAD_TASK_WHEN_AGV_NO_CARGO = 3032,
        CANNOT_DISPATCH_UNLOAD_TASK_WHEN_AGV_HAS_CARGO = 3033,
        CANNOT_DISPATCH_UNLOAD_TASK_TO_NOT_EQ_STATION = 3034,
        CANNOT_DISPATCH_CHARGE_TASK_TO_NOT_CHARGABLE_STATION = 3035,
        CANNOT_DISPATCH_ORDER_BY_AGV_BAT_STATUS_CHECK = 3036,
        AGV_Task_Feedback_But_Task_Name_Not_Match = 1036,
        SYSTEM_ERROR = 1037,
        CANNOT_DISPATCH_INTO_WORKSTATION_TASK_WHEN_AGV_HAS_CARGO = 1038,
        CANNOT_DISPATCH_NORMAL_MOVE_TASK_WHEN_DESTINE_IS_WORKSTATION = 1039,
        EQ_TAG_NOT_EXIST_IN_CURRENT_MAP = 1040,
        SubTask_Queue_Empty_But_Try_DownloadTask_To_AGV = 1041,
        AGV_STATUS_DOWN = 1042,
        CANNOT_DISPATCH_TASK_WITH_ILLEAGAL_STATUS = 1043,
        AGV_BATTERY_LOW_LEVEL = 1044,
        AGV_WORKSTATION_DATA_NOT_SETTING = 1045,
        AGV_AT_UNKNON_TAG_LOCATION = 1046,
        Task_Status_Cant_Save_To_Database = 1047,
        Task_Cancel_Fail = 1048,
        Task_Add_To_Database_Fail = 1049,
        AGV_TCPIP_DISCONNECT = 1050,
        Save_Measure_Data_to_DB_Fail = 1051,
        Region_Has_No_Agv_To_Dispatch_Task = 1052,
        CANNOT_DISPATCH_CARRY_TASK_WHEN_AGV_HAS_CARGO = 1053,
        CANNOT_DISPATCH_LOCAL_AUTO_TRANSFER_TASK = 1054,
        Source_Eq_Unload_Request_Off = 1055,
        Destine_Eq_Load_Request_Off = 1056,
        Source_Eq_Status_Down = 1057,
        Destine_Eq_Status_Down = 1058,
        Cannot_Auto_Parking_When_AGV_Has_Cargo = 1059,
        Destine_Eq_Already_Has_Task_To_Excute = 1060,
        Destine_EQ_Has_AGV = 1061,
        Destine_EQ_Has_Registed = 1062,
        UNLOAD_BUT_AGV_NO_CARGO_MOUNTED = 1063,
        UNLOAD_BUT_CARGO_ID_EMPTY = 1064,
        UNLOAD_BUT_CARGO_ID_NOT_MATCHED = 1065,
        TASK_DOWNLOAD_DATA_ILLEAGAL = 1066,
        AGV_TaskFeedback_ERROR = 1067,
        EQ_Disconnect = 1068,
        ERROR_WHEN_TASK_STATUS_CHAGE_DB = 1069,
        Destine_Normal_Station_Has_Task_To_Reach = 1070,
        Destine_Eq_Station_Has_Task_To_Park = 1071,
        Destine_Charge_Station_Has_AGV = 1072,
        Download_Task_To_AGV_Fail = 1074,
        EQ_LOAD_REQUEST_ON_BUT_HAS_CARGO = 1075,
        EQ_UNLOAD_REQUEST_ON_BUT_NO_CARGO = 1076,
        EQ_UNLOAD_REQUEST_ON_BUT_POSE_NOT_UP = 10761,
        EQ_LOAD_REQUEST_ON_BUT_POSE_NOT_DOWN = 10762,
        EQ_UNLOAD_REQ_BUT_RACK_FULL_OR_EMPTY_IS_UNKNOWN = 1077,
        EQ_LOAD_REQ_BUT_RACK_FULL_OR_EMPTY_IS_UNKNOWN = 1078,
        AGV_Type_Is_Not_Allow_To_Execute_Task_At_Source_Equipment = 1079,
        AGV_Type_Is_Not_Allow_To_Execute_Task_At_Destine_Equipment = 1080,
        EQ_Input_Data_Not_Enough = 1081,
        AGV_Already_Has_Charge_Task = 1082,
        Charge_Station_Already_Has_Task_Assigned = 1083,
        Charge_Station_Already_Has_AGV_Parked = 1084,
        Station_Disabled = 1085,
        TASK_DOWNLOAD_TO_AGV_FAIL_AGV_STATUS_DOWN = 1086,
        TASK_DOWNLOAD_TO_AGV_FAIL = 1087,
        TASK_DOWNLOAD_TO_AGV_FAIL_AGV_NOT_ON_TAG = 1088,
        TASK_DOWNLOAD_TO_AGV_FAIL_WORKSTATION_NOT_SETTING_YET = 1089,
        TASK_DOWNLOAD_TO_AGV_FAIL_AGV_BATTERY_LOW_LEVEL = 1090,
        TASK_DOWNLOAD_TO_AGV_FAIL_AGV_CANNOT_GO_TO_WORKSTATION_WITH_NORMAL_MOVE_ACTION = 1091,
        TASK_DOWNLOAD_TO_AGV_FAIL_TASK_DOWNLOAD_DATA_ILLEAGAL = 1092,
        TASK_DOWNLOAD_TO_AGV_FAIL_SYSTEM_EXCEPTION = 1093,
        TASK_DOWNLOAD_TO_AGV_FAIL_NO_PATH_FOR_NAVIGATION = 1094,
        TASK_DOWNLOAD_TO_AGV_FAIL_TASK_CANCEL = 1095,
        TASK_DOWNLOAD_TO_AGV_FAIL_TASK_DOWN_LOAD_TIMEOUT = 1096,
        Task_Canceled = 1097,
        CANT_AUTO_SEARCH_STATION_TYPE_IS_NOT_EXCHANGE_FOR_INSPECTION_AGV = 1098,
        NO_AVAILABLE_BAT_EXCHANGER_USABLE = 1099,
        REGIST_REGIONS_TO_PARTS_SYSTEM_FAIL = 1100,
        SYSTEM_EQP_MANAGEMENT_INITIALIZE_FAIL_WITH_EXCEPTION = 1101,
        INVALID_CHARGE_STATION = 1102,
        Battery_Not_Connect = 3037,
        Rack_Port_Sensor_Flash = 3038,
        Transfer_Tags_Not_Found = 3039,
        No_Transfer_Station_To_Work = 3040,
        Task_Aborted = 3041,
        VEHICLES_TRAJECTORY_CONFLIC = 3043,
        REGION_NOT_ENTERABLE = 3044,
        From_To_Of_Transfer_Task_Is_Incorrectly = 3045,
        AGV_NO_Carge_Cannot_Transfer_Cargo_From_AGV_To_Desinte = 3046,
        Path_Not_Exist_In_Route = 3047,
        Path_Conflic_But_Dispatched = 3048,
        WaitTaskBeExecutedTimeout = 3049,
        No_NG_Port_Can_Be_Used = 3050,
        OrderExecuteTimeout = 3051,
        Destine_Point_Is_Not_Allow_To_Reach = 3052,
        #region KG
        CreateCommandFail = 210,
        NoActionType = 11,
        EmptySourcePort = 12,
        NoDefineSourcePort = 13,
        EmptyDestPort = 14,
        NoDefineDestPort = 15,
        WrongTypeOfSourcePort = 16,
        WrongTargetOfSourcePort = 17,
        WrongTypeOfDestPort = 19,
        WrongTargetOfDestPort = 220,
        SaveDBFail = 221,
        WrongAssignVehilceID = 26,
        WrongVehicleCSTStatus = 28,
        DownloadTaskDataWrong = 29,
        NoAssignVehicle = 50,
        DownloadTaskNoCarrierID = 55,
        CannotFindCommand = 260,
        WrongStatus = 262,
        LicenseErr = 1,
        RunTimeErr = 2,
        ReloadLogiciniErr = 10,
        MissAlarmini = 20,
        NotFoundAlarmCode = 21,
        NotFoundAlarmText = 22,
        TrafficBookingTaskCannotFindVehicle = 60,
        CannotFindChargeStationInMap = 61,
        CannotFindIdlePointInMap = 62,
        CannotFindParkingPointInMap = 64,
        TrafficAbort = 100,
        TrafficGetTaskFail = 101,
        TrafficTaskInButTrafficStateNotIdle = 104,
        TrafficTaskInButStartorTargetPointNotExist = 105,
        TrafficTaskInButStartorTargetPointIsSegment = 106,
        TrafficTaskInButVehcleIDNotExist = 107,
        TrafficTaskInButProgramingException = 108,
        GetCSTButCSTSensorNotMatch = 120,
        PutCSTButCSTSensorNotMatch = 121,
        UnloadTaskWithWrongTaskType = 122,
        LoadTaskWithWrongTaskType = 123,
        GetChargeTaskButVehicleisWrongState = 124,
        CreateSubJobFail = 125,
        SetTrajectFail = 126,
        SetHomingTrajectFail = 127,
        VMSWillSendRechargeCommandButAGVStatusIsNotAtIdle = 130,
        VMSWillSendRechargeCommandButSystemStateIsNotAtOnline = 131,
        AGVCargoStatusNotMatchWithTask = 132,
        AGVCarriedIDNotMatchWithTask = 133,
        TaskIsNotTransferButAGVCarrierExist = 134,
        PortStatusisWrongCannottLoadUnload = 135,
        CannottFindExecuteVehicleofTaskforCheckPortStatus = 136,
        SourcePortAssignAGVbutVehicleNoCarrier = 137,
        SourcePortNotAssignAGVbutVehicleCarrierExist = 138,
        VMSDisconnectwithVehicle = 140,
        VehicleReplyMessage0302NG = 141,
        VehicleMessageT3Timeout = 142,
        CannotAssignAutoParkingJobBecauseWrongCargoStatus = 143,
        RetryCalculateMoveAngleFail = 144,
        VMSTestPingIPFailed = 150,
        ReplanErrorSystemStateisnotPausedorJobisnotMovingMission = 155,
        ReplanErrorCannotGoBackOriginalPosition = 156,
        ReplanErrorCannotFindNextPoint = 157,
        CannotAssignChargeJobBecauseWrongCargoStatus = 158,
        CannotOnlineVehicleBecauseWrongCurrentPosition = 159,
        CannotOnlineVehicleBecauseAtWrongStationType = 160,
        CannotOnlineVehicleBecauseEffectOtherAGV = 161,
        ReportCompletedTaskTimeoutByCheckPosition = 168,
        ReportCompletedTaskTimeoutByCheckJobStatus = 169,
        CannotOnlineVehicleBecauseWrongRunStatus = 170,
        CannotOnlineVehicleBecauseAtVirtualPoint = 171,
        TrafficCross = 600,
        TrafficDriveVehicleAwaybutGetTaskFail = 601,
        TrafficDriveVehicleAwaybutVehicleStatusNotIdle = 602,
        TrafficDriveVehicleAwayButCannotFindAvoidPosition = 603,
        TrafficDriveVehicleAwaybutWaitOtherVehicleReleasePointTimeout = 604,
        DownloadTaskbutActionTypeWrong = 620,
        DownloadTaskbutVehicleNotExist = 621,
        TheTaskNeedtoAssignVehicle = 622,
        DownloadTaskbutNoSourcePort = 623,
        DownloadTaskbutSourcePortNotFind = 624,
        DownloadTaskbutAssignVehicleNotMatch = 625,
        DownloadTaskbutNoDestPort = 626,
        DownloadTaskbutDestPortNotFind = 627,
        ChargeTaskbutNotChargeStation = 628,
        SourcePortNoTargetPosition = 629,
        DestPortNoTargetPosition = 630,
        VehicleNotAtChargeStation = 631,
        SourcePortTypeisWrong = 632,
        DestPortTypeisWrong = 633,
        DownloadTaskbutCstSensorWrong = 634,
        DownloadTaskbutCstIDWrong = 635,
        DownloadTaskException = 636,
        AssignSourcePortbutVehicleCargoStatueIsWrong = 640,
        DownloadTaskbutManageTrafficGetChargeorParkingStationFail = 641,
        DownloadTaskbutSourcePortCannotEqualtoManageTrafficReturnPosition = 642,
        SourcePortAssignVehiclebutVehicleCarrierNotExist = 643,
        DownloadTaskbutVehicleCstStatusWrong = 644,
        SourcePortAssignVehicleButVehicleNotReady = 645,
        SourcePortAssignVehicleButOtherTaskSourcePortIsSame = 646,
        TaskCancelByHost = 670,
        Vehicle0303JobStatusStartTimeout = 680,
        Vehicle0303JobStatusEndTimeout = 681,
        Charge_Station_EMO = 10763,
        Charge_Station_Air_Error = 10764,
        Charge_Station_Smoke_Detected = 10765,
        REPEATED_TAG_SETTING = 10766,
        #endregion
    }

}
