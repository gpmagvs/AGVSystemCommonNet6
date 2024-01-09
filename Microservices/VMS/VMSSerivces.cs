using AGVSystemCommonNet6.AGVDispatch.RunMode;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.HttpTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.Microservices.VMS
{
    public static class VMSSerivces
    {
        public static string VMSHostUrl => "http://127.0.0.1:5036";
        public static event EventHandler OnVMSReconnected;
        public static List<clsAGVStateDto> AgvStatesData = new List<clsAGVStateDto>();
        public static bool IsAlive = false;


        public static Dictionary<VMS_GROUP, VMSConfig>? ReadVMSVehicleGroupSetting(string Vehicle_Json_file)
        {
            if (File.Exists(Vehicle_Json_file))
            {
                var json = File.ReadAllText(Vehicle_Json_file);
                if (json == null)
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<Dictionary<VMS_GROUP, VMSConfig>>(json);
            }
            else
            {
                return null;
            }
        }
        public static void SaveVMSVehicleGroupSetting(string Vehicle_Json_file, string json)
        {
            File.WriteAllText(Vehicle_Json_file, json);
        }

        /// <summary>
        /// 請求VMS回覆
        /// </summary>
        /// <returns></returns>
        public static async Task AliveCheckWorker()
        {
            _ = Task.Run(async () =>
            {
                IsAlive = true;
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
                    Thread.Sleep(5000);
                    try
                    {
                        bool hasVmsDisconnectAlarm = alarm != null;
                        (bool alive, string message) response = await VMSAliveCheck();
                        if (previous_alive_state != response.alive)
                        {
                            if (!response.alive)
                            {
                                IsAlive = false;
                                sw.Restart();
                                disconnectAlarm.Checked = false;
                                disconnectAlarm.Time = DateTime.Now;
                                AlarmManagerCenter.AddAlarmAsync(ALARMS.VMS_DISCONNECT, ALARM_SOURCE.AGVS, Equipment_Name: "VMS");
                            }
                            else
                            {
                                IsAlive = true;
                                OnVMSReconnected?.Invoke("", EventArgs.Empty);
                                sw.Restart();
                                await AlarmManagerCenter.SetAlarmCheckedAsync("VMS", ALARMS.VMS_DISCONNECT, "SystemAuto");
                            }
                        }
                        else if (!response.alive)
                        {
                            disconnectAlarm.Duration = (int)(sw.ElapsedMilliseconds / 1000);
                            AlarmManagerCenter.UpdateAlarmAsync(disconnectAlarm);
                            continue;
                        }

                        previous_alive_state = response.alive;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            });
        }

        public static void AgvStateFetchWorker()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(100);
                    if (!IsAlive)
                        continue;

                    AgvStatesData = await GetAGV_StatesData_FromVMS();
                }
            });
        }

        public static async Task<List<clsAGVStateDto>> GetAGV_StatesData_FromVMS()
        {
            try
            {
                HttpHelper httpHelper = new HttpHelper($"http://127.0.0.1:5036", timeout_sec: 2);
                return await httpHelper.GetAsync<List<clsAGVStateDto>>("/api/VmsManager/AGVStatus");
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static async Task<(bool confirm, string message)> RunModeSwitch(RUN_MODE mode, bool forecing_change = false)
        {
            //confirm = confirm, message
            try
            {
                HttpHelper http = new HttpHelper(VMSHostUrl);
                Dictionary<string, object> response = await http.PostAsync<Dictionary<string, object>, object>($"/api/System/RunMode?mode={mode}&forecing_change={forecing_change}", null);
                return ((bool)response["confirm"], response["message"].ToString());
            }
            catch (Exception ex)
            {
                return (false, $"[VMS]:{ex.Message}");
            }
        }


        private static async Task<(bool alive, string message)> VMSAliveCheck()
        {
            try
            {
                HttpHelper http = new HttpHelper(VMSHostUrl);
                bool alive = await http.GetAsync<bool>($"/api/System/VMSAliveCheck", 5);
                return (alive, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
