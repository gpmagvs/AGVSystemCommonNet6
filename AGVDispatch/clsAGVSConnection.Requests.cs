using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static AGVSystemCommonNet6.AGVDispatch.Messages.clsVirtualIDQu;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSConnection
    {
        private bool TryExitResponseAck(bool accept_task, int system_byte)
        {
            if (AGVSMessageStoreDictionary.TryRemove(system_byte, out MessageBase _retMsg))
            {
                byte[] data = AGVSMessageFactory.CreateTaskDownloadReqAckData(EQName, SID, accept_task, system_byte, out clsSimpleReturnMessage ackMsg);
                _ = LOG.INFO($"TaskDownload Ack : {ackMsg.ToJson()}");
                _retMsg.Dispose();
                ackMsg.Dispose();
                return WriteDataOut(data);
            }
            else
                return false;
        }
        private bool TryTaskDownloadReqAckAsync(bool accept_task, int system_byte)
        {
            if (AGVSMessageStoreDictionary.TryRemove(system_byte, out MessageBase _retMsg))
            {
                byte[] data = AGVSMessageFactory.CreateTaskDownloadReqAckData(EQName, SID, accept_task, system_byte, out clsSimpleReturnMessage ackMsg);
                _ = LOG.INFO($"TaskDownload Ack : {ackMsg.ToJson()}");
                _retMsg.Dispose();
                ackMsg.Dispose();
                return WriteDataOut(data);
            }
            else
                return false;
        }
        public class clsActionFinishFeedback
        {

        }
        public async Task<bool> TryTaskFeedBackAsync(string TaskName, string TaskSimplex, int TaskSequence, int point_index, TASK_RUN_STATUS task_status, int currentTAg,
            clsCoordination coordination, CancellationToken cancelToken, bool isTaskCancel = false)
        {
            bool _IsActionFinishFeedback = task_status == TASK_RUN_STATUS.ACTION_FINISH;
            if (_IsActionFinishFeedback)
            {
                _ = LOG.WARN("Action Finish Feedback, Wait Main Status should not equal 'RUN' start");
                (bool confirmed, string message) _main_status_not_run_now = await WaitMainStatusNotRUNRepoted();
                if (!_main_status_not_run_now.confirmed)
                {
                    _ = LOG.Critical(_main_status_not_run_now.message);
                    return false;
                }
                _ = LOG.INFO("Action Finish Feedback, Confirmed Main Status is not 'RUN'!");

            }
            CancellationTokenSource _T1TimeoutCancelToskSource = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            byte[] data = AGVSMessageFactory.CreateTaskFeedbackMessageData(EQName, SID,
                TaskName, TaskSimplex, TaskSequence, point_index, task_status, currentTAg, coordination, out clsTaskFeedbackMessage msg);

            FeedbackData _FeedbackData = msg.Header.Values.First();
            _FeedbackData.IsFeedbackBecauseTaskCancel = isTaskCancel;

            try
            {
                bool _success = false;
                while (!_success)
                {
                    await Task.Delay(50);
                    // 检查取消标记，如果已经取消则立即退出循环
                    if (_T1TimeoutCancelToskSource.Token.IsCancellationRequested)
                    {
                        _T1TimeoutCancelToskSource.Token.ThrowIfCancellationRequested();
                    }
                    if (cancelToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException("已被流程取消");
                    }
                    if (_T1TimeoutCancelToskSource.IsCancellationRequested)
                    {
                        _success = false;
                    }
                    else
                    {
                        try
                        {
                            if (UseWebAPI)
                            {
                                SimpleRequestResponse response = await PostTaskFeedback(new clsFeedbackData(_FeedbackData));
                                _ = LOG.INFO($" Task Feedback to AGVS RESULT(Task:{TaskName}_{TaskSimplex},{(isTaskCancel ? "[Raise Because Task Cancel_0305]" : "")}| Point Index : {point_index}(Tag:{currentTAg}) | Status : {task_status.ToString()}) ===> {response.ReturnCode}");
                                _success = response.ReturnCode == RETURN_CODE.OK;
                            }
                            else
                            {
                                _ = LOG.TRACE($"Task Feedback: {msg.ToJson()}");
                                bool _sendMsgToAGVsSuccess = await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes, MESSAGE_TYPE.ACK_0304_TASK_FEEDBACK_REPORT_ACK, 2);
                                MessageBase _retMsg = null;
                                while (!AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out _retMsg))
                                {
                                    if (cancelToken.IsCancellationRequested)
                                        throw new TaskCanceledException("已被流程取消");
                                    if (_T1TimeoutCancelToskSource.Token.IsCancellationRequested)
                                        _T1TimeoutCancelToskSource.Token.ThrowIfCancellationRequested();
                                    await Task.Delay(1);
                                }
                                clsSimpleReturnMessage msg_return = (clsSimpleReturnMessage)_retMsg;
                                _ = LOG.INFO($"Task Feedback to AGVS RESULT" +
                                    $"(\r\nTaskName   :{TaskName}" +
                                    $"\r\nTaskSimplex :{TaskSimplex}" +
                                    $"\r\nPoint Index :{point_index}(Tag:{currentTAg})" +
                                    $"\r\nStatus      :{task_status.ToString()})" +
                                    $"\r\nResult      :{msg_return.ReturnData.ReturnCode}", color: msg_return.ReturnData.ReturnCode == RETURN_CODE.OK ? ConsoleColor.White : ConsoleColor.Yellow);
                                _retMsg.Dispose();
                                msg.Dispose();
                                _success = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = LOG.ERROR($"TryTaskFeedBackAsync FAIL>.>>{ex.Message}", ex);
                            _success = false;
                        }
                    }
                }
                return _success;
            }
            catch (TaskCanceledException ex)
            {
                _ = LOG.ERROR($"Task Feedback ({task_status}) to AGVS Process Cancel.({ex.Message})");
                return false;
            }
            catch (OperationCanceledException ex)
            {
                _ = LOG.ERROR($"Task Feedback to AGVS .T1 Timeout!");
                OnTaskFeedBack_T1Timeout?.Invoke(this, _FeedbackData);
                return false;
            }
            catch (Exception ex)
            {
                _ = LOG.ERROR($"Task Feedback to AGVS Exception Happend:{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }
        private async Task<(bool confirmed, string message)> WaitMainStatusNotRUNRepoted()
        {

            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            while (_GetLastMainStatusReported() == MAIN_STATUS.RUN)
            {
                if (cts.IsCancellationRequested)
                    return (false, "Wait Main Status of running status report should not equal 'RUN' Timeout!");
                await Task.Delay(100);
            }
            return (true, "");

            //取得前次上報狀態
            MAIN_STATUS _GetLastMainStatusReported()
            {
                if (UseWebAPI)
                    return previousRunningStatusReport_via_WEBAPI.AGV_Status;
                else
                    return previousRunningStatusReport_via_TCPIP.AGV_Status;
            }
        }

        public RunningStatus previousRunningStatusReport_via_TCPIP = new RunningStatus();
        public clsRunningStatus previousRunningStatusReport_via_WEBAPI = new clsRunningStatus();
        internal ManualResetEvent WaitExitResponse = new ManualResetEvent(false);
        internal int TagOfExitResponseFromAGVS = 0;
        public async Task<bool> Exist_Request(int tag)
        {
            return true; //TODO CHECK
            WaitExitResponse.Reset();
            byte[] data = AGVSMessageFactory.CreateExistRequestData(EQName, SID, tag, out clsExitRequest? exitReqMsg);
            bool success = await SendMsgToAGVSAndWaitReply(data, exitReqMsg.SystemBytes, MESSAGE_TYPE.ACK_0312_EXIT_REQUEST_ACK, 8);
            if (!AGVSMessageStoreDictionary.TryRemove(exitReqMsg.SystemBytes, out MessageBase mesg))
                return false;
            clsExitRequestACKMessage ackMsgWrapper = mesg as clsExitRequestACKMessage;
            if (ackMsgWrapper == null)
                return false;
            bool agvsTrafficStart = ackMsgWrapper.ExitRequestAck.ReturnCode == 0;
            if (!agvsTrafficStart)
                return false;
            //等待AGV交管完成 會送0313給車載
            bool reached = WaitExitResponse.WaitOne(3000);
            return reached && TagOfExitResponseFromAGVS == tag;
        }

        private async Task<(bool, SimpleRequestResponseWithTimeStamp runningStateReportAck)> TryRnningStateReportAsync(int timeout_ = 8)
        {
            clsRunningStatus runnginStatus = null;
            SimpleRequestResponse response = null;
            try
            {
                if (UseWebAPI)
                {
                    runnginStatus = OnWebAPIProtocolGetRunningStatus();
                    response = await PostRunningStatus(runnginStatus);
                    previousRunningStatusReport_via_WEBAPI = runnginStatus;
                    return (response.ReturnCode == RETURN_CODE.OK || response.ReturnCode == RETURN_CODE.NG, new SimpleRequestResponseWithTimeStamp
                    {
                        ReturnCode = response.ReturnCode,
                        TimeStamp = DateTime.Now.ToString()
                    });
                }
                else
                {
                    RunningStatus runningStatus = OnTcpIPProtocolGetRunningStatus();
                    byte[] data = AGVSMessageFactory.CreateRunningStateReportQueryData(EQName, SID, runningStatus, out clsRunningStatusReportMessage msg);
                    bool success = await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes, MESSAGE_TYPE.RunningStateReport_ACK_0106, timeout_);
                    if (!success)
                    {
                        return (false, null);
                    }
                    previousRunningStatusReport_via_TCPIP = msg.Header.First().Value;
                    if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase mesg))
                    {
                        clsRunningStatusReportResponseMessage QueryResponseMessage = mesg as clsRunningStatusReportResponseMessage;

                        if (QueryResponseMessage != null)
                        {
                            SimpleRequestResponseWithTimeStamp ack = QueryResponseMessage.RuningStateReportAck;
                            mesg.Dispose();
                            QueryResponseMessage.Dispose();
                            msg.Dispose();
                            return (true, ack);
                        }
                        else
                        {
                            msg.Dispose();
                            return (false, null);
                        }
                    }
                    else
                    {
                        msg.Dispose();
                        return (false, null);
                    }
                }


            }
            catch (Exception)
            {
                return (false, null);
            }
            finally
            {
                runnginStatus = null;
                response = null;
            }
        }

        private void TrySimpleReply(string header_key, bool reset_accept, int system_byte)
        {
            byte[] data = AGVSMessageFactory.CreateSimpleReturnMessageData(EQName, SID, header_key, reset_accept, system_byte, out clsSimpleReturnWithTimestampMessage msg);
            bool writeOutSuccess = WriteDataOut(data);
        }

        /// <summary>
        /// Send 0103 Online Request with tag 
        /// </summary>
        /// <param name="currentTag"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task<(bool success, RETURN_CODE return_code)> TrySendOnlineModeChangeRequest(int currentTag, REMOTE_MODE mode)
        {
            try
            {
                await Task.Delay(10);
                if (UseWebAPI)
                {
                    SimpleRequestResponse response = await PostOnlineModeChangeRequset(currentTag, mode);
                    return (response.ReturnCode == RETURN_CODE.OK || response.ReturnCode == RETURN_CODE.NG, response.ReturnCode);
                }
                else
                {
                    byte[] data = AGVSMessageFactory.CreateOnlineModeChangeRequesData(EQName, SID, currentTag, mode, out clsOnlineModeRequestMessage msg);
                    bool agvs_replyed = await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes, MESSAGE_TYPE.OnlineMode_Change_ACK_0104);
                    if (!agvs_replyed)
                        return (false, RETURN_CODE.No_Response);
                    if (AGVOnlineReturnCode == RETURN_CODE.OK)
                    {
                        await Task.Delay(1);
                        return (true, RETURN_CODE.OK);
                    }
                    else
                        return (false, RETURN_CODE.NG);
                }
            }
            catch (Exception ex)
            {
                _ = LOG.WARN($"[AGVS] OnlineModeChangeRequest Fail...Code Error:{ex.Message}");
                return (false, RETURN_CODE.System_Error);
            }
        }

        public async Task<(bool result, string virtual_id, string message)> TryGetVirtualID(VIRTUAL_ID_QUERY_TYPE QueryType, CST_TYPE CstType)
        {
            if (UseWebAPI)
            {
                try
                {
                    var response = await GetCarrierVirtualID();
                    return (true, response.VirtualID, "");
                }
                catch (Exception ex)
                {
                    return (false, "", ex.Message);
                }
            }
            byte[] data = AGVSMessageFactory.Create0323VirtualIDQueryMsg(EQName, SID, QueryType, CstType, out clsCarrierVirtualIDQueryMessage? msg);
            if (await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes, MESSAGE_TYPE.ACK_0324_VirtualID_ACK))
            {
                if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase mesg))
                {
                    clsCarrierVirtualIDResponseMessage QueryResponseMessage = mesg as clsCarrierVirtualIDResponseMessage;
                    var id = QueryResponseMessage.CarrierVirtualIDResponse.VirtualID.ToString();
                    mesg.Dispose();
                    return (true, id, "Success");
                }
                else
                {
                    return (false, "", "Fail");
                }
            }
            else
            {
                return (false, "", "Fail");
            }
        }
        public async Task<(bool, OnlineModeQueryResponse onlineModeQuAck)> TryOnlineModeQueryAsync(int timeout_ = 8)
        {
            OnlineModeQueryResponse response = null;
            try
            {
                await Task.Delay(10);
                if (UseWebAPI)
                {
                    response = await GetOnlineMode(timeout_);
                    VMS_API_Call_Fail_Flag = false;
                    return (true, response);
                }

                byte[] data = AGVSMessageFactory.CreateOnlineModeQueryData(EQName, SID, out clsOnlineModeQueryMessage msg);
                if (await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes, MESSAGE_TYPE.OnlineMode_Query_ACK_0102, timeout_))
                {
                    if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase mesg))
                    {
                        clsOnlineModeQueryResponseMessage QueryResponseMessage = mesg as clsOnlineModeQueryResponseMessage;
                        response = QueryResponseMessage.OnlineModeQueryResponse;
                        mesg.Dispose();
                        return (true, response);
                    }
                    else
                        return (false, null);
                }
                else
                {
                    return (false, null);
                }

            }
            catch (Exception ex)
            {
                _ = LOG.ERROR($"Try Get OnlineMode Fail{ex.Message}");
                VMS_API_Call_Fail_Flag = true;
                await Task.Delay(3000);
                return (false, null);
            }
            finally
            {
                response = null;
            }
        }
    }
}
