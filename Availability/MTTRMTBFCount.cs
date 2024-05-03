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
        public static List<DateTime> MttrMtbf_date = new List<DateTime>();
        public static List<int> Mtbf_data = new List<int>();

        public static void MTTRMTBF_TimeCount(DateTime startTime, DateTime endTime, string AGV_Name)
        {
            Mtbf_data.Clear();
            MttrMtbf_date.Clear();
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
                    List<string> MTTRMTBF_Duration_Tostring  = _alarms.Select(alarm => $"{alarm.Duration}").ToList();
                    List<string> alarmstime_Tostring = _alarms.Select(alarm => $"{alarm.Time}").ToList();
                    List<int> MTTRMTBF_DurationToint = new List<int>();
                    List<DateTime> MTTRMTBF_TimeToDateTime = new List<DateTime>();
                    List<DateTime> AlarmEndTimeList = new List<DateTime>();
                    DateTime AlarmEndTime =new DateTime();
                    TimeSpan RunUntilAlarmTime = new TimeSpan();
                    List<TimeSpan> RunUntilAlarmTimeList = new List<TimeSpan>();
                    int RunUntilAlarmTimeTotal = new int();
                    try
                    {
                        foreach (string str in MTTRMTBF_Duration_Tostring)
                        {
                            MTTRMTBF_DurationToint.Add(int.Parse(str));// 將持續時間轉換為int
                        }
                        foreach (int num in MTTRMTBF_DurationToint)
                        {
                            Mttr_DurationCount += num; //計算總異常時間
                        }
                        foreach (string alarmtime in alarmstime_Tostring)
                        {
                            MTTRMTBF_TimeToDateTime.Add(DateTime.Parse(alarmtime)); //將異常時間轉為datetime
                        }
                        int index = 0;
                        foreach (DateTime alarmendtime in MTTRMTBF_TimeToDateTime)
                        {
                            //取的異常結束時間
                            AlarmEndTime = MTTRMTBF_TimeToDateTime[index].AddSeconds(MTTRMTBF_DurationToint[index]);
                        }
                        AlarmEndTimeList.Add(AlarmEndTime);
                        for (int i = 1; i < count; i++)
                        {
                            //計算異常排除至下一筆異常時間差
                            RunUntilAlarmTime = AlarmEndTimeList[i] - AlarmEndTimeList[i - 1];
                        }
                        RunUntilAlarmTimeList.Add(RunUntilAlarmTime);
                        TimeSpan totalSpan = TimeSpan.Zero;
                        foreach (TimeSpan timeSpantotal in RunUntilAlarmTimeList)
                        {
                            //計算總時間差
                            totalSpan.Add(timeSpantotal);
                        }
                        RunUntilAlarmTimeTotal = (int)totalSpan.TotalSeconds;//將時間差統一轉成秒
                        Mttr_data.Add(Mttr_DurationCount / count);
                        MttrMtbf_date.Add(time);
                        Mtbf_data.Add(RunUntilAlarmTimeTotal / (count - 1));
                        time = time.AddDays(1);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                
            }
        }
    }
}
