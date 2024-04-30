using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Availability
{
    public class MTTRMTBFCount
    {
        public static int [] Mttr_data;
        public static void MTTR_TimeCount(DateTime startTime, DateTime endTime, string AGV_Name)
        {
            int i = 1;
            int count = 0;
            int Mttr_DurationCount =0;
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                DateTime time ;
                List<clsAlarmDto> alarms = new List<clsAlarmDto>();
                for (time = startTime; time <= endTime; time.AddDays(1))
                {
                    alarms = new List<clsAlarmDto>();
                    var _alarms = dbhelper._context.Set<clsAlarmDto>().Where(alarm => alarm.Time >= time && alarm.Time <= time.AddDays(1)
                                        && (AGV_Name == "AGV_001" ? (true) : (alarm.Equipment_Name == AGV_Name))
                    );
                    count = _alarms.Count();
                    List<string> MTTR_Time = _alarms.Select(alarm => $"{alarm.Time}").ToList();
                    List<string> MTTR_Duration = _alarms.Select(alarm => $"{alarm.Duration}").ToList();
                    List<int> MTTR = new List<int>();
                    foreach (string str in MTTR_Duration)
                    {
                        MTTR.Add(int.Parse(str));
                    }

                    foreach (int num in MTTR)
                    {
                        Mttr_DurationCount += num;
                    }
                    i++;
                    Mttr_data[i] = Mttr_DurationCount / count;
                }
            }
        }
        public static void MTBF_TimeCount(DateTime startTime, DateTime endTime, string AGV_Name)
        {
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {

            }
        }
    }
}
