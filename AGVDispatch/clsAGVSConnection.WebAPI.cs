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

        public HttpHelper VMSWebAPIHttp;
        public HttpHelper AGVsWebAPIHttp;
        public async Task<OnlineModeQueryResponse> GetOnlineMode(int timeout_)
        {
            try
            {

                string api_route = $"/api/AGV/OnlineMode?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(Get) {api_route}");
                var response = await VMSWebAPIHttp.GetAsync<Dictionary<string, object>>(api_route, timeout_);

                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson()}");
                return new OnlineModeQueryResponse
                {
                    RemoteMode = int.Parse(response["RemoteMode"].ToString()) == 0 ? REMOTE_MODE.OFFLINE : REMOTE_MODE.ONLINE,
                    TimeStamp = response["TimeStamp"].ToString()
                };
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<SimpleRequestResponse> PostOnlineModeChangeRequset(int currentTag, REMOTE_MODE mode)
        {
            try
            {

                string api_route = mode == REMOTE_MODE.ONLINE ? $"/api/AGV/OnlineReq?AGVName={EQName}&tag={currentTag}" : $"/api/AGV/OfflineReq?AGVName={EQName}&";
                logger?.LogTrace($"(Post) {api_route},body json =");
                var response = await VMSWebAPIHttp.PostAsync<SimpleRequestResponse, object>(api_route, null);
                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson()}");
                return response;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                throw;
            }
        }
        public async Task<SimpleRequestResponse> PostRunningStatus(clsRunningStatus status)
        {
            try
            {

                string api_route = $"/api/AGV/AGVStatus?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(Post) {api_route},body json = {status.ToJson()}");
                var response = await VMSWebAPIHttp.PostAsync<Dictionary<string, object>, clsRunningStatus>(api_route, status);
                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson()}");
                var returnCode = int.Parse(response["ReturnCode"].ToString());
                return new SimpleRequestResponse
                {
                    ReturnCode = Enum.GetValues(typeof(RETURN_CODE)).Cast<RETURN_CODE>().First(code => ((int)code) == returnCode),
                };
            }
            catch (Exception ex)
            {

                logger?.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<SimpleRequestResponse> PostTaskFeedback(clsFeedbackData feedback)
        {
            try
            {
                // return Ok(new { ReturnCode = 1, Message = "AGV Not Found" });
                var api_route = $"/api/AGV/TaskFeedback?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(Post) {api_route},body json ={feedback.ToJson()}");

                var response = await VMSWebAPIHttp.PostAsync<Dictionary<object, string>, clsFeedbackData>(api_route, feedback, 1);
                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson()}");
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

                var api_route = $"/api/AGV/ReportMeasure?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(Post){api_route},body json ={measure_reuslt.ToJson()}");
                var response = await VMSWebAPIHttp.PostAsync<Dictionary<object, string>, clsMeasureResult>(api_route, measure_reuslt);
                logger?.LogTrace($"(Post) {api_route},Response={response.ToJson()}");
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
        }

        public async Task<clsCarrierVirtualIDResponseWebAPI> GetCarrierVirtualID()
        {
            try
            {
                // return Ok(new { ReturnCode = 1, Message = "AGV Not Found" });
                var api_route = $"/api/AGV/CarrierVirtualID?AGVName={EQName}&Model={AGV_Type}";
                logger?.LogTrace($"(GET){api_route},body json =");
                clsCarrierVirtualIDResponseWebAPI response = await VMSWebAPIHttp.GetAsync<clsCarrierVirtualIDResponseWebAPI>(api_route);
                return response;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                throw;
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
                throw;
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
                throw;
            }
        }

        public async Task<bool> LeaveWorkStationRequest(string vehicleName, int currentEQTag)
        {
            try
            {
                var api_route = $"/api/AGV/LeaveWorkStationRequest?AGVName={vehicleName}&EQTag={currentEQTag}";
                logger?.LogTrace($"(POST){api_route},body json =");
                (bool success, string json) response = await VMSWebAPIHttp.PostAsync(api_route, null);
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
        }

        public class clsEQOptions
        {
            public int Tag { get; set; }
            public string EqName { get; set; } = "";
            public int AGVModbusGatewayPort { get; set; }
        }
    }
}
