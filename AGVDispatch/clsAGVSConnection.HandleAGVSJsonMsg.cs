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
        internal Dictionary<MESSAGE_TYPE, ManualResetEvent> WaitAckResetEvents = new Dictionary<MESSAGE_TYPE, ManualResetEvent>
        {
            { MESSAGE_TYPE.OnlineMode_Query_ACK_0102, new ManualResetEvent(true) },
            { MESSAGE_TYPE.OnlineMode_Change_ACK_0104, new ManualResetEvent(true) },
            { MESSAGE_TYPE.RunningStateReport_ACK_0106, new ManualResetEvent(true) },
            { MESSAGE_TYPE.ACK_0304_TASK_FEEDBACK_REPORT_ACK, new ManualResetEvent(true) },
            { MESSAGE_TYPE.ACK_0312_EXIT_REQUEST_ACK, new ManualResetEvent(true) },
            { MESSAGE_TYPE.ACK_0324_VirtualID_ACK, new ManualResetEvent(true) }
        };

        public async void HandleAGVSJsonMsg(string _json)
        {
            MessageBase? MSG = null;
            MESSAGE_TYPE msgType = GetMESSAGE_TYPE(_json);
            logger.LogTrace(_json);
            try
            {
                if (msgType == MESSAGE_TYPE.UNKNOWN)
                {
                    LOG.ERROR($"Recieve undefined Msg {_json}");
                    return;
                }
                MessageHandlerFactory factory = new MessageHandlerFactory(this);
                MessageHandlerAbstract handler = factory.GetHandler(msgType);

                if (handler != null)
                    handler.HandleMessage(_json);

                #region Legacy Code

                //if (msgType == MESSAGE_TYPE.OnlineMode_Query_ACK_0102)
                //{
                //    clsOnlineModeQueryResponseMessage? onlineModeQuAck = JsonConvert.DeserializeObject<clsOnlineModeQueryResponseMessage>(_json);
                //    CurrentREMOTE_MODE_Downloaded = onlineModeQuAck.OnlineModeQueryResponse.RemoteMode;
                //    MSG = onlineModeQuAck;
                //    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                //}
                //else if (msgType == MESSAGE_TYPE.OnlineMode_Change_ACK_0104)  //AGV上線請求的回覆
                //{
                //    LOG.INFO($"AGVS Reply 0104 :{_json}");
                //    clsOnlineModeRequestResponseMessage? onlineModeRequestResponse = JsonConvert.DeserializeObject<clsOnlineModeRequestResponseMessage>(_json);
                //    AGVOnlineReturnCode = onlineModeRequestResponse.ReturnCode;
                //    MSG = onlineModeRequestResponse;
                //    AGVSMessageStoreDictionary.TryAdd(onlineModeRequestResponse.SystemBytes, MSG);
                //}
                //else if (msgType == MESSAGE_TYPE.RunningStateReport_ACK_0106)  //Running State Report的回覆
                //{
                //    clsRunningStatusReportResponseMessage? runningStateReportAck = JsonConvert.DeserializeObject<clsRunningStatusReportResponseMessage>(_json);
                //    MSG = runningStateReportAck;
                //    AGVSMessageStoreDictionary.TryAdd(runningStateReportAck.SystemBytes, MSG);
                //}
                //else if (msgType == MESSAGE_TYPE.REQ_0107_AGVS_Online_Req) //AGVS要求上線
                //{
                //    clsOnlineModeRequestMessage? onlineModeChageReq = JsonConvert.DeserializeObject<clsOnlineModeRequestMessage>(_json);
                //    MSG = onlineModeChageReq;
                //    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                //    var modereq = onlineModeChageReq.Header["0107"].ModeRequest;
                //    TrySimpleReply("0108", true, onlineModeChageReq.SystemBytes);
                //}
                //else if (msgType == MESSAGE_TYPE.REQ_0301_TASK_DOWNLOAD)  //TASK DOWNLOAD
                //{
                //    clsTaskDownloadMessage? taskDownloadReq = JsonConvert.DeserializeObject<clsTaskDownloadMessage>(_json);
                //    MSG = taskDownloadReq;
                //    AGVSMessageStoreDictionary.TryAdd(MSG.SystemBytes, MSG);
                //    taskDownloadReq.TaskDownload.OriTaskDataJson = _json;
                //    var return_code = OnTaskDownload(taskDownloadReq.TaskDownload);
                //    if (TryTaskDownloadReqAckAsync(return_code == TASK_DOWNLOAD_RETURN_CODES.OK, taskDownloadReq.SystemBytes))
                //    {
                //        //WaitAckResetEvents[MESSAGE_TYPE.ACK_0304_TASK_FEEDBACK_REPORT_ACK].WaitOne();
                //        OnTaskDownloadFeekbackDone?.Invoke(this, taskDownloadReq.TaskDownload);
                //    }
                //}
                //else if (msgType == MESSAGE_TYPE.ACK_0304_TASK_FEEDBACK_REPORT_ACK)  //TASK Feedback的回傳
                //{
                //    try
                //    {
                //        clsSimpleReturnMessage? taskFeedbackAck = JsonConvert.DeserializeObject<clsSimpleReturnMessage>(_json);
                //        MSG = taskFeedbackAck;
                //        AGVSMessageStoreDictionary.TryAdd(taskFeedbackAck.SystemBytes, MSG);
                //    }
                //    catch (Exception ex)
                //    {
                //        LOG.Critical($"處理0304過程發生例外:{ex.Message}", ex);
                //    }
                //}
                //else if (msgType == MESSAGE_TYPE.REQ_0305_TASK_CANCEL)
                //{
                //    clsTaskResetReqMessage? taskResetMsg = JsonConvert.DeserializeObject<clsTaskResetReqMessage>(_json);
                //    MSG = taskResetMsg;
                //    AGVSMessageStoreDictionary.TryAdd(taskResetMsg.SystemBytes, MSG);
                //    bool reset_accept = await OnTaskResetReq(taskResetMsg.ResetData.ResetMode, false);
                //    TrySimpleReply("0306", reset_accept, taskResetMsg.SystemBytes);
                //}
                //else if (msgType == MESSAGE_TYPE.Carrier_Remove_Request_ACK_0322)
                //{
                //    clsSimpleReturnWithTimestampMessage? taskFeedbackAck = JsonConvert.DeserializeObject<clsSimpleReturnWithTimestampMessage>(_json);
                //    MSG = taskFeedbackAck;
                //    AGVSMessageStoreDictionary.TryAdd(taskFeedbackAck.SystemBytes, MSG);
                //}
                //else if (msgType == MESSAGE_TYPE.ACK_0324_VirtualID_ACK) //Virtual ID Query結果回傳
                //{
                //    clsCarrierVirtualIDResponseMessage? virtualIDQuAck = JsonConvert.DeserializeObject<clsCarrierVirtualIDResponseMessage>(_json);
                //    MSG = virtualIDQuAck;
                //    AGVSMessageStoreDictionary.TryAdd(virtualIDQuAck.SystemBytes, MSG);
                //}

                #endregion

                if (WaitAckResetEvents.TryGetValue(msgType, out ManualResetEvent? manualResetEvent))
                {
                    if (msgType == MESSAGE_TYPE.ACK_0312_EXIT_REQUEST_ACK)
                    {

                    }
                    manualResetEvent.Set();
                }
            }
            catch (Exception ex)
            {
                LOG.ERROR("HandleAGVSJsonMsg_Code Error", ex);
            }
        }
    }
}
