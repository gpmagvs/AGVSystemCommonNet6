﻿using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.HttpTools;
using AGVSystemCommonNet6.Material;
using EquipmentManagment.WIP;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.MCS
{
    public partial class MCSCIMService
    {
        public static string MCSCIMUrl => "http://localhost:7107";
        private static HttpHelper _http = new HttpHelper(MCSCIMUrl);
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        public static bool IsHostOnline
        {
            get
            {
                using (AGVSDatabase db = new())
                {
                    Sys.AGVSSystemStatus? states = db.tables.SysStatus.AsNoTracking().FirstOrDefault();
                    if (states == null)
                        return false;
                    return states.HostConnMode == AGVDispatch.RunMode.HOST_CONN_MODE.ONLINE;
                }
            }
        }

        public static async Task<(bool confirm, string message)> IsPlatformAlive()
        {
            return await AliveCheck();
        }

        private static async Task<(bool confirm, string message)> AliveCheck()
        {
            try
            {
                var route = $"/api/AliveCheck";
                ResponseObject _response = await _http.GetAsync<ResponseObject>(route, 0.500, 1);
                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(bool confirm, string message)> Online(double Timeout = 3)
        {
            try
            {
                var route = $"/api/HostMode/OnlineLocal";
                ResponseObject _response = await _http.GetAsync<ResponseObject>(route, Timeout, 2);
                return (_response.confirm, _response.message);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Secs Platform Communication Error {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                return (false, $"Switch Online Timeout, Check Communication With Host({ex.Message})");
            }
            catch (JsonSerializationException ex)
            {
                return (false, $"系統錯誤");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public static async Task<(bool confirm, string message)> Offline(double Timeout = 3)
        {
            try
            {
                var route = $"/api/HostMode/OFFline";
                ResponseObject _response = await _http.GetAsync<ResponseObject>(route, Timeout, 2);
                return (_response.confirm, _response.message);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Secs Platform Communication Error ({ex.Message})");
            }
            catch (TaskCanceledException ex)
            {
                return (false, $"Switch Offline Timeout, Check Communication With Host({ex.Message})");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public static async Task<(bool confirm, string message)> OnlineLocalToOnlineRemote(int Timeout = 3)
        {
            (bool confirm, string message) response = new(false, "[OnlineLocalToOnlineRemote] Fail");
            try
            {
                var route = $"/api/HostMode/OnlineLoacl2OnlineRemote";
                ResponseObject v = await _http.GetAsync<ResponseObject>(route, Timeout, 2);
                response.confirm = v.confirm;
                response.message = v.message;
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Secs Platform Communication Error ({ex.Message})");
            }
            catch (TaskCanceledException ex)
            {
                return (false, $"Switch Remote Timeout, Check Communication With Host({ex.Message})");
            }
            catch (Exception ex)
            {
                response.message += ex.ToString();
            }
            return response;
        }
        public static async Task<(bool confirm, string message)> OnlineRemote2OnlineLocal(int Timeout = 3)
        {
            (bool confirm, string message) response = new(false, "[OnlineRemote2OnlineLocal] Fail");
            try
            {
                var route = $"/api/HostMode/OnlineRemote2OnlineLocal";
                ResponseObject v = await _http.GetAsync<ResponseObject>(route, Timeout, 2);
                response.confirm = v.confirm;
                response.message = v.message;
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Secs Platform Communication Error ({ex.Message})");
            }
            catch (TaskCanceledException ex)
            {
                return (false, $"Switch Local Timeout, Check Communication With Host({ex.Message})");
            }
            catch (Exception ex)
            {
                response.message += ex.ToString();
            }
            return response;
        }
        public static async Task AlarmReport(ushort alarmID, string alarmText)
        {
            try
            {
                await _http.PostAsync($"/api/Alarm/AlarmReport?alarmID={alarmID}&alarmText={alarmText}", null, 2, 2);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public static async Task AlarmClear(ushort alarmID, string alarmText)
        {
            try
            {
                await _http.PostAsync($"/api/Alarm/AlarmClear?alarmID={alarmID}&alarmText={alarmText}", null, 2, 2);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        internal class ResponseObject
        {
            public bool confirm { get; set; } = false;
            public string message { get; set; }
        }
        /// <summary>
        /// 需與 AGVSCIMServicePlatform.Models.clsTask.TaskStatus 完全一致
        /// </summary>
        public enum TaskStatus
        {
            None = 0,
            init = 1,
            wait_to_execute = 2,
            wait_to_assign = 3,
            assgined = 4,
            wait_to_start = 5,
            start = 6,
            wait_to_source = 7,
            at_source_wait_in = 8,
            wait_to_dest = 9,
            at_destination_wait_in = 10,
            wait_to_complete = 90,
            completed = 96, fail = 97, cancel = 98, finish_and_reported = 99, transport_fail = 93, ignore = 101
        }
    }
}
