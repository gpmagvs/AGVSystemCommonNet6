using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE.BackgroundServices;
using Microsoft.Extensions.Logging;
using NLog;

namespace AGVSystemCommonNet6.DATABASE
{
    public static class DatabaseCaches
    {
        internal static ILogger<DatabaseBackgroundService> logger;
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
            public static event EventHandler<List<clsAlarmDto>> OnUnCheckedAlarmsChanged;
            public static List<clsAlarmDto> UnCheckedAlarms { get; internal set; } = new List<clsAlarmDto>();

            /// <summary>
            /// 為一標示碼 : 時間+alarmCode
            /// </summary>
            /// <param name="alarmList"></param>
            /// <returns></returns>
            public static async Task UpdateUnCheckdAlarms(List<clsAlarmDto> alarmList)
            {

                try
                {

                    Dictionary<clsAlarmDto, string> keysOfAlarmsInput = alarmList.ToDictionary(al => al, al => getKeyOfAlarm(al));
                    List<string> previousAlarmsKeyCollection = GetPreviousAlarmsKeyCollection();

                    //found new added .
                    List<clsAlarmDto> _unCheckedAlarms = keysOfAlarmsInput.Where(keyPair => !previousAlarmsKeyCollection.Contains(keyPair.Value))
                                                                          .Select(keypair => keypair.Key)
                                                                          .ToList();
                    //found removed 

                    List<clsAlarmDto> _checkedAlarms = UnCheckedAlarms.Where(al => !keysOfAlarmsInput.Values.Contains(getKeyOfAlarm(al))).ToList();

                    bool anyAlarmRemoved = _checkedAlarms.Any();
                    bool anyAlarmNewAdded = _unCheckedAlarms.Any();

                    UnCheckedAlarms = alarmList;
                    if (anyAlarmNewAdded || anyAlarmRemoved)
                    {
                        OnUnCheckedAlarmsChanged?.Invoke("", UnCheckedAlarms);
                    }

                    List<string> GetPreviousAlarmsKeyCollection()
                    {
                        return UnCheckedAlarms.Select(alarm => getKeyOfAlarm(alarm)).ToList();
                    }

                    string getKeyOfAlarm(clsAlarmDto alramDto)
                    {
                        return alramDto.Time.ToString("yyyyMMddHHmmssffff") + "_" + alramDto.AlarmCode + "_" + alramDto.Equipment_Name + "_" + alramDto.OccurLocation;
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, ex.Message);
                }
            }
        }

        public struct Vehicle
        {
            public static List<clsAGVStateDto> VehicleStates { get; internal set; } = new List<clsAGVStateDto>();
        }
    }
}
