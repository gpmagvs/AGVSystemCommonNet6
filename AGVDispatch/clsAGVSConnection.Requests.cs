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
using System.Threading.Tasks;
using System.Xml.Linq;
using static AGVSystemCommonNet6.AGVDispatch.Messages.clsVirtualIDQu;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSConnection
    {

        private bool TryTaskDownloadReqAckAsync(bool accept_task, int system_byte)
        {
            if (AGVSMessageStoreDictionary.TryRemove(system_byte, out MessageBase _retMsg))
            {
                byte[] data = AGVSMessageFactory.CreateTaskDownloadReqAckData(EQName, SID, accept_task, system_byte, out clsSimpleReturnMessage ackMsg);
                LOG.INFO($"TaskDownload Ack : {ackMsg.ToJson()}");
                _retMsg.Dispose();
                ackMsg.Dispose();
                return WriteDataOut(data);
            }
            else
                return false;
        }

        public async Task TryTaskFeedBackAsync(clsTaskDownloadData taskData, int point_index, TASK_RUN_STATUS task_status, int currentTAg, clsCoordination coordination, bool isTaskCancel = false, CancellationTokenSource cts = null)
        {
            await Task.Factory.StartNew(async () =>
            {
                cts = cts == null ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : cts;
                while (true)
                {
                    Thread.Sleep(1);
                    if (cts.IsCancellationRequested)
                    {
                        LOG.INFO($"Try Task Feedback to AGVS {task_status},{(isTaskCancel ? "[Raise Because Task Cancel_0305]" : "")} => Canceled");
                        break;
                    }
                    LOG.INFO($"Try Task Feedback to AGVS {task_status},{(isTaskCancel ? "[Raise Because Task Cancel_0305]" : "")}");
                    try
                    {
                        byte[] data = AGVSMessageFactory.CreateTaskFeedbackMessageData(EQName, SID, taskData, point_index, task_status, currentTAg, coordination, out clsTaskFeedbackMessage msg);

                        if (UseWebAPI)
                        {
                            SimpleRequestResponse response = await PostTaskFeedback(new clsFeedbackData(msg.Header.Values.First()));
                            LOG.INFO($" Task Feedback to AGVS RESULT(Task:{taskData.Task_Name}_{taskData.Task_Simplex},{(isTaskCancel ? "[Raise Because Task Cancel_0305]" : "")}| Point Index : {point_index}(Tag:{currentTAg}) | Status : {task_status.ToString()}) ===> {response.ReturnCode}");
                            return;
                        }
                        else
                        {
                            bool success = await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes);
                            if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase _retMsg))
                            {
                                clsSimpleReturnMessage msg_return = (clsSimpleReturnMessage)_retMsg;
                                LOG.INFO($" Task Feedback to AGVS RESULT(Task:{taskData.Task_Name}_{taskData.Task_Simplex}| Point Index : {point_index}(Tag:{currentTAg}) | Status : {task_status.ToString()}) ===> {msg_return.ReturnData.ReturnCode}");
                                _retMsg.Dispose();
                                msg.Dispose();
                                break;
                            }
                            else
                            {
                                LOG.ERROR($"TryTaskFeedBackAsync FAIL>.>>");
                            }
                            msg.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.ERROR($"TryTaskFeedBackAsync FAIL>.>>{ex.Message}", ex);
                        continue;
                    }
                }
            });

        }


        public RunningStatus previousRunningStatusReport_via_TCPIP = new RunningStatus();
        public clsRunningStatus previousRunningStatusReport_via_WEBAPI = new clsRunningStatus();
        private async Task<(bool, SimpleRequestResponseWithTimeStamp runningStateReportAck)> TryRnningStateReportAsync()
        {
            try
            {
                if (UseWebAPI)
                {
                    clsRunningStatus runnginStatus = OnWebAPIProtocolGetRunningStatus();
                    SimpleRequestResponse response = await PostRunningStatus(runnginStatus);
                    previousRunningStatusReport_via_WEBAPI = runnginStatus;
                    return (response.ReturnCode == RETURN_CODE.OK | response.ReturnCode == RETURN_CODE.NG, new SimpleRequestResponseWithTimeStamp
                    {
                        ReturnCode = response.ReturnCode,
                        TimeStamp = DateTime.Now.ToString()
                    });
                }
                else
                {
                    RunningStatus runningStatus = OnTcpIPProtocolGetRunningStatus();
                    byte[] data = AGVSMessageFactory.CreateRunningStateReportQueryData(EQName, SID, runningStatus, out clsRunningStatusReportMessage msg);
                    bool success = await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes);
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
                if (UseWebAPI)
                {
                    SimpleRequestResponse response = await PostOnlineModeChangeRequset(currentTag, mode);
                    return (response.ReturnCode == RETURN_CODE.OK | response.ReturnCode == RETURN_CODE.NG, response.ReturnCode);
                }
                else
                {
                    byte[] data = AGVSMessageFactory.CreateOnlineModeChangeRequesData(EQName, SID, currentTag, mode, out clsOnlineModeRequestMessage msg);
                    bool agvs_replyed = await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes);
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
                LOG.WARN($"[AGVS] OnlineModeChangeRequest Fail...Code Error:{ex.Message}");
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
            if (await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes))
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
        public async Task<(bool, OnlineModeQueryResponse onlineModeQuAck)> TryOnlineModeQueryAsync()
        {
            try
            {
                if (UseWebAPI)
                {
                    var response = await GetOnlineMode();
                    VMS_API_Call_Fail_Flag = false;
                    return (true, response);
                }

                byte[] data = AGVSMessageFactory.CreateOnlineModeQueryData(EQName, SID, out clsOnlineModeQueryMessage msg);
                if (await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes))
                {

                    if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase mesg))
                    {
                        clsOnlineModeQueryResponseMessage QueryResponseMessage = mesg as clsOnlineModeQueryResponseMessage;
                        var response = QueryResponseMessage.OnlineModeQueryResponse;
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
                LOG.ERROR($"Try Get OnlineMode Fail{ex.Message}");
                VMS_API_Call_Fail_Flag = true;
                return (false, null);
            }
        }

        public async Task<RETURN_CODE> TryRemoveCSTData(string toRemoveCSTID, string task_name = "", string opid = "")
        {
            try
            {
                byte[] data = AGVSMessageFactory.CreateCarrierRemovedData(EQName, SID, new string[] { toRemoveCSTID }, task_name, opid, out clsCarrierRemovedMessage msg);
                await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes);

                if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase mesg))
                {
                    clsSimpleReturnWithTimestampMessage CarrierRemovedAckMessage = mesg as clsSimpleReturnWithTimestampMessage;
                    return CarrierRemovedAckMessage.ReturnData.ReturnCode;
                }
                else
                    return RETURN_CODE.System_Error;
            }
            catch (Exception)
            {
                return RETURN_CODE.System_Error;
            }
        }
    }
}
