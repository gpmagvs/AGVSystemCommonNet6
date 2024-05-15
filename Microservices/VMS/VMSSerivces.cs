using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.RunMode;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.HttpTools;
using AGVSystemCommonNet6.Microservices.ResponseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;
using clsAlarmCode = AGVSystemCommonNet6.Alarm.clsAlarmCode;

namespace AGVSystemCommonNet6.Microservices.VMS
{
    public static class VMSSerivces
    {
        public static string VMSHostUrl => "http://127.0.0.1:5036";
        public static event EventHandler OnVMSReconnected;
        public static bool IsAlive { get; private set; } = false;
        public static Dictionary<VMS_GROUP, VMSConfig>? ReadVMSVehicleGroupSetting(string Vehicle_Json_file)
        {
            Dictionary<VMS_GROUP, VMSConfig> config = new Dictionary<VMS_GROUP, VMSConfig>();
            if (File.Exists(Vehicle_Json_file))
            {
                var json = File.ReadAllText(Vehicle_Json_file);
                config = JsonConvert.DeserializeObject<Dictionary<VMS_GROUP, VMSConfig>>(json);
            }
            else
            {
                config.Add(VMS_GROUP.GPM_FORK, new VMSConfig()
                {
                    AGV_List = new Dictionary<string, clsAGVOptions>()
                     {
                         { "AGV_001", new clsAGVOptions(){
                             Enabled = true,
                             HostIP="127.0.0.1",
                             HostPort=7025 ,
                             InitTag=50,
                             Protocol = clsAGVOptions.PROTOCOL.RESTFulAPI,
                             Simulation=false
                            }
                         }
                    }
                });
            }
            SaveVMSVehicleGroupSetting(Vehicle_Json_file, JsonConvert.SerializeObject(config, Formatting.Indented));
            return config;
        }
        public static void SaveVMSVehicleGroupSetting(string Vehicle_Json_file, string json)
        {
            File.WriteAllText(Vehicle_Json_file, json);
        }

        /// <summary>
        /// 請求VMS回覆
        /// </summary>
        /// <returns></returns>
        public static async void AliveCheckWorker()
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
                    await Task.Delay(5000);
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

        public static async Task TaskCancel(string taskName)
        {
            if (!IsAlive)
            {
                return;
            }
            try
            {
                using HttpHelper http = new HttpHelper(VMSHostUrl);
                await http.GetStringAsync($"/api/Task/Cancel?task_name={taskName}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static async Task<(bool alive, string message)> VMSAliveCheck()
        {
            try
            {
                HttpHelper http = new HttpHelper(VMSHostUrl);
                bool alive = await http.GetAsync<bool>($"/api/System/VMSAliveCheck", 8);
                return (alive, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async static Task<ResponseModel.clsResponseBase> AddPartsReplaceworkstationTag(int tagOfEQInPartsReplacing)
        {
            try
            {
                HttpHelper http = new HttpHelper(VMSHostUrl);
                var response = await http.GetAsync<ResponseModel.clsResponseBase>($"/api/Navigation/AddPartsReplaceworkstationTag?workstationTag={tagOfEQInPartsReplacing}", 3);
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel.clsResponseBase(false, ex.Message);
            }
        }
        public async static Task<ResponseModel.clsResponseBase> RemovePartsReplaceworkstationTag(int tagOfEQInPartsReplacing)
        {
            try
            {
                HttpHelper http = new HttpHelper(VMSHostUrl);
                var response = await http.GetAsync<ResponseModel.clsResponseBase>($"/api/Navigation/RemovePartsReplaceworkstationTag?workstationTag={tagOfEQInPartsReplacing}", 3);
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel.clsResponseBase(false, ex.Message);
            }
        }

        public struct TASK_DISPATCH
        {
            public async static Task<clsResponseBase> CheckOutAGVBatteryAndChargeStatus(string agvName, ACTION_TYPE orderAction)
            {
                try
                {

                    using (HttpHelper http = new HttpHelper(VMSHostUrl))
                    {
                        return await http.GetAsync<clsResponseBase>($"/api/Task/CheckOrderExecutableByBatStatus?agvName={agvName}&orderAction={orderAction}");
                    }
                }
                catch (Exception ex)
                {
                    return new clsResponseBase(false, $"服務器內部失敗:{ex.Message}");
                }
            }
        }
    }
}
