using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;

namespace AGVSystemCommonNet6.ViewModels
{
    public class DataQueryCondition
    {
        public int CurrentPage { get; set; } = 0;
        public int DataNumberPerPage { get; set; } = 20;
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public string AGVName { get; set; } = "";
        public string TaskName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Source { get; set; } = "";
        public string Destine { get; set; }= "";

    }


    public class TaskQueryCondition : DataQueryCondition
    {
        public TASK_RUN_STATUS TaskResult { get; set; } = TASK_RUN_STATUS.UNKNOWN;
        public ACTION_TYPE ActionType { get; set; } = ACTION_TYPE.Unknown;

    }

    public class AlarmQueryCondition : DataQueryCondition
    {
        public enum ALARM_TYPE_QUERY
        {
            ALL,
            ALARM,
            WARNING
        }

        public ALARM_TYPE_QUERY AlarmType { get; set; } = ALARM_TYPE_QUERY.ALL;
    }
}
