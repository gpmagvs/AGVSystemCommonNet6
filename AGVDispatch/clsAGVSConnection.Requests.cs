using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task TryTaskFeedBackAsync(clsTaskDownloadData taskData, int point_index, TASK_RUN_STATUS task_status, int currentTAg)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                while (true)
                {
                    await Task.Delay(1);
                    try
                    {
                        byte[] data = AGVSMessageFactory.CreateTaskFeedbackMessageData(taskData, point_index, task_status, out clsTaskFeedbackMessage msg);
                        if (UseWebAPI)
                        {
                            SimpleRequestResponse response = await PostTaskFeedback(new clsFeedbackData(msg.Header.Values.First()));
                            LOG.INFO($" Task Feedback to AGVS RESULT(Task:{taskData.Task_Name}_{taskData.Task_Simplex}| Point Index : {point_index}(Tag:{currentTAg}) | Status : {task_status.ToString()}) ===> {response.ReturnCode}");
                            return;
                        }
                        else
                        {
                            bool success = await WriteDataOut(data, msg.SystemBytes);
                            if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase _retMsg))
                            {
                                clsSimpleReturnMessage msg_return = (clsSimpleReturnMessage)_retMsg;
                                LOG.INFO($" Task Feedback to AGVS RESULT(Task:{taskData.Task_Name}_{taskData.Task_Simplex}| Point Index : {point_index}(Tag:{currentTAg}) | Status : {task_status.ToString()}) ===> {msg_return.ReturnData.ReturnCode}", false);
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



        private async Task<(bool, SimpleRequestResponseWithTimeStamp runningStateReportAck)> TryRnningStateReportAsync()
        {
            try
            {
                if (UseWebAPI)
                {
                    clsRunningStatus runnginStatus = AGVSMessageFactory.OnWebAPIProtocolGetRunningStatus();
                    SimpleRequestResponse response = await PostRunningStatus(runnginStatus);

                    return (response.ReturnCode == RETURN_CODE.OK | response.ReturnCode == RETURN_CODE.NG, new SimpleRequestResponseWithTimeStamp
                    {
                        ReturnCode = response.ReturnCode,
                        TimeStamp = DateTime.Now.ToString()
                    });
                }
                else
                {
                    byte[] data = AGVSMessageFactory.CreateRunningStateReportQueryData(out clsRunningStatusReportMessage msg);
                    await WriteDataOut(data, msg.SystemBytes);
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

        private void TryOnlineModeChangeReqRply_0108(int systemBytes)
        {
            byte[] data = AGVSMessageFactory.CreateSimpleReturnMessageData("0108", true, systemBytes, out clsSimpleReturnWithTimestampMessage msg);
            Console.WriteLine(msg.ToJson());
            bool writeOutSuccess = WriteDataOut(data);
            Console.WriteLine("TryTaskResetReqAckAsync : " + writeOutSuccess);
        }
        private void TryTaskResetReqAckAsync(bool reset_accept, int system_byte)
        {
            byte[] data = AGVSMessageFactory.CreateSimpleReturnMessageData("0306", reset_accept, system_byte, out clsSimpleReturnWithTimestampMessage msg);
            Console.WriteLine(msg.ToJson());
            bool writeOutSuccess = WriteDataOut(data);
            Console.WriteLine("TryTaskResetReqAckAsync : " + writeOutSuccess);

        }
        public async Task<(bool success, RETURN_CODE return_code)> TrySendOnlineModeChangeRequest(int currentTag, REMOTE_MODE mode)
        {
            Console.WriteLine($"[Online Mode Change] 車載請求 {mode} , Tag {currentTag}");
            try
            {
                AGVOnlineReturnCode = RETURN_CODE.No_Response;
                WaitAGVSAcceptOnline = new ManualResetEvent(false);
                if (UseWebAPI)
                {

                    SimpleRequestResponse response = await PostOnlineModeChangeRequset(currentTag, mode);
                    return (response.ReturnCode == RETURN_CODE.OK | response.ReturnCode == RETURN_CODE.NG, response.ReturnCode);
                }
                byte[] data = AGVSMessageFactory.CreateOnlineModeChangeRequesData(currentTag, mode, out clsOnlineModeRequestMessage msg);
                await WriteDataOut(data, msg.SystemBytes);
                if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase mesg))
                {
                }
                WaitAGVSAcceptOnline.WaitOne(1000);
                bool success = AGVOnlineReturnCode == RETURN_CODE.OK;
                return (success, AGVOnlineReturnCode);
            }
            catch (Exception ex)
            {
                LOG.WARN($"[AGVS] OnlineModeChangeRequest Fail...Code Error:{ex.Message}");
                return (false, RETURN_CODE.System_Error);
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
                await WriteDataOut(data, msg.SystemBytes);

                if (AGVSMessageStoreDictionary.TryRemove(msg.SystemBytes, out MessageBase mesg))
                {
                    clsOnlineModeQueryResponseMessage QueryResponseMessage = mesg as clsOnlineModeQueryResponseMessage;
                    return (true, QueryResponseMessage.OnlineModeQueryResponse);
                }
                else
                    return (false, null);
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
                await WriteDataOut(data, msg.SystemBytes);

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
