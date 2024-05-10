using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RosSharp.RosBridgeClient.MessageTypes.Std;
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
        public static List<int> MissTagcount = new List<int>();

        public static void MTTRMTBF_TimeCount(DateTime startTime, DateTime endTime, string AGV_Name)
        {
            Mtbf_data.Clear();
            MttrMtbf_date.Clear();
            Mttr_data.Clear();

            List<clsAlarmDto> alarms = new List<clsAlarmDto>();
            for (DateTime time = startTime; time <= endTime; time.AddDays(1))
            {
                int count = 1;
                int Mttr_DurationCount = 0;
                List<string> MTTRMTBF_Duration_Tostring = new List<string>();
                List<string> alarmstime_Tostring = new List<string>();
                using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    alarms = new List<clsAlarmDto>();
                    var _alarms = dbhelper._context.Set<clsAlarmDto>().Where(alarm => alarm.Time >= time && alarm.Time <= time.AddDays(1)
                                        && (alarm.Equipment_Name == AGV_Name)
                    );
                    count = _alarms.Count();
                    if (count == 0)
                    {
                        count = 1;
                    }
                    MTTRMTBF_Duration_Tostring = _alarms.Select(alarm => $"{alarm.Duration}").ToList();
                    alarmstime_Tostring = _alarms.Select(alarm => $"{alarm.Time}").ToList();
                    List<int> MTTRMTBF_DurationToint = new List<int>();
                    List<DateTime> MTTRMTBF_TimeToDateTime = new List<DateTime>();
                    List<DateTime> AlarmEndTimeList = new List<DateTime>();
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
                        if (MTTRMTBF_TimeToDateTime.Count > 0)
                        {
                            for (int index = 0; index < count; index++)
                            {
                                //取的異常結束時間
                                //AlarmEndTime = MTTRMTBF_TimeToDateTime[index].AddSeconds(MTTRMTBF_DurationToint[index]);
                                AlarmEndTimeList.Add(MTTRMTBF_TimeToDateTime[index].AddSeconds(MTTRMTBF_DurationToint[index]));
                            }

                            for (int i = 1; i < count; i++)
                            {
                                //計算異常排除至下一筆異常時間差
                                //RunUntilAlarmTime = AlarmEndTimeList[i] - AlarmEndTimeList[i - 1];
                                RunUntilAlarmTimeList.Add(AlarmEndTimeList[i] - AlarmEndTimeList[i - 1]);
                            }

                            TimeSpan totalSpan = TimeSpan.Zero;
                            for (int i = 0; i < RunUntilAlarmTimeList.Count; i++)
                            {
                                totalSpan += RunUntilAlarmTimeList[i];
                            }
                            RunUntilAlarmTimeTotal = (int)totalSpan.TotalSeconds;//將時間差統一轉成秒
                        }
                        else
                        {
                            RunUntilAlarmTimeTotal = 0;
                        }
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
        public static void MissTagCount(DateTime startTime, DateTime endTime, string AGV_Name)
        {
            MissTagcount.Clear();
            List<clsAlarmDto> alarms = new List<clsAlarmDto>();
            for (DateTime time = startTime; time <= endTime; time.AddDays(1))
            {
                using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    alarms = new List<clsAlarmDto>();
                    var _alarms = dbhelper._context.Set<clsAlarmDto>().Where(alarm => alarm.Time >= time && alarm.Time <= time.AddDays(1)
                                        && (AGV_Name == "AGV_001" ? (true) : (alarm.Equipment_Name == AGV_Name) && alarm.AlarmCode == 23)
                    );
                    MissTagcount.Add(_alarms.Count());
                }
                time = time.AddDays(1);
            }
        }
    }
}
