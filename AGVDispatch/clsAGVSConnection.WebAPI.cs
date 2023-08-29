﻿using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.HttpTools;
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
            var response = await WebAPIHttp.GetAsync<Dictionary<string, object>>($"/api/AGV/OnlineMode?AGVName={AGVSMessageFactory.EQName}&Model=0");

            return new OnlineModeQueryResponse
            {
                RemoteMode = int.Parse(response["RemoteMode"].ToString()) == 0 ? REMOTE_MODE.OFFLINE : REMOTE_MODE.ONLINE,
                TimeStamp = response["TimeStamp"].ToString()
            };
        }

        public async Task<SimpleRequestResponse> PostOnlineModeChangeRequset(int currentTag, REMOTE_MODE mode)
        {
            string api_route = mode== REMOTE_MODE.ONLINE? $"/api/AGV/OnlineReq?AGVName={AGVSMessageFactory.EQName}&tag={currentTag}": $"/api/AGV/OfflineReq?AGVName={AGVSMessageFactory.EQName}&";
            var response = await WebAPIHttp.PostAsync<SimpleRequestResponse, object>(api_route, null);
            return response;
        }
        public async Task<SimpleRequestResponse> PostRunningStatus(clsRunningStatus status)
        {
            var response = await WebAPIHttp.PostAsync<Dictionary<string, object>, clsRunningStatus>($"/api/AGV/AGVStatus?AGVName={AGVSMessageFactory.EQName}&Model=0", status);
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
                var response = await WebAPIHttp.PostAsync<Dictionary<object, string>, clsFeedbackData> ($"/api/AGV/TaskFeedback?AGVName={AGVSMessageFactory.EQName}&Model=0", feedback);
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
