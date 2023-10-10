using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Log;
using Microsoft.Extensions.Logging;
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
        public async void HandleAGVSJsonMsg(string _json)
        {
            MessageBase? MSG = null;
            MESSAGE_TYPE msgType = GetMESSAGE_TYPE(_json);
            LogMsgFromAGVS(_json);
            try
            {
                if (msgType == MESSAGE_TYPE.UNKNOWN)
                {
                    LOG.ERROR($"Recieve undefined Msg {_json}");
                    return;
                }

                if (msgType == MESSAGE_TYPE.ACK_0102)
                {
                    clsOnlineModeQueryResponseMessage? onlineModeQuAck = JsonConvert.DeserializeObject<clsOnlineModeQueryResponseMessage>(_json);
                    CurrentREMOTE_MODE_Downloaded = onlineModeQuAck.OnlineModeQueryResponse.RemoteMode;
                    MSG = onlineModeQuAck;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                }
                else if (msgType == MESSAGE_TYPE.ACK_0104)  //AGV上線請求的回覆
                {
                    LOG.INFO($"AGVS Reply 0104 :{_json}");
                    clsOnlineModeRequestResponseMessage? onlineModeRequestResponse = JsonConvert.DeserializeObject<clsOnlineModeRequestResponseMessage>(_json);
                    AGVOnlineReturnCode = onlineModeRequestResponse.ReturnCode;
                    MSG = onlineModeRequestResponse;
                    AGVSMessageStoreDictionary.TryAdd(onlineModeRequestResponse.SystemBytes, MSG);
                }
                else if (msgType == MESSAGE_TYPE.ACK_0106)  //Running State Report的回覆
                {
                    clsRunningStatusReportResponseMessage? runningStateReportAck = JsonConvert.DeserializeObject<clsRunningStatusReportResponseMessage>(_json);
                    MSG = runningStateReportAck;
                    AGVSMessageStoreDictionary.TryAdd(runningStateReportAck.SystemBytes, MSG);
                }
                else if (msgType == MESSAGE_TYPE.REQ_0107_AGVS_Online_Req) //AGVS要求上線
                {
                    clsOnlineModeRequestMessage? onlineModeChageReq = JsonConvert.DeserializeObject<clsOnlineModeRequestMessage>(_json);
                    MSG = onlineModeChageReq;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                    var modereq = onlineModeChageReq.Header["0107"].ModeRequest;
                    // bool? agv_accept = OnRemoteModeChanged?.Invoke(modereq, true);
                    TrySimpleReply("0108", true, onlineModeChageReq.SystemBytes);
                }
                else if (msgType == MESSAGE_TYPE.REQ_0301_TASK_DOWNLOAD)  //TASK DOWNLOAD
                {
                    clsTaskDownloadMessage? taskDownloadReq = JsonConvert.DeserializeObject<clsTaskDownloadMessage>(_json);
                    MSG = taskDownloadReq;
                    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                    taskDownloadReq.TaskDownload.OriTaskDataJson = _json;
                    var return_code = OnTaskDownload(taskDownloadReq.TaskDownload);
                    if (TryTaskDownloadReqAckAsync(return_code == TASK_DOWNLOAD_RETURN_CODES.OK, taskDownloadReq.SystemBytes))
                    {
                        OnTaskDownloadFeekbackDone?.Invoke(this, taskDownloadReq.TaskDownload);
                    }
                }
                else if (msgType == MESSAGE_TYPE.ACK_0304_TASK_FEEDBACK_REPORT_ACK)  //TASK Feedback的回傳
                {
                    clsSimpleReturnMessage? taskFeedbackAck = JsonConvert.DeserializeObject<clsSimpleReturnMessage>(_json);
                    MSG = taskFeedbackAck;
                    AGVSMessageStoreDictionary.TryAdd(taskFeedbackAck.SystemBytes, MSG);
                }
                else if (msgType == MESSAGE_TYPE.REQ_0305_TASK_CANCEL)
                {
                    clsTaskResetReqMessage? taskResetMsg = JsonConvert.DeserializeObject<clsTaskResetReqMessage>(_json);
                    MSG = taskResetMsg;
                    AGVSMessageStoreDictionary.TryAdd(taskResetMsg.SystemBytes, MSG);
                    bool reset_accept = await OnTaskResetReq(taskResetMsg.ResetData.ResetMode, false);
                    TrySimpleReply("0306", reset_accept, taskResetMsg.SystemBytes);
                }
                else if (msgType == MESSAGE_TYPE.ACK_0322)
                {
                    clsSimpleReturnWithTimestampMessage? taskFeedbackAck = JsonConvert.DeserializeObject<clsSimpleReturnWithTimestampMessage>(_json);
                    MSG = taskFeedbackAck;
                    AGVSMessageStoreDictionary.TryAdd(taskFeedbackAck.SystemBytes, MSG);
                }

                else if (msgType == MESSAGE_TYPE.ACK_0324_VirtualID_ACK) //Virtual ID Query結果回傳
                {
                    clsCarrierVirtualIDResponseMessage? virtualIDQuAck = JsonConvert.DeserializeObject<clsCarrierVirtualIDResponseMessage>(_json);
                    MSG = virtualIDQuAck;
                    AGVSMessageStoreDictionary.TryAdd(virtualIDQuAck.SystemBytes, MSG);
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
