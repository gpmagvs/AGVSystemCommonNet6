﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Alarm
{
    /// <summary>
    /// [Dev Note] 若有修改 Table 內容，必須更新 VERSION
    /// </summary>
    public class AlarmCodeTable
    {
        public const string VERSION = "1.3.9";
        public string FileVersion { get; set; } = "1.0.0";
        public clsAlarmCode[] Table { get; set; } = new clsAlarmCode[]
        {
            new clsAlarmCode(ALARMS.CreateCommandFail, "派車系統發生程式上的Crash造成", "CreateCommandFail", "", "", ""),
            new clsAlarmCode(ALARMS.NoActionType, "沒有選擇或是無法辨識任務命令類別", "NoActionType", "", "", ""),
            new clsAlarmCode(ALARMS.EmptySourcePort, "沒有選擇任務起點", "EmptySourcePort", "", "", ""),
            new clsAlarmCode(ALARMS.NoDefineSourcePort, "沒有定義或無法辨識任務起點點位", "NoDefineSourcePort", "", "", ""),
            new clsAlarmCode(ALARMS.EmptyDestPort, "沒有選擇任務終點", "EmptyDestPort", "", "", ""),
            new clsAlarmCode(ALARMS.NoDefineDestPort, "沒有定義或無法辨識任務終點點位", "NoDefineDestPort", "", "", ""),
            new clsAlarmCode(ALARMS.WrongTypeOfSourcePort, "起點的站點種類與任務命令類別匹配錯誤", "WrongTypeOfSourcePort", "", "", ""),
            new clsAlarmCode(ALARMS.WrongTargetOfSourcePort, "起點無法找到有效之二次定位點", "WrongTargetOfSourcePort", "", "", ""),
            new clsAlarmCode(ALARMS.WrongTypeOfDestPort, "終點的站點種類與任務命令類別匹配錯誤", "WrongTypeOfDestPort", "", "", ""),
            new clsAlarmCode(ALARMS.WrongTargetOfDestPort, "終點無法找到有效之二次定位點", "WrongTargetOfDestPort", "", "", ""),
            new clsAlarmCode(ALARMS.SaveDBFail, "任務資料儲存至資料庫錯誤", "SaveDBFail", "", "", ""),
            new clsAlarmCode(ALARMS.WrongAssignVehilceID, "起點指定的車子與任務指定的車子不同", "WrongAssignVehilceID", "", "", ""),
            new clsAlarmCode(ALARMS.WrongVehicleCSTStatus, "起點為車子但無卡匣存在", "WrongVehicleCSTStatus", "", "", ""),
            new clsAlarmCode(ALARMS.DownloadTaskDataWrong, "檢查任務資料異常", "DownloadTaskDataWrong", "", "", ""),
            new clsAlarmCode(ALARMS.NoAssignVehicle, "沒有指定或指定的車子ID不存在", "NoAssignVehicle", "", "", ""),
            new clsAlarmCode(ALARMS.DownloadTaskNoCarrierID, "任務沒有指定的CarrierID", "DownloadTaskNoCarrierID", "", "", ""),
            new clsAlarmCode(ALARMS.CannotFindCommand, "於資料庫中找不到指定欲取消的任務", "CannotFindCommand", "", "", ""),
            new clsAlarmCode(ALARMS.WrongStatus, "於錯誤的狀態下取消任務", "WrongStatus", "", "", ""),
            new clsAlarmCode(ALARMS.LicenseErr, "找不到註冊碼", "License Err", "", "", ""),
            new clsAlarmCode(ALARMS.RunTimeErr, "執行階段錯誤", "Run Time Err", "", "", ""),
            new clsAlarmCode(ALARMS.ReloadLogiciniErr, "載入主設定檔時發生錯誤", "Reload Logic.ini Err", "", "", ""),
            new clsAlarmCode(ALARMS.MissAlarmini, "找不到異常碼設定檔", "Miss Alarm.ini", "", "", ""),
            new clsAlarmCode(ALARMS.NotFoundAlarmCode, "設定檔找不到指定的異常碼", "Not Found Alarm Code", "", "", ""),
            new clsAlarmCode(ALARMS.NotFoundAlarmText, "設定檔找不到指定的異常碼描述", "Not Found Alarm Text", "", "", ""),
            new clsAlarmCode(ALARMS.TrafficBookingTaskCannotFindVehicle, "交管系統註冊路徑回傳參數找不到指定車載", "Traffic Booking Task Cannot Find Vehicle", "", "", ""),
            new clsAlarmCode(ALARMS.CannotFindChargeStationInMap, "系統在地圖上找不到充電站", "Cannot Find Charge Station In Map", "", "", ""),
            new clsAlarmCode(ALARMS.CannotFindIdlePointInMap, "系統在地圖上找不到 Idle Point", "Cannot Find Idle Point In Map", "", "", ""),
            new clsAlarmCode(ALARMS.CannotFindParkingPointInMap, "系統在地圖上找不到 Parking Point", "Cannot Find Parking Point In Map", "", "", ""),
            new clsAlarmCode(ALARMS.REGION_NOT_ENTERABLE, "區域不可進入", "REGION_NOT_ENTERABLE", "", "", ""),
            new clsAlarmCode(ALARMS.From_To_Of_Transfer_Task_Is_Incorrectly, "轉移任務的起點和終點錯誤", "From_To_Of_Transfer_Task_Is_Incorrectly", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_NO_Carge_Cannot_Transfer_Cargo_From_AGV_To_Desinte, "AGV 無貨物，無法從 AGV 轉移貨物至目標", "AGV_NO_Carge_Cannot_Transfer_Cargo_From_AGV_To_Desinte", "", "", ""),
            new clsAlarmCode(ALARMS.NONE, "無", "None", "", "", ""),
            new clsAlarmCode(ALARMS.AGVS_DISCONNECT_WITH_VMS, "AGVS 與 VMS 斷開連接", "AGVS Disconnect With VMS", "", "", ""),
            new clsAlarmCode(ALARMS.GET_ONLINE_REQ_BUT_AGV_DISCONNECT, "收到AGV上線請求但AGV已斷線", "Get Online Req But AGV Disconnect", "", "", ""),
            new clsAlarmCode(ALARMS.GET_OFFLINE_REQ_BUT_AGV_DISCONNECT, "收到AGV下線請求但AGV已斷線", "Get Offline Req But AGV Disconnect", "", "", ""),
            new clsAlarmCode(ALARMS.GET_ONLINE_REQ_BUT_AGV_STATE_ERROR, "收到AGV上線請求但AGV狀態為DOWN", "Get Online Req But AGV State Error", "", "", ""),
            new clsAlarmCode(ALARMS.TRAFFIC_BLOCKED_NO_PATH_FOR_NAVIGATOR, "交通阻塞沒有路線可供AGV行走", "Traffic Blocked No Path For Navigator", "", "", ""),
            new clsAlarmCode(ALARMS.NO_AVAILABLE_CHARGE_PILE, "沒有可用的充電樁", "No Available Charge Pile", "", "", ""),
            new clsAlarmCode(ALARMS.TRAFFIC_CONTROL_CENTER_EXCEPTION_WHEN_CHECK_NAVIGATOR_PATH, "檢查導航路徑時交通控制中心異常", "Traffic Control Center Exception When Check Navigator Path", "", "", ""),
            new clsAlarmCode(ALARMS.GET_CHARGE_TASK_BUT_AGV_CHARGING_ALREADY, "收到充電任務但 AGV 已在充電", "Get Charge Task But AGV Charging Already", "", "", ""),
            new clsAlarmCode(ALARMS.CST_STATUS_CHECK_FAIL, "CST狀態檢查失敗", "CST Status Check Fail", "", "", ""),
            new clsAlarmCode(ALARMS.Endpoint_EQ_NOT_CONNECTED, "與終端設備的連線尚未建立", "Endpoint EQ Not Connected", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_LOAD_REQUEST_IS_NOT_ON, "設備沒有入料請求", "EQ Load Request Is Not On", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_UNLOAD_REQUEST_IS_NOT_ON, "設備沒有出料請求", "EQ Unload Request Is Not On", "", "", ""),
            new clsAlarmCode(ALARMS.TRANSFER_TASK_TO_VMS_BUT_ERROR_OCCUR, "轉移任務到 VMS 時發生錯誤", "Transfer Task To VMS But Error Occur", "", "", ""),
            new clsAlarmCode(ALARMS.DESTIN_TAG_IS_INVLID_FORMAT, "目的地TAG設置格式錯誤", "Destin Tag Is Invalid Format", "", "", ""),
            new clsAlarmCode(ALARMS.STATION_TYPE_CANNOT_USE_AUTO_SEARCH, "無法自動搜索出非充電站或停車點的目的地", "Station Type Cannot Use Auto Search", "", "", ""),
            new clsAlarmCode(ALARMS.NO_AVAILABLE_PARK_STATION, "沒有可用的停車點", "No Available Park Station", "", "", ""),
            new clsAlarmCode(ALARMS.ERROR_WHEN_CREATE_NAVIGATION_JOBS_LINK, "創建導航任務鏈接時出錯", "Error When Create Navigation Jobs Link", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_UNLOAD_REQUEST_ON_BUT_TAG_NOT_EXIST_ON_MAP, "設備出料請求但所設定的TAG在圖資中不存在", "EQ Unload Request On But Tag Not Exist On Map", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_UNLOAD_REQUEST_ON_BUT_STATION_TYPE_SETTING_IS_NOT_EQ, "設備發起取貨請求但在圖資中被非設為設備", "EQ Unload Request On But Station Type Setting Is Not EQ", "", "", ""),
            new clsAlarmCode(ALARMS.ERROR_WHEN_FIND_TAG_OF_STATION, "查找站點TAG時發生錯誤", "Error When Find Tag Of Station", "", "", ""),
            new clsAlarmCode(ALARMS.MAP_POINT_NO_TARGETS, "站點沒有可行走的下一點", "Map Point No Targets", "", "", ""),
            new clsAlarmCode(ALARMS.ERROR_WHEN_AGV_STATUS_WRITE_TO_DB, "將AGV狀態寫入資料庫時發生錯誤", "Error When AGV Status Write To DB", "", "", ""),
            new clsAlarmCode(ALARMS.TRAFFIC_ABORT, "交通中止", "Traffic Abort", "", "", ""),
            new clsAlarmCode(ALARMS.GET_ONLINE_REQ_BUT_AGV_LOCATION_IS_TOO_FAR_FROM_POINT, "收到AGV上線請求但AGV位置距離過遠", "Get Online Req But AGV Location Is Too Far From Point", "", "", ""),
            new clsAlarmCode(ALARMS.GET_ONLINE_REQ_BUT_AGV_LOCATION_IS_NOT_EXIST_ON_MAP, "收到AGV上線請求但AGV所在位置不存在於當前地圖", "Get Online Req But AGV Location Is Not Exist On Map", "", "", ""),
            new clsAlarmCode(ALARMS.GET_ONLINE_REQ_BUT_AGV_IS_NOT_REGISTED, "收到AGV上線請求但該AGV尚未被派車系統綁定", "Get Online Req But AGV Is Not Registered", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_MOVE_TASK_IN_WORKSTATION, "當AGV位於工作站時無法指派移動任務", "Cannot Dispatch Move Task In Workstation", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_LOAD_TASK_TO_NOT_EQ_STATION, "站點非設備將無法指派放貨任務", "Cannot Dispatch Load Task To Not EQ Station", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_LOAD_TASK_WHEN_AGV_NO_CARGO, "當AGV沒有貨物時無法指派放貨任務", "Cannot Dispatch Load Task When AGV No Cargo", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_UNLOAD_TASK_WHEN_AGV_HAS_CARGO, "當AGV有貨物時無法指派取貨任務", "Cannot Dispatch Unload Task When AGV Has Cargo", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_UNLOAD_TASK_TO_NOT_EQ_STATION, "站點非設備將無法指派取貨任務", "Cannot Dispatch Unload Task To Not EQ Station", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_CHARGE_TASK_TO_NOT_CHARGABLE_STATION, "站點非充電站將無法指派充電任務", "Cannot Dispatch Charge Task To Not Chargeable Station", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_ORDER_BY_AGV_BAT_STATUS_CHECK, "因AGV電量異常無法指派任務", "Cannot Dispatch Order By AGV BAT Status Check", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_Task_Feedback_But_Task_Name_Not_Match, "AGV回報的任務ID與執行中任務ID不匹配", "AGV Task Feedback But Task Name Not Match", "", "", ""),
            new clsAlarmCode(ALARMS.SYSTEM_ERROR, "系統錯誤", "System Error", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_INTO_WORKSTATION_TASK_WHEN_AGV_HAS_CARGO, "當 AGV 有貨物時無法調度進入工作站的任務", "Cannot Dispatch Into Workstation Task When AGV Has Cargo", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_NORMAL_MOVE_TASK_WHEN_DESTINE_IS_WORKSTATION, "當目的地是工作站時無法調度正常移動任務", "Cannot Dispatch Normal Move Task When Destine Is Workstation", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_TAG_NOT_EXIST_IN_CURRENT_MAP, "設備標籤在當前地圖中不存在", "EQ Tag Not Exist In Current Map", "", "", ""),
            new clsAlarmCode(ALARMS.SubTask_Queue_Empty_But_Try_DownloadTask_To_AGV, "子任務隊列為空但嘗試下載任務到 AGV", "SubTask Queue Empty But Try Download Task To AGV", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_STATUS_DOWN, "AGV 狀態異常", "AGV Status Down", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_TASK_WITH_ILLEAGAL_STATUS, "無法調度非法狀態的任務", "Cannot Dispatch Task With Illegal Status", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_BATTERY_LOW_LEVEL, "AGV電量過低", "AGV Battery Low Level", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_WORKSTATION_DATA_NOT_SETTING, "AGV工作站數據未設置", "AGV Workstation Data Not Setting", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_AT_UNKNON_TAG_LOCATION, "AGV位於位知的TAG位置", "AGV At Unknown Tag Location", "", "", ""),
            new clsAlarmCode(ALARMS.Task_Status_Cant_Save_To_Database, "無法將任務狀態變更結果儲存至資料庫", "Task Status Can't Save To Database", "", "", ""),
            new clsAlarmCode(ALARMS.Task_Cancel_Fail, "任務取消失敗", "Task Cancel Fail", "", "", ""),
            new clsAlarmCode(ALARMS.Task_Add_To_Database_Fail, "將任務新增到資料庫失敗", "Task Add To Database Fail", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_TCPIP_DISCONNECT, "AGV TCP/IP 斷開連接", "AGV TCP/IP Disconnect", "", "", ""),
            new clsAlarmCode(ALARMS.Save_Measure_Data_to_DB_Fail, "儲存測量數據至資料庫失敗", "Save Measure Data to DB Fail", "", "", ""),
            new clsAlarmCode(ALARMS.Region_Has_No_Agv_To_Dispatch_Task, "區域內沒有AGV可被指派任務", "Region Has No AGV To Dispatch Task", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_CARRY_TASK_WHEN_AGV_HAS_CARGO, "當AGV車上有貨物時無法指派搬運任務", "Cannot Dispatch Carry Task When AGV Has Cargo", "", "", ""),
            new clsAlarmCode(ALARMS.CANNOT_DISPATCH_LOCAL_AUTO_TRANSFER_TASK, "無法調度本地自動轉移任務", "Cannot Dispatch Local Auto Transfer Task", "", "", ""),
            new clsAlarmCode(ALARMS.Source_Eq_Unload_Request_Off, "來源設備出料請求已關閉", "Source EQ Unload Request Off", "", "", ""),
            new clsAlarmCode(ALARMS.Destine_Eq_Load_Request_Off, "目的地設備入料請求已關閉", "Destine EQ Load Request Off", "", "", ""),
            new clsAlarmCode(ALARMS.Source_Eq_Status_Down, "來源設備狀態為當機", "Source EQ Status Down", "", "", ""),
            new clsAlarmCode(ALARMS.Destine_Eq_Status_Down, "目的地設備狀態為當機", "Destine EQ Status Down", "", "", ""),
            new clsAlarmCode(ALARMS.Cannot_Auto_Parking_When_AGV_Has_Cargo, "當AGV有貨物時無法自動停車", "Cannot Auto Parking When AGV Has Cargo", "", "", ""),
            new clsAlarmCode(ALARMS.Destine_Eq_Already_Has_Task_To_Excute, "目的地設備已經有任務執行", "Destine EQ Already Has Task To Execute", "", "", ""),
            new clsAlarmCode(ALARMS.Destine_EQ_Has_AGV, "目的地設備已有AGV停駐", "Destine EQ Has AGV", "", "", ""),
            new clsAlarmCode(ALARMS.Destine_EQ_Has_Registed, "目的地設備已被註冊", "Destine EQ Has Registered", "", "", ""),
            new clsAlarmCode(ALARMS.UNLOAD_BUT_AGV_NO_CARGO_MOUNTED, "AGV已取貨但在席檢知無貨物", "Unload But AGV No Cargo Mounted", "", "", ""),
            new clsAlarmCode(ALARMS.UNLOAD_BUT_CARGO_ID_EMPTY, "AGV已取貨但在席檢知無貨物", "Unload But Cargo ID Empty", "", "", ""),
            new clsAlarmCode(ALARMS.UNLOAD_BUT_CARGO_ID_NOT_MATCHED, "AGV已取貨但回報之貨物ID與任務指派的ID不匹配", "Unload But Cargo ID Not Matched", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_DATA_ILLEAGAL, "任務下載數據非法", "Task Download Data Illegal", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_TaskFeedback_ERROR, "AGV 任務回報錯誤", "AGV TaskFeedback Error", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_Disconnect, "設備斷開連接", "EQ Disconnect", "", "", ""),
            new clsAlarmCode(ALARMS.ERROR_WHEN_TASK_STATUS_CHAGE_DB, "任務狀態變更修改資料庫時發生錯誤", "Error When Task Status Change DB", "", "", ""),
            new clsAlarmCode(ALARMS.Destine_Normal_Station_Has_Task_To_Reach, "目的地點位已指派AGV前往", "Destine Normal Station Has Task To Reach", "", "", ""),
            new clsAlarmCode(ALARMS.Destine_Eq_Station_Has_Task_To_Park, "目的地設備已指派AGV前往停車", "Destine EQ Station Has Task To Park", "", "", ""),
            new clsAlarmCode(ALARMS.Destine_Charge_Station_Has_AGV, "目的地充電站有 AGV", "Destine Charge Station Has AGV", "", "", ""),
            new clsAlarmCode(ALARMS.Download_Task_To_AGV_Fail, "下載任務到 AGV 失敗", "Download Task To AGV Fail", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_LOAD_REQUEST_ON_BUT_HAS_CARGO, "設備入料請求但設備有貨物", "EQ Load Request On But Has Cargo", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_UNLOAD_REQUEST_ON_BUT_NO_CARGO, "設備出料請求但設備沒有貨物", "EQ Unload Request On But No Cargo", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_UNLOAD_REQUEST_ON_BUT_POSE_NOT_UP, "設備出料請求但撈爪非位於上點位", "EQ Unload Request On But Pose Not Up", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_LOAD_REQUEST_ON_BUT_POSE_NOT_DOWN, "設備入料請求但撈爪非位於下點位", "EQ Load Request On But Pose Not Down", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_UNLOAD_REQ_BUT_RACK_FULL_OR_EMPTY_IS_UNKNOWN, "設備出料請求但框為滿框或空框狀態未知", "EQ Unload Req But Rack Full Or Empty Is Unknown", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_LOAD_REQ_BUT_RACK_FULL_OR_EMPTY_IS_UNKNOWN, "設備入料請求但框為滿框或空框狀態未知", "EQ Load Req But Rack Full Or Empty Is Unknown", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_Type_Is_Not_Allow_To_Execute_Task_At_Source_Equipment, "來源設備不允許該AGV車款執行任務", "AGV Type Is Not Allow To Execute Task At Source Equipment", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_Type_Is_Not_Allow_To_Execute_Task_At_Destine_Equipment, "目的地設備不允許該AGV車款執行任務", "AGV Type Is Not Allow To Execute Task At Destine Equipment", "", "", ""),
            new clsAlarmCode(ALARMS.EQ_Input_Data_Not_Enough, "設備狀態訊號INPUT輸入長度不足", "EQ Input Data Not Enough", "", "", ""),
            new clsAlarmCode(ALARMS.AGV_Already_Has_Charge_Task, "AGV已經有充電任務", "AGV Already Has Charge Task", "", "", ""),
            new clsAlarmCode(ALARMS.Charge_Station_Already_Has_Task_Assigned, "充電站已經有任務分配", "Charge Station Already Has Task Assigned", "", "", ""),
            new clsAlarmCode(ALARMS.Charge_Station_Already_Has_AGV_Parked, "充電站已經有AGV停駐", "Charge Station Already Has AGV Parked", "", "", ""),
            new clsAlarmCode(ALARMS.Station_Disabled, "站點已禁用", "Station Disabled", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_AGV_STATUS_DOWN, "下載任務到 AGV 失敗-AGV狀態Down", "TASK DOWNLOAD TO AGV FAIL AGV STATUS DOWN", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL, "下載任務到 AGV 失敗", "TASK DOWNLOAD TO AGV FAIL", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_AGV_NOT_ON_TAG, "下載任務到 AGV 失敗-AGV當前位置不在TAG上", "TASK DOWNLOAD TO AGV FAIL AGV NOT ON TAG", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_WORKSTATION_NOT_SETTING_YET, "下載任務到 AGV 失敗-工作站尚未設置", "TASK DOWNLOAD TO AGV FAIL WORKSTATION NOT SETTING YET", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_AGV_BATTERY_LOW_LEVEL, "下載任務到 AGV 失敗-AGV電量過低", "TASK DOWNLOAD TO AGV FAIL AGV BATTERY LOW LEVEL", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_AGV_CANNOT_GO_TO_WORKSTATION_WITH_NORMAL_MOVE_ACTION, "下載任務到 AGV 失敗-不可使用一般走行命令AGV移動至工作站", "TASK DOWNLOAD TO AGV FAIL AGV CANNOT GO TO WORKSTATION WITH NORMAL MOVE ACTION", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_TASK_DOWNLOAD_DATA_ILLEAGAL, "下載任務到 AGV 失敗-任務數據包含不合法的內容", "TASK DOWNLOAD TO AGV FAIL TASK DOWNLOAD DATA ILLEGAL", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_SYSTEM_EXCEPTION, "下載任務到 AGV 失敗-系統異常", "TASK DOWNLOAD TO AGV FAIL SYSTEM EXCEPTION", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_NO_PATH_FOR_NAVIGATION, "下載任務到 AGV 失敗-無路徑可供AGV行走", "TASK DOWNLOAD TO AGV FAIL NO PATH FOR NAVIGATION", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_TASK_CANCEL, "下載任務到 AGV 失敗-任務已取消", "TASK DOWNLOAD TO AGV FAIL TASK CANCEL", "", "", ""),
            new clsAlarmCode(ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_TASK_DOWN_LOAD_TIMEOUT, "下載任務到 AGV 失敗-任務下載超時", "TASK DOWNLOAD TO AGV FAIL TASK DOWN LOAD TIMEOUT", "", "", ""),
            new clsAlarmCode(ALARMS.Task_Canceled, "任務已取消", "Task Canceled", "", "", ""),
            new clsAlarmCode(ALARMS.CANT_AUTO_SEARCH_STATION_TYPE_IS_NOT_EXCHANGE_FOR_INSPECTION_AGV, "無法為巡檢AGV搜尋充電站", "CANT AUTO SEARCH STATION TYPE IS NOT EXCHANGE FOR INSPECTION AGV", "", "", ""),
            new clsAlarmCode(ALARMS.NO_AVAILABLE_BAT_EXCHANGER_USABLE, "沒有可用的電池交換站", "NO AVAILABLE BAT EXCHANGER USABLE", "", "", ""),
            new clsAlarmCode(ALARMS.REGIST_REGIONS_TO_PARTS_SYSTEM_FAIL, "將區域註冊到 PARTS 系統失敗", "REGIST REGIONS TO PARTS SYSTEM FAIL", "", "", ""),
            new clsAlarmCode(ALARMS.SYSTEM_EQP_MANAGEMENT_INITIALIZE_FAIL_WITH_EXCEPTION, "設備管理模組初始化失敗並出現異常", "SYSTEM EQP MANAGEMENT INITIALIZE FAIL WITH EXCEPTION", "", "", ""),
            new clsAlarmCode(ALARMS.INVALID_CHARGE_STATION, "無效的充電站", "INVALID CHARGE STATION", "", "", ""),
            new clsAlarmCode(ALARMS.Battery_Not_Connect, "電池未連接", "Battery Not Connect", "", "", ""),
            new clsAlarmCode(ALARMS.Rack_Port_Sensor_Flash, "料架 Sensor 閃爍", "Rack Port Sensor Flash", "", "", ""),
            new clsAlarmCode(ALARMS.Transfer_Tags_Not_Found, "未找到轉換站TAG", "Transfer Tags Not Found", "", "", ""),
            new clsAlarmCode(ALARMS.No_Transfer_Station_To_Work, "無轉換站可供使用", "No Transfer Station To Work", "", "", ""),
            new clsAlarmCode(ALARMS.Task_Aborted, "任務中止", "Task Aborted", "", "", ""),
            new clsAlarmCode(ALARMS.VEHICLES_TRAJECTORY_CONFLIC, "車輛軌跡衝突", "Vehicles Trajectory Conflict", "", "", ""),
            new clsAlarmCode(ALARMS.VMSDisconnectwithVehicle, "AGV與VMS斷線", "AGV Disconnect", "", "", ""),
            new clsAlarmCode(ALARMS.Charge_Station_Status_EMO, "充電站EMO", "Charger EMO", "", "", ""),
            new clsAlarmCode(ALARMS.Charge_Station_Status_Air_Error, "充電站氣壓異常", "Charger Air Pressure Abnormal", "", "", ""),
            new clsAlarmCode(ALARMS.Charge_Station_Status_Smoke_Detected, "充電站偵煙異常", "Charger Smoke Detected", "", "", ""),
            new clsAlarmCode(ALARMS.Charge_Station_Status_IO_Module_Disconnected, "充電站IO模組連線異常", "Charger IO Moduble Connection Error", "", "", ""),
            new clsAlarmCode(ALARMS.TrafficAbort, "交管異常停止", "Task Failure Because Traffic Error", "", "", ""),
            new clsAlarmCode(ALARMS.Navigation_Path_Contain_Forbidden_Point, "導航路徑包含被設為禁止通行或抵達的點位", "Navigation Path Contain Forbidden Point", "", "", ""),
            new clsAlarmCode(ALARMS.Charge_Station_Status_Temperature_High, "充電樁溫度過高", "Charge Station Temperature Too High", "", "", ""),
            new clsAlarmCode(ALARMS.UNLOAD_BUT_CARGO_ID_READ_FAIL, "AGV取貨完成後貨物ID讀取失敗", "Cargo ID Read Fail When Unload", "", "", ""),
            new clsAlarmCode(ALARMS.AGVCargoStatusNotMatchWithTask, "AGV貨物狀態與目前任務不符", "AGV Cargo Status Not Match With Current Task", "", "", ""),
            new clsAlarmCode(ALARMS.TrafficDriveVehicleAwayButCannotFindAvoidPosition, "交管請求車輛避讓但找不到避讓點", "No available avoid point for traffic control request", "", "", ""),
            new clsAlarmCode(ALARMS.TrafficDriveVehicleAwaybutWaitOtherVehicleReleasePointTimeout, "等待其他車輛釋放取放貨Port位逾時", "Wait Other Vehicle Release Port TIMOUT", "", "", ""),
            new clsAlarmCode(ALARMS.TrafficDriveVehicleAwaybutAppendOrderToDatabaseFail, "產生避讓任務時發生寫入資料庫異常", "Add Avoid Task To Database ERROR", "", "", ""),
            new clsAlarmCode(ALARMS.TrafficDriveVehicleAwaybutVehicleNotOnline, "交管請求車輛釋放取放貨Port位但該車未上線", "Cannot Release Port When Vehicle Not ONLINE", "", "", ""),
            new clsAlarmCode(ALARMS.TrafficDriveVehicleAwaybutVehicleCannotLeaveNow, "交管請求車輛釋放取放貨Port位但該車已無法離開", "Cannot Release Port When Vehicle Down", "", "", ""),
            new clsAlarmCode(ALARMS.TrafficDriveVehicleAwayTaskCanceledByManualWhenWaitingVehicleLeave, "等待車輛釋放取放貨Port的過程中被手動取消任務", "Task Canceled By Manual When Leave Vehicles Release Ports", "", "", ""),
            new clsAlarmCode(ALARMS.SourceRackPortNoCargo, "來源Rack儲位沒有貨物", "Source Rack Port Not Has Cargo", "", "", ""),
            new clsAlarmCode(ALARMS.DestineRackPortHasCargo, "目標Rack儲位有貨物", "Destine Rack Port Has Cargo", "", "", ""),
            new clsAlarmCode(ALARMS.HostCommunicationError, "Host連線異常", "Host Communication Error", "", "", ""),
            new clsAlarmCode(ALARMS.VMSOrderActionStatusReportToAGVSFail, "VMS向AGVS上報搬運任務狀態失敗", "VMS Report Order Status Fail", "", "", ""),
            new clsAlarmCode(ALARMS.VMSOrderActionStatusReportToAGVSTimeout, "VMS向AGVS上報搬運任務狀態Timeout", "VMS Report Order Status Timeout", "", "", ""),
            new clsAlarmCode(ALARMS.VMSOrderActionStatusReportToAGVSButAGVSGetException, "VMS向AGVS上報搬運任務狀態但AGVS發生錯誤例外", "VMS Report Order Status But AGVS Has Exception", "", "", ""),
            new clsAlarmCode(ALARMS.SecsPlatformNotRun, "GPM SECS Platform 運轉異常", "GPM SECS Platform Is Not Running", "", "", ""),
        };
    }
}
