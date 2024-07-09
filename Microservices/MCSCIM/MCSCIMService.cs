using AGVSystemCommonNet6.HttpTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.MCS
{
    public class MCSCIMService
    {
        public static string MCSCIMUrl => "http://127.0.0.1:7107";
        private static HttpHelper agvs_http => new HttpHelper(MCSCIMUrl);
        public static async Task<(bool confirm, string message)> Online()
        {
            (bool confirm, string message) response = new(false, "[Online] Fail:");
            using (agvs_http)
            {
                try
                {
                    var route = $"/api/HostMode/OnlineLocal";
                    ResponseObject v = await agvs_http.GetAsync<ResponseObject>(route);
                    response.confirm = v.confirm;
                    response.message = v.message;
                }
                catch (Exception ex)
                {
                    response.message += ex.Message.ToString();
                }
            }
            return response;
        }
        public static async Task<(bool confirm, string message)> Offline()
        {
            (bool confirm, string message) response = new(false, "[Offline] Fail:");
            using (agvs_http)
            {
                try
                {
                    var route = $"/api/HostMode/OFFline";
                    ResponseObject v = await agvs_http.GetAsync<ResponseObject>(route);
                    response.confirm = v.confirm;
                    response.message = v.message;
                }
                catch (Exception ex)
                {
                    response.message += ex.Message.ToString();
                }
            }
            return response;
        }
        public static async Task<(bool confirm, string message)> OnlineLocalToOnlineRemote()
        {
            (bool confirm, string message) response = new(false, "[OnlineLocalToOnlineRemote] Fail");
            using (agvs_http)
            {
                try
                {
                    var route = $"/api/HostMode/OnlineLoacl2OnlineRemote";
                    ResponseObject v = await agvs_http.GetAsync<ResponseObject>(route);
                    response.confirm = v.confirm;
                    response.message = v.message;
                }
                catch (Exception ex)
                {
                    response.message += ex.ToString();
                }
            }
            return response;
        }
        public static async Task<(bool confirm, string message)> OnlineRemote2OnlineLocal()
        {
            (bool confirm, string message) response = new(false, "[OnlineRemote2OnlineLocal] Fail");
            using (agvs_http)
            {
                try
                {
                    var route = $"/api/HostMode/OnlineRemote2OnlineLocal";
                    ResponseObject v = await agvs_http.GetAsync<ResponseObject>(route);
                    response.confirm = v.confirm;
                    response.message = v.message;
                }
                catch (Exception ex)
                {
                    response.message += ex.ToString();
                }
            }
            return response;
        }
        /// <summary>
        /// data[0]:clsTaskDto 
        /// data[1]:task status, ref to AGVSCIM.TaskManager.clsTask.TaskStatus
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<(bool confirm, string message)> TaskReporter(object data)
        {
            (bool confirm, string message) response = new(false, "[MCSCIMService.TaskReporter] System Error.");
            using (agvs_http)
            {
                try
                {
                    var route = $"/api/HostMode/TaskCollecter";
                    string strJson = data.ToJson();
                    (bool success, string json) v = await agvs_http.PostAsync(route,data);
                    response.confirm = v.success;
                    response.message = v.json;
                }
                catch (Exception ex)
                {
                    response.message = $"[MCSCIMService.TaskReporter] Report to: {agvs_http.http_client.BaseAddress} with exmessage: {ex.Message}";
                }
            }
            return response;
        }


        internal class ResponseObject
        {
            public bool confirm { get; set; } = false;
            public string message { get; set; }
        }
    }
}
