using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.HttpHelper;

namespace AGVSystemCommonNet6.Microservices
{
    public class AliveChecker
    {

        public static async Task VMSAliveCheckWorker()
        {
            _ = Task.Run(async () =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                bool previous_alive_state = true;
                clsAlarmCode alarm = AlarmManagerCenter.GetAlarmCode(ALARMS.VMS_DISCONNECT);
                clsAlarmDto disconnectAlarm = new clsAlarmDto()
                {
                    AlarmCode = (int)alarm.AlarmCode,
                    Description_En = alarm.Description_En,
                    Description_Zh = alarm.Description_Zh,
                    Equipment_Name = "VMS",
                    Level = ALARM_LEVEL.ALARM,
                    Source = ALARM_SOURCE.AGVS,

                };

                while (true)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        bool hasVmsDisconnectAlarm = alarm != null;
                        (bool alive, string message) response = await VMSAliveCheck();
                        if (previous_alive_state != response.alive)
                        {
                            if (!response.alive)
                            {
                                disconnectAlarm.Checked = false;
                                disconnectAlarm.Time = new DateTime(DateTime.Now.Ticks);
                                disconnectAlarm.ResetAalrmMemberName = "";
                            }
                        }
                        if (!response.alive)
                        {
                            disconnectAlarm.Duration = (int)(sw.ElapsedMilliseconds / 1000);

                            AGVStatusDBHelper agv_status_db = new AGVStatusDBHelper();
                            agv_status_db.ChangeAllOffline();
                            AlarmManagerCenter.UpdateAlarm(disconnectAlarm);
                        }
                        else
                        {
                            sw.Restart();
                            disconnectAlarm.ResetAalrmMemberName = typeof(AliveChecker).Name;
                            AlarmManagerCenter.ResetAlarm(disconnectAlarm, true);
                        }
                        previous_alive_state = response.alive;
                    }
                    catch (Exception ex)
                    {
                        ErrorLog(ex);
                    }
                }
            });
        }

        private static void ErrorLog(Exception ex)
        {
            Console.WriteLine($"[{typeof(AliveChecker).Name}] {ex.Message} ");
        }

        public static async Task<(bool alive, string message)> VMSAliveCheck(string url = "http://127.0.0.1:5036/ws/VMSAliveCheck")
        {
            try
            {
                bool alive = await Http.GetAsync<bool>(url);
                return (alive, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
