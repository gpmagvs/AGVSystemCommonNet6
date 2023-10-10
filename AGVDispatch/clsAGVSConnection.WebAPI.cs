using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.HttpTools;
using AGVSystemCommonNet6.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSConnection
    {

        public HttpHelper WebAPIHttp;
        public async Task<OnlineModeQueryResponse> GetOnlineMode()
        {
            string api_route = $"/api/AGV/OnlineMode?AGVName={AGVSMessageFactory.EQName}&Model={AGV_Model}";
            LogMsgToAGVS($"(Get) {api_route}");
            var response = await WebAPIHttp.GetAsync<Dictionary<string, object>>(api_route);

            LogMsgFromAGVS($"(Post) {api_route},Response={response.ToJson()}");
            return new OnlineModeQueryResponse
            {
                RemoteMode = int.Parse(response["RemoteMode"].ToString()) == 0 ? REMOTE_MODE.OFFLINE : REMOTE_MODE.ONLINE,
                TimeStamp = response["TimeStamp"].ToString()
            };
        }

        public async Task<SimpleRequestResponse> PostOnlineModeChangeRequset(int currentTag, REMOTE_MODE mode)
        {
            string api_route = mode == REMOTE_MODE.ONLINE ? $"/api/AGV/OnlineReq?AGVName={AGVSMessageFactory.EQName}&tag={currentTag}" : $"/api/AGV/OfflineReq?AGVName={AGVSMessageFactory.EQName}&";
            LogMsgToAGVS($"(Post) {api_route},body json =");
            var response = await WebAPIHttp.PostAsync<SimpleRequestResponse, object>(api_route, null);
            LogMsgFromAGVS($"(Post) {api_route},Response={response.ToJson()}");
            return response;
        }
        public async Task<SimpleRequestResponse> PostRunningStatus(clsRunningStatus status)
        {
            string api_route = $"/api/AGV/AGVStatus?AGVName={AGVSMessageFactory.EQName}&Model={AGV_Model}";
            LogMsgToAGVS($"(Post) {api_route},body json = {status.ToJson()}");
            var response = await WebAPIHttp.PostAsync<Dictionary<string, object>, clsRunningStatus>(api_route, status);
            LogMsgFromAGVS($"(Post) {api_route},Response={response.ToJson()}");
            var returnCode = int.Parse(response["ReturnCode"].ToString());
            return new SimpleRequestResponse
            {
                ReturnCode = Enum.GetValues(typeof(RETURN_CODE)).Cast<RETURN_CODE>().First(code => ((int)code) == returnCode),
            };
        }

        public async Task<SimpleRequestResponse> PostTaskFeedback(clsFeedbackData feedback)
        {
            try
            {
                // return Ok(new { ReturnCode = 1, Message = "AGV Not Found" });
                var api_route = $"/api/AGV/TaskFeedback?AGVName={AGVSMessageFactory.EQName}&Model={AGV_Model}";
                LogMsgToAGVS($"(Post) {api_route},body json ={feedback.ToJson()}");
                var response = await WebAPIHttp.PostAsync<Dictionary<object, string>, clsFeedbackData>(api_route, feedback);
                LogMsgFromAGVS($"(Post) {api_route},Response={response.ToJson()}");
                var returnCode = int.Parse(response["ReturnCode"].ToString());
                return new SimpleRequestResponse
                {
                    ReturnCode = Enum.GetValues(typeof(RETURN_CODE)).Cast<RETURN_CODE>().First(code => ((int)code) == returnCode),
                    Message = response["Message"].ToString()
                };
            }
            catch (Exception ex)
            {
                throw ex;
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

                var api_route = $"/api/AGV/ReportMeasure?AGVName={AGVSMessageFactory.EQName}&Model={AGV_Model}";
                LogMsgToAGVS($"(Post){api_route},body json ={measure_reuslt.ToJson()}");
                var response = await WebAPIHttp.PostAsync<Dictionary<object, string>, clsMeasureResult>(api_route, measure_reuslt);
                LogMsgFromAGVS($"(Post) {api_route},Response={response.ToJson()}");
                var returnCode = int.Parse(response["ReturnCode"].ToString());
                return new SimpleRequestResponse
                {
                    ReturnCode = Enum.GetValues(typeof(RETURN_CODE)).Cast<RETURN_CODE>().First(code => ((int)code) == returnCode),
                    Message = response["Message"].ToString()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<clsCarrierVirtualIDResponseWebAPI> GetCarrierVirtualID()
        {
            try
            {
                // return Ok(new { ReturnCode = 1, Message = "AGV Not Found" });
                var api_route = $"/api/AGV/CarrierVirtualID?AGVName={AGVSMessageFactory.EQName}&Model={AGV_Model}";
                LogMsgToAGVS($"(GET){api_route},body json =");
                clsCarrierVirtualIDResponseWebAPI response = await WebAPIHttp.GetAsync<clsCarrierVirtualIDResponseWebAPI>(api_route);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
