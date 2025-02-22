﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AGVSystemCommonNet6
{
    public class clsEnums
    {
        public enum AGV_TYPE
        {
            FORK = 0,
            YUNTECH_FORK_AGV = 1,
            INSPECTION_AGV = 2,
            SUBMERGED_SHIELD = 3,
            SUBMERGED_SHIELD_Parts = 4,
            Any = 999,
            Null = -1
        }
        public enum OPERATOR_MODE
        {
            MANUAL,
            AUTO,
        }

        public enum SUB_STATUS
        {
            IDLE = 1,
            RUN = 2,
            DOWN = 3,
            Charging = 4,
            Initializing = 5,
            ALARM = 6,
            WARNING = 7,
            STOP,
            UNKNOWN
        }

        public enum ONLINE_STATE
        {
            OFFLINE,
            ONLINE,
            UNKNOWN,
        }

        public enum MAIN_STATUS
        {
            IDLE = 1,
            RUN,
            DOWN,
            Charging,
            Unknown
        }

        public enum VMS_GROUP
        {
            GPM_FORK,
            GPM_SUBMARINE_SHIELD,
            YUNTECH_FORK,
            GPM_INSPECTION_AGV
        }
    }
}
