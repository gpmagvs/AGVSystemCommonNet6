using System;
using System.Collections.Generic;
using System.Text;

namespace AGVSystemCommonNet6
{
    public class clsEnums
    {
        public enum AGV_TYPE
        {
            FORK,
            SUBMERGED_SHIELD,
            INSPECTION_AGV,
            UNKNOWN
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
            STOP
        }
        public enum AGV_MODEL
        {
            FORK_AGV,
            YUNTECH_FORK_AGV,
            /// <summary>
            /// 巡檢AGV
            /// </summary>
            INSPECTION_AGV,
            UNKNOWN = 999
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
