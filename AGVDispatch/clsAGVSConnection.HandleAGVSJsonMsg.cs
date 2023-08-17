using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSConnection
    {
        public void HandleAGVSJsonMsg(string _json)
        {
            MessageBase? MSG = null;
            MESSAGE_TYPE msgType = GetMESSAGE_TYPE(_json);

            try
            {
                if (msgType == MESSAGE_TYPE.ACK_0102)
                {
                    clsOnlineModeQueryResponseMessage? onlineModeQuAck = JsonConvert.DeserializeObject<clsOnlineModeQueryResponseMessage>(_json);
                    CurrentREMOTE_MODE_Downloaded = onlineModeQuAck.OnlineModeQueryResponse.RemoteMode;
                    OnRemoteModeChanged(CurrentREMOTE_MODE_Downloaded, true);
                    MSG = onlineModeQuAck;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                }
                else if (msgType == MESSAGE_TYPE.ACK_0104)  //AGV上線請求的回覆
                {
                    clsOnlineModeRequestResponseMessage? onlineModeRequestResponse = JsonConvert.DeserializeObject<clsOnlineModeRequestResponseMessage>(_json);
                    AGVOnlineReturnCode = onlineModeRequestResponse.ReturnCode;
                    WaitAGVSAcceptOnline.Set();
                    MSG = onlineModeRequestResponse;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                }
                else if (msgType == MESSAGE_TYPE.ACK_0106)  //Running State Report的回覆
                {
                    clsRunningStatusReportResponseMessage? runningStateReportAck = JsonConvert.DeserializeObject<clsRunningStatusReportResponseMessage>(_json);
                    MSG = runningStateReportAck;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                }
                else if (msgType == MESSAGE_TYPE.REQ_0301_TASK_DOWNLOAD)  //TASK DOWNLOAD
                {
                    clsTaskDownloadMessage? taskDownloadReq = JsonConvert.DeserializeObject<clsTaskDownloadMessage>(_json);
                    MSG = taskDownloadReq;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                    taskDownloadReq.TaskDownload.OriTaskDataJson = _json;
                    bool accept_task = OnTaskDownload(taskDownloadReq.TaskDownload);
                    if (TryTaskDownloadReqAckAsync(accept_task, taskDownloadReq.SystemBytes))
                    {
                        OnTaskDownloadFeekbackDone?.Invoke(this, taskDownloadReq.TaskDownload);
                    }
                }
                else if (msgType == MESSAGE_TYPE.ACK_0304_TASK_FEEDBACK_REPORT_ACK)  //TASK Feedback的回傳
                {
                    clsSimpleReturnMessage? taskFeedbackAck = JsonConvert.DeserializeObject<clsSimpleReturnMessage>(_json);
                    MSG = taskFeedbackAck;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                }
                else if (msgType == MESSAGE_TYPE.REQ_0305)
                {
                    clsTaskResetReqMessage? taskResetMsg = JsonConvert.DeserializeObject<clsTaskResetReqMessage>(_json);
                    MSG = taskResetMsg;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                    bool reset_accept = OnTaskResetReq(taskResetMsg.ResetData.ResetMode);
                    TryTaskResetReqAckAsync(reset_accept, taskResetMsg.SystemBytes);
                }
                else if (msgType == MESSAGE_TYPE.ACK_0322)
                {
                    clsSimpleReturnWithTimestampMessage? taskFeedbackAck = JsonConvert.DeserializeObject<clsSimpleReturnWithTimestampMessage>(_json);
                    MSG = taskFeedbackAck;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                }
                MSG.OriJsonString = _json;
                if (WaitAGVSReplyMREDictionary.TryRemove(MSG.SystemBytes, out ManualResetEvent mse))
                {
                    mse.Set();
                }
            }
            catch (Exception ex)
            {
                LOG.ERROR("HandleAGVSJsonMsg_Code Error", ex);
            }
        }

    }
}
