﻿using AGVSystemCommonNet6.AGVDispatch.Messages;
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
        public class clsActionFinishFeedback
        {

        }
        public async Task<bool> TryTaskFeedBackAsync(string TaskName, string TaskSimplex, int TaskSequence, int point_index, TASK_RUN_STATUS task_status, int currentTAg, clsCoordination coordination, bool isTaskCancel = false, CancellationTokenSource cts = null)
        {
            return await Task.Run(async () =>
             {
                 cts = cts == null ? new CancellationTokenSource(TimeSpan.FromSeconds(5)) : cts;
                 var _T1TimeoutCancelToskSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                 byte[] data = AGVSMessageFactory.CreateTaskFeedbackMessageData(EQName, SID,
                     TaskName, TaskSimplex, TaskSequence, point_index, task_status, currentTAg, coordination, out clsTaskFeedbackMessage msg);
                 FeedbackData _FeedbackData = msg.Header.Values.First();
                 LOG.INFO($"Try Task Feedback to AGVS_Looping start::: {task_status},{(isTaskCancel ? "[Raise Because Task Cancel_0305]" : "")}");
                 bool _success = false;
                 while (!_success)
                 {
                     Thread.Sleep(1);
                     if (_T1TimeoutCancelToskSource.IsCancellationRequested)
                     {
                         OnTaskFeedBack_T1Timeout?.Invoke(this, _FeedbackData);
                         _success = false;
                         break;
                     }
                     if (cts.IsCancellationRequested)
                     {
                         LOG.WARN($"Task Feedback to AGVS({task_status})=> Canceled because process process canceled");
                         _success = false;
                         break;
                     }
                     try
                     {
                         if (UseWebAPI)
                         {
                             SimpleRequestResponse response = await PostTaskFeedback(new clsFeedbackData(_FeedbackData));
                             LOG.INFO($" Task Feedback to AGVS RESULT(Task:{TaskName}_{TaskSimplex},{(isTaskCancel ? "[Raise Because Task Cancel_0305]" : "")}| Point Index : {point_index}(Tag:{currentTAg}) | Status : {task_status.ToString()}) ===> {response.ReturnCode}");
                             _success = true;
                             return true;
                         }
                         else
                         {
                             await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes);
                             if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase _retMsg))
                             {
                                 clsSimpleReturnMessage msg_return = (clsSimpleReturnMessage)_retMsg;
                                 LOG.INFO($"Task Feedback to AGVS RESULT" +
                                     $"(\r\nTaskName   :{TaskName}" +
                                     $"\r\nTaskSimplex :{TaskSimplex}" +
                                     $"\r\nPoint Index :{point_index}(Tag:{currentTAg})" +
                                     $"\r\nStatus      :{task_status.ToString()})" +
                                     $"\r\nResult      :{msg_return.ReturnData.ReturnCode}", color: msg_return.ReturnData.ReturnCode == RETURN_CODE.OK ? ConsoleColor.White : ConsoleColor.Yellow);
                                 _retMsg.Dispose();
                                 msg.Dispose();
                                 _success = true;
                                 break;
                             }
                             else
                             {
                                 LOG.ERROR($"TryTaskFeedBackAsync FAIL>.>>");
                             }
                             msg.Dispose();
                             Thread.Sleep(200);
                         }
                     }
                     catch (Exception ex)
                     {
                         LOG.ERROR($"TryTaskFeedBackAsync FAIL>.>>{ex.Message}", ex);
                         continue;
                     }
                 }

                 return _success;
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
                    return (response.ReturnCode == RETURN_CODE.OK || response.ReturnCode == RETURN_CODE.NG, response.ReturnCode);
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
                Thread.Sleep(10);
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
