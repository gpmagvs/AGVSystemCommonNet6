using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE
{
    public static class DatabaseCaches
    {
        public struct TaskCaches
        {

            public static List<clsTaskDto> WaitExecuteTasks { get; internal set; } = new List<clsTaskDto>();
            /// <summary>
            /// 包含失敗與完成
            /// </summary>
            public static List<clsTaskDto> CompleteTasks { get; internal set; } = new List<clsTaskDto>();

            public static List<clsTaskDto> RunningTasks { get; internal set; } = new List<clsTaskDto>();

            public static List<clsTaskDto> InCompletedTasks
            {
                get
                {
                    List<clsTaskDto> clsTaskDtos = new List<clsTaskDto>();
                    clsTaskDtos.AddRange(WaitExecuteTasks);
                    clsTaskDtos.AddRange(RunningTasks);
                    return clsTaskDtos.OrderBy(st => (int)st.State).ToList();
                }
            }
        }
        public struct Alarms
        {
            public static List<clsAlarmDto> UnCheckedAlarms { get; internal set; } = new List<clsAlarmDto>();
        }

        public struct Vehicle
        {
            public static List<clsAGVStateDto> VehicleStates { get; internal set; } = new List<clsAGVStateDto>();
        }
    }
}
