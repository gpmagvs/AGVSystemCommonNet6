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
        NO_AVAILABLE_PARK_STATION = 19
    }

}
