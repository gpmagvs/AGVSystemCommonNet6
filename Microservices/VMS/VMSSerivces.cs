using AGVSystemCommonNet6.AGVDispatch.RunMode;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.HttpTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.VMS
{
    public static class VMSSerivces
    {
        public static string VMSHostUrl => "http://127.0.0.1:5036";
        /// <summary>
        /// 請求VMS回覆
        /// </summary>
        /// <returns></returns>
        public static async Task AliveCheckWorker()
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
                    Thread.Sleep(1);
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

        public static async Task<(bool confirm, string message)> RunModeSwitch(RUN_MODE mode)
        {
            //confirm = confirm, message
            try
            {
                HttpHelper http = new HttpHelper(VMSHostUrl);
                Dictionary<string, object> response = await http.PostAsync<Dictionary<string, object>, object>($"/api/System/RunMode?mode={mode}", null);
                return ( (bool)response["confirm"], response["message"].ToString());
            }
            catch (Exception ex)
            {
                return (false,$"[VMS]:{ex.Message}");
            }
        }

        private static void ErrorLog(Exception ex)
        {
            Console.WriteLine($"[{typeof(AliveChecker).Name}] {ex.Message} ");
        }

        private static async Task<(bool alive, string message)> VMSAliveCheck()
        {
            try
            {
                HttpHelper http = new HttpHelper(VMSHostUrl);
                bool alive = await http.GetAsync<bool>($"/api/System/VMSAliveCheck");
                return (alive, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
