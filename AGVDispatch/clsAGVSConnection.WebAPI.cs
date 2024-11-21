using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.HttpTools;
using AGVSystemCommonNet6.Log;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSConnection
    {

        public enum VMS_HTTP_CHANNEL
        {
            ONLINE_MODE_GET,
            ONLINE_MODE_CHANGE_REQUEST,
            RUNNING_STATUS_REPORT,
            TASK_FEEDBACK,
            MEASURE_REPORT,
            CARRIER_VIRTUAL_ID_GET,
            LEAVE_WORKSTATION_REQUEST
        }

        public HttpHelper AGVsWebAPIHttp;

        public Dictionary<VMS_HTTP_CHANNEL, HttpHelper> VMSHttps = new Dictionary<VMS_HTTP_CHANNEL, HttpHelper>()
        {
            { VMS_HTTP_CHANNEL.ONLINE_MODE_GET,null},
            { VMS_HTTP_CHANNEL.ONLINE_MODE_CHANGE_REQUEST,null},
            { VMS_HTTP_CHANNEL.RUNNING_STATUS_REPORT,null},
            { VMS_HTTP_CHANNEL.TASK_FEEDBACK,null},
            { VMS_HTTP_CHANNEL.MEASURE_REPORT,null},
            { VMS_HTTP_CHANNEL.CARRIER_VIRTUAL_ID_GET,null},
            { VMS_HTTP_CHANNEL.LEAVE_WORKSTATION_REQUEST,null},
        };

        private void InitVMSWebAPIHttpChannels(string vms_host)
        {
            VMSHttps[VMS_HTTP_CHANNEL.ONLINE_MODE_GET] = new HttpHelper(vms_host, comment: VMS_HTTP_CHANNEL.ONLINE_MODE_GET.ToString());
            VMSHttps[VMS_HTTP_CHANNEL.ONLINE_MODE_CHANGE_REQUEST] = new HttpHelper(vms_host, comment: VMS_HTTP_CHANNEL.ONLINE_MODE_CHANGE_REQUEST.ToString());
            VMSHttps[VMS_HTTP_CHANNEL.RUNNING_STATUS_REPORT] = new HttpHelper(vms_host, comment: VMS_HTTP_CHANNEL.RUNNING_STATUS_REPORT.ToString());
            VMSHttps[VMS_HTTP_CHANNEL.TASK_FEEDBACK] = new HttpHelper(vms_host, comment: VMS_HTTP_CHANNEL.TASK_FEEDBACK.ToString());
            VMSHttps[VMS_HTTP_CHANNEL.MEASURE_REPORT] = new HttpHelper(vms_host, comment: VMS_HTTP_CHANNEL.MEASURE_REPORT.ToString());
            VMSHttps[VMS_HTTP_CHANNEL.CARRIER_VIRTUAL_ID_GET] = new HttpHelper(vms_host, comment: VMS_HTTP_CHANNEL.CARRIER_VIRTUAL_ID_GET.ToString());
            VMSHttps[VMS_HTTP_CHANNEL.LEAVE_WORKSTATION_REQUEST] = new HttpHelper(vms_host, comment: VMS_HTTP_CHANNEL.LEAVE_WORKSTATION_REQUEST.ToString());
        }
        public async Task<OnlineModeQueryResponse> GetOnlineMode(int timeout_)
        {
            try
            {
                string api_route = $"/api/AGV/OnlineMode?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(Get) {api_route}");
                var http = VMSHttps[VMS_HTTP_CHANNEL.ONLINE_MODE_GET];
                var response = await http.GetAsync<Dictionary<string, object>>(api_route, timeout_);
                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson(Formatting.None)}");
                return new OnlineModeQueryResponse
                {
                    RemoteMode = int.Parse(response["RemoteMode"].ToString()) == 0 ? REMOTE_MODE.OFFLINE : REMOTE_MODE.ONLINE,
                    TimeStamp = response["TimeStamp"].ToString()
                };
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                throw ex;
            }
            finally
            {

            }
        }

        public async Task<SimpleRequestResponse> PostOnlineModeChangeRequset(int currentTag, REMOTE_MODE mode)
        {
            try
            {

                var http = VMSHttps[VMS_HTTP_CHANNEL.ONLINE_MODE_CHANGE_REQUEST];
                string api_route = mode == REMOTE_MODE.ONLINE ? $"/api/AGV/OnlineReq?AGVName={EQName}&tag={currentTag}" : $"/api/AGV/OfflineReq?AGVName={EQName}&";
                logger?.LogTrace($"(Post) {api_route},body json =");
                var response = await http.PostAsync<SimpleRequestResponse, object>(api_route, null, timeout: 4);
                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson(Formatting.None)}");
                return response;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                throw ex;
            }
            finally
            {


            }
        }
        public async Task<SimpleRequestResponse> PostRunningStatus(clsRunningStatus status)
        {
            try
            {

                var http = VMSHttps[VMS_HTTP_CHANNEL.RUNNING_STATUS_REPORT];
                string api_route = $"/api/AGV/AGVStatus?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(Post) {api_route},body json = {status.ToJson(Formatting.None)}");
                var response = await http.PostAsync<Dictionary<string, object>, clsRunningStatus>(api_route, status, timeout: 8);
                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson(Formatting.None)}");
                var returnCode = int.Parse(response["ReturnCode"].ToString());
                return new SimpleRequestResponse
                {
                    ReturnCode = Enum.GetValues(typeof(RETURN_CODE)).Cast<RETURN_CODE>().First(code => ((int)code) == returnCode),
                };
            }
            catch (Exception ex)
            {

                logger?.LogError(ex, ex.Message);
                throw ex;
            }
            finally
            {


            }
        }

        public async Task<SimpleRequestResponse> PostTaskFeedback(clsFeedbackData feedback)
        {
            try
            {


                var http = VMSHttps[VMS_HTTP_CHANNEL.TASK_FEEDBACK];
                // return Ok(new { ReturnCode = 1, Message = "AGV Not Found" });
                var api_route = $"/api/AGV/TaskFeedback?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(Post) {api_route},body json ={feedback.ToJson(Formatting.None)}");

                var response = await http.PostAsync<Dictionary<object, string>, clsFeedbackData>(api_route, feedback, timeout: 8);
                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson(Formatting.None)}");
                var returnCode = int.Parse(response["ReturnCode"].ToString());
                return new SimpleRequestResponse
                {
                    ReturnCode = Enum.GetValues(typeof(RETURN_CODE)).Cast<RETURN_CODE>().First(code => ((int)code) == returnCode),
                    Message = response["Message"].ToString()
                };
            }
            catch (Exception ex)
            {
                logger?.LogError($"PostTaskFeedBack Exception:{ex.Message}", ex);
                return new SimpleRequestResponse
                {
                    Message = ex.Message,
                    ReturnCode = RETURN_CODE.System_Error
                };
            }
            finally
            {


            }
        }

        /// <summary>
        /// 上報量測資料
        /// </summary>
        /// <param name="measure_reuslt"></param>
        /// <returns></returns>
        public async Task<SimpleRequestResponse> ReportMeasureData(clsMeasureResult measure_reuslt)
        {
            try
            {
                // return Ok(new { ReturnCode = 1, Message = "AGV Not Found" });


                var http = VMSHttps[VMS_HTTP_CHANNEL.MEASURE_REPORT];
                var api_route = $"/api/AGV/ReportMeasure?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(Post){api_route},body json ={measure_reuslt.ToJson(Formatting.None)}");
                var response = await http.PostAsync<Dictionary<object, string>, clsMeasureResult>(api_route, measure_reuslt, timeout: 8);
                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson(Formatting.None)}");
                var returnCode = int.Parse(response["ReturnCode"].ToString());
                return new SimpleRequestResponse
                {
                    ReturnCode = Enum.GetValues(typeof(RETURN_CODE)).Cast<RETURN_CODE>().First(code => ((int)code) == returnCode),
                    Message = response["Message"].ToString()
                };
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                throw ex;
            }
            finally
            {


            }
        }

        public async Task<clsCarrierVirtualIDResponseWebAPI> GetCarrierVirtualID()
        {
            try
            {

                var http = VMSHttps[VMS_HTTP_CHANNEL.CARRIER_VIRTUAL_ID_GET];
                // return Ok(new { ReturnCode = 1, Message = "AGV Not Found" });
                var api_route = $"/api/AGV/CarrierVirtualID?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(GET){api_route},body json =");
                clsCarrierVirtualIDResponseWebAPI response = await http.GetAsync<clsCarrierVirtualIDResponseWebAPI>(api_route, timeout: 8);
                return response;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                throw ex;
            }
            finally
            {


            }
        }

        public async Task<clsEQOptions> GetEQInfo(int eQTag)
        {
            try
            {
                // return Ok(new { ReturnCode = 1, Message = "AGV Not Found" });
                var api_route = $"/api/Equipment/GetEQOptionByTag?eq_tag={eQTag}";
                logger?.LogTrace($"(GET){api_route},body json =");
                //(new { Tag = option.TagID, EqName = option.Name, AGVModbusGatewayPort = option.ConnOptions.AGVModbusGatewayPort }
                clsEQOptions response = await AGVsWebAPIHttp.GetAsync<clsEQOptions>(api_route);
                return response;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<List<clsEQOptions>> GetEQsInfos(int[] eQTags)
        {
            try
            {
                // return Ok(new { ReturnCode = 1, Message = "AGV Not Found" });
                var api_route = $"/api/Equipment/GetEQOptionsByTags";
                logger?.LogTrace($"(POST){api_route},body json =");
                //(new { Tag = option.TagID, EqName = option.Name, AGVModbusGatewayPort = option.ConnOptions.AGVModbusGatewayPort }
                List<clsEQOptions> response = await AGVsWebAPIHttp.PostAsync<List<clsEQOptions>, int[]>(api_route, eQTags);
                return response;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<bool> LeaveWorkStationRequest(string vehicleName, int currentEQTag)
        {
            try
            {
                var http = VMSHttps[VMS_HTTP_CHANNEL.LEAVE_WORKSTATION_REQUEST];

                var api_route = $"/api/AGV/LeaveWorkStationRequest?AGVName={vehicleName}&EQTag={currentEQTag}";
                logger?.LogTrace($"(POST){api_route},body json =");
                (bool success, string json) response = await http.PostAsync(api_route, null, timeout: 3);
                if (!response.success)
                    return false;

                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.json);
                return (bool)dict["confirm"];
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                return false;
            }
            finally
            {


            }
        }

        public class clsEQOptions
        {
            public int Tag { get; set; }
            public string EqName { get; set; } = "";
            public int AGVModbusGatewayPort { get; set; }
        }
    }
}
