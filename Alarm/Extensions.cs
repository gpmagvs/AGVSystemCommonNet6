using AGVSystemCommonNet6.AGVDispatch.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Alarm
{
    public static class Extensions
    {
        public static ALARMS ToAGVSAlarmCode(this TASK_DOWNLOAD_RETURN_CODES taskDownloadReturnCode)
        {
            switch (taskDownloadReturnCode)
            {
                case TASK_DOWNLOAD_RETURN_CODES.OK:
                    return ALARMS.NONE;
                case TASK_DOWNLOAD_RETURN_CODES.AGV_STATUS_DOWN:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_AGV_STATUS_DOWN;
                case TASK_DOWNLOAD_RETURN_CODES.AGV_NOT_ON_TAG:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_AGV_NOT_ON_TAG;
                case TASK_DOWNLOAD_RETURN_CODES.WORKSTATION_NOT_SETTING_YET:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_WORKSTATION_NOT_SETTING_YET;
                case TASK_DOWNLOAD_RETURN_CODES.AGV_BATTERY_LOW_LEVEL:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_AGV_BATTERY_LOW_LEVEL;
                case TASK_DOWNLOAD_RETURN_CODES.AGV_CANNOT_GO_TO_WORKSTATION_WITH_NORMAL_MOVE_ACTION:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_AGV_CANNOT_GO_TO_WORKSTATION_WITH_NORMAL_MOVE_ACTION;
                case TASK_DOWNLOAD_RETURN_CODES.TASK_DOWNLOAD_DATA_ILLEAGAL:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_TASK_DOWNLOAD_DATA_ILLEAGAL;
                case TASK_DOWNLOAD_RETURN_CODES.SYSTEM_EXCEPTION:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_SYSTEM_EXCEPTION;
                case TASK_DOWNLOAD_RETURN_CODES.NO_PATH_FOR_NAVIGATION:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_NO_PATH_FOR_NAVIGATION;
                case TASK_DOWNLOAD_RETURN_CODES.OK_AGV_ALREADY_THERE:
                    return ALARMS.NONE;
                case TASK_DOWNLOAD_RETURN_CODES.TASK_CANCEL:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_TASK_CANCEL;
                case TASK_DOWNLOAD_RETURN_CODES.TASK_DOWN_LOAD_TIMEOUT:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL_TASK_DOWN_LOAD_TIMEOUT;
                case TASK_DOWNLOAD_RETURN_CODES.TASK_DOWNLOAD_FAIL:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL;
                case TASK_DOWNLOAD_RETURN_CODES.Parts_System_Not_Allow_Point_Regist:
                    return ALARMS.REGIST_REGIONS_TO_PARTS_SYSTEM_FAIL;
                default:
                    return ALARMS.TASK_DOWNLOAD_TO_AGV_FAIL;
            }
        }
    }
}
