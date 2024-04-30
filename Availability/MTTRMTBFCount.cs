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
        public static List<int> Mttr_data = new List<int>();
        public static List<DateTime> Mttr_date = new List<DateTime>();

        public static void MTTR_TimeCount(DateTime startTime, DateTime endTime, string AGV_Name)
        {
            Mttr_date.Clear();
            Mttr_data.Clear();
            int count = 1;
            int Mttr_DurationCount = 0;
            List<clsAlarmDto> alarms = new List<clsAlarmDto>();
            for (DateTime time = startTime; time <= endTime; time.AddDays(1))
            {
                
                using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    alarms = new List<clsAlarmDto>();
                    var _alarms = dbhelper._context.Set<clsAlarmDto>().Where(alarm => alarm.Time >= time && alarm.Time <= time.AddDays(1)
                                        && (AGV_Name == "AGV_001" ? (true) : (alarm.Equipment_Name == AGV_Name))
                    );
                    count = _alarms.Count();
                    if (count == 0) 
                    { 
                        count = 1; 
                    }
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
                    Mttr_data.Add(Mttr_DurationCount / count);
                    Mttr_date.Add(time);
                    time = time.AddDays(1);
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
