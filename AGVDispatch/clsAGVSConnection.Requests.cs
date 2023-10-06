using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Alarm.VMS_ALARM;
using AGVSystemCommonNet6.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.AGVDispatch.Messages.clsVirtualIDQu;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSConnection
    {

        private bool TryTaskDownloadReqAckAsync(bool accept_task, int system_byte)
        {
            if (AGVSMessageStoreDictionary.TryRemove(system_byte, out MessageBase _retMsg))
            {
                byte[] data = AGVSMessageFactory.CreateTaskDownloadReqAckData(accept_task, system_byte, out clsSimpleReturnMessage ackMsg);
                LOG.INFO($"TaskDownload Ack : {ackMsg.ToJson()}");
                return WriteDataOut(data);
            }
            else
                return false;
        }

        public async Task TryTaskFeedBackAsync(clsTaskDownloadData taskData, int point_index, TASK_RUN_STATUS task_status, int currentTAg, clsCoordination coordination)
        {
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    LOG.INFO($" Task Feedback to AGVS {task_status}");
                    try
                    {
                        byte[] data = AGVSMessageFactory.CreateTaskFeedbackMessageData(taskData, point_index, task_status, currentTAg, coordination, out clsTaskFeedbackMessage msg);
                        if (UseWebAPI)
                        {
                            SimpleRequestResponse response = await PostTaskFeedback(new clsFeedbackData(msg.Header.Values.First()));
                            LOG.INFO($" Task Feedback to AGVS RESULT(Task:{taskData.Task_Name}_{taskData.Task_Simplex}| Point Index : {point_index}(Tag:{currentTAg}) | Status : {task_status.ToString()}) ===> {response.ReturnCode}");
                            return;
                        }
                        else
                        {
                            bool success = await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes);
                            if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase _retMsg))
                            {
                                clsSimpleReturnMessage msg_return = (clsSimpleReturnMessage)_retMsg;
                                LOG.INFO($" Task Feedback to AGVS RESULT(Task:{taskData.Task_Name}_{taskData.Task_Simplex}| Point Index : {point_index}(Tag:{currentTAg}) | Status : {task_status.ToString()}) ===> {msg_return.ReturnData.ReturnCode}");
                                break;
                            }
                            else
                            {
                                LOG.ERROR($"TryTaskFeedBackAsync FAIL>.>>");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.ERROR($"TryTaskFeedBackAsync FAIL>.>>{ex.Message}", ex);
                        return;
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
                    clsRunningStatus runnginStatus = AGVSMessageFactory.OnWebAPIProtocolGetRunningStatus();
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
                    byte[] data = AGVSMessageFactory.CreateRunningStateReportQueryData(out clsRunningStatusReportMessage msg);
                    LOG.TRACE(msg.ToJson(), false);
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
                            return (true, QueryResponseMessage.RuningStateReportAck);
                        else
                            return (false, null);
                    }
                    else
                        return (false, null);
                }


            }
            catch (Exception)
            {
                return (false, null);
            }
        }

        private void TrySimpleReply(string header_key, bool reset_accept, int system_byte)
        {
            byte[] data = AGVSMessageFactory.CreateSimpleReturnMessageData(header_key, reset_accept, system_byte, out clsSimpleReturnWithTimestampMessage msg);
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
                    byte[] data = AGVSMessageFactory.CreateOnlineModeChangeRequesData(currentTag, mode, out clsOnlineModeRequestMessage msg);
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
            byte[] data = AGVSMessageFactory.Create0323VirtualIDQueryMsg(QueryType, CstType, out clsCarrierVirtualIDQueryMessage? msg);
            if (await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes))
            {
                if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase mesg))
                {
                    clsCarrierVirtualIDResponseMessage QueryResponseMessage = mesg as clsCarrierVirtualIDResponseMessage;
                    return (true, QueryResponseMessage.CarrierVirtualIDResponse.VirtualID, "Success");
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

                byte[] data = AGVSMessageFactory.CreateOnlineModeQueryData(out clsOnlineModeQueryMessage msg);
                if (await SendMsgToAGVSAndWaitReply(data, msg.SystemBytes))
                {

                    if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase mesg))
                    {
                        clsOnlineModeQueryResponseMessage QueryResponseMessage = mesg as clsOnlineModeQueryResponseMessage;
                        return (true, QueryResponseMessage.OnlineModeQueryResponse);
                    }
                    else
                        return (false, null);
                }
                else
                {
                    return (false, null);
                }

            }
            catch (Exception)
            {
                VMS_API_Call_Fail_Flag = true;
                return (false, null);
            }
        }

        public async Task<RETURN_CODE> TryRemoveCSTData(string toRemoveCSTID, string task_name = "", string opid = "")
        {
            try
            {
                byte[] data = AGVSMessageFactory.CreateCarrierRemovedData(new string[] { toRemoveCSTID }, task_name, opid, out clsCarrierRemovedMessage msg);
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
