using AGVSystemCommonNet6.AGVDispatch.Messages;
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
        private bool _is_delay_invoke_task_downed = false;
        private bool _is_task_cancel_recieved = false;
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
                        _is_delay_invoke_task_downed = true;
                        Thread _wait_thred = new Thread((download_data) =>
                        {
                            clsTaskDownloadData data = (clsTaskDownloadData)download_data;

                            Stopwatch _stopwatch = Stopwatch.StartNew();
                            while (_stopwatch.ElapsedMilliseconds < 500)
                            {
                                Thread.Sleep(1);
                                if (_is_task_cancel_recieved)
                                {
                                    _stopwatch.Stop();
                                    _is_task_cancel_recieved = _is_delay_invoke_task_downed = false;
                                    LOG.WARN($"0301 TaskDownload Task not invoked. because recieve task cancle after {_stopwatch.ElapsedMilliseconds} ms");
                                    return;
                                }
                            }
                            _is_task_cancel_recieved = _is_delay_invoke_task_downed = false;
                            OnTaskDownloadFeekbackDone?.Invoke(this, data);
                            LOG.WARN($"0301 TaskDownload  invoked.");
                        });
                        _wait_thred.IsBackground = true;
                        _wait_thred.Start(taskDownloadReq.TaskDownload);
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

                    if (_is_delay_invoke_task_downed)
                    {
                        _is_task_cancel_recieved = true;
                        LOG.WARN($"0305 TASK_CANCEL not invoked. because 0301 not invoked yet");
                        TrySimpleReply("0306", true, taskResetMsg.SystemBytes);
                    }
                    else
                    {
                        bool reset_accept = await OnTaskResetReq(taskResetMsg.ResetData.ResetMode, false);
                        TrySimpleReply("0306", reset_accept, taskResetMsg.SystemBytes);
                    }
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
