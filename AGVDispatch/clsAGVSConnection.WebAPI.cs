using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.HttpHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSConnection
    {
        public string WebAPIHost => $"http://{IP}:{Port}";

        public async Task<OnlineModeQueryResponse> GetOnlineMode()
        {
            var response = await Http.GetAsync<Dictionary<string, object>>($"{WebAPIHost}/api/AGV/OnlineMode?AGVName={AGVSMessageFactory.EQName}&Model=0");

            return new OnlineModeQueryResponse
            {
                RemoteMode = int.Parse(response["RemoteMode"].ToString()) == 0 ? REMOTE_MODE.OFFLINE : REMOTE_MODE.ONLINE,
                TimeStamp = response["TimeStamp"].ToString()
            };
        }

        public async Task<SimpleRequestResponse> PostOnlineModeChangeRequset(int currentTag, REMOTE_MODE mode)
        {
            string url = mode== REMOTE_MODE.ONLINE? $"{WebAPIHost}/api/AGV/OnlineReq?AGVName={AGVSMessageFactory.EQName}&tag={currentTag}": $"{WebAPIHost}/api/AGV/OfflineReq?AGVName={AGVSMessageFactory.EQName}&";
            var response = await Http.PostAsync<SimpleRequestResponse, object>(url, null);
            return response;
        }
        public async Task<SimpleRequestResponse> PostRunningStatus(clsRunningStatus status)
        {
            var response = await Http.PostAsync< Dictionary<string, object>, clsRunningStatus>($"{WebAPIHost}/api/AGV/AGVStatus?AGVName={AGVSMessageFactory.EQName}&Model=0", status);
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
                var response = await Http.PostAsync<  Dictionary<object, string>,clsFeedbackData > ($"{WebAPIHost}/api/AGV/TaskFeedback?AGVName={AGVSMessageFactory.EQName}&Model=0", feedback);
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


    }
}
