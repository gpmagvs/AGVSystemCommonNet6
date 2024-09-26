using AGVSystemCommonNet6.AGVDispatch.Messages;
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

        public abstract class MessageHandlerAbstract
        {
            protected readonly clsAGVSConnection agvs_entity;

            public MessageHandlerAbstract(clsAGVSConnection agvs_entity)
            {
                this.agvs_entity = agvs_entity;
            }
            public virtual object HandleMessage(string jsonMessage)
            {
                var obj = GetMessageFromJson(jsonMessage);
                AddMessageToDict(obj);
                return obj;
            }
            protected abstract object GetMessageFromJson(string json);
            protected void AddMessageToDict(object MSG)
            {
                var _mess = (MessageBase)MSG;
                agvs_entity.AGVSMessageStoreDictionary.TryAdd(_mess.SystemBytes, _mess);
            }
        }
        public class MessageHandlerFactory
        {
            private readonly clsAGVSConnection agvs_entity;
            public MessageHandlerFactory(clsAGVSConnection agvs_entity)
            {
                this.agvs_entity = agvs_entity;
            }
            public MessageHandlerAbstract GetHandler(MESSAGE_TYPE messageType)
            {
                switch (messageType)
                {
                    case MESSAGE_TYPE.OnlineMode_Query_ACK_0102:
                        return new OnlineModeQueryAckHandler(agvs_entity);
                    case MESSAGE_TYPE.OnlineMode_Change_ACK_0104:
                        return new OnlineModeChangeAckHandler(agvs_entity);
                    case MESSAGE_TYPE.RunningStateReport_ACK_0106:
                        return new RunningStateReportAckHander(agvs_entity);
                    case MESSAGE_TYPE.REQ_0301_TASK_DOWNLOAD:
                        return new TaskDownloadHander(agvs_entity);
                    case MESSAGE_TYPE.ACK_0304_TASK_FEEDBACK_REPORT_ACK:
                        return new TaskFeedbackAckHandler(agvs_entity);
                    case MESSAGE_TYPE.REQ_0305_TASK_CANCEL:
                        return new TaskCancelReqHandler(agvs_entity);
                    case MESSAGE_TYPE.Carrier_Remove_Request_ACK_0322:
                        return new CarrierRemovedReportAckHandler(agvs_entity);
                    case MESSAGE_TYPE.ACK_0324_VirtualID_ACK:
                        return new VirtualIDQueryAckHandler(agvs_entity);
                    case MESSAGE_TYPE.ACK_0312_EXIT_REQUEST_ACK:
                        return new ExitRequestAckHandler(agvs_entity);
                    case MESSAGE_TYPE.REQ_0313_EXIT_RESPONSE:
                        return new ExitResponseHandler(agvs_entity);
                    default:
                        return null;
                }
            }
        }

        public class OnlineModeQueryAckHandler : MessageHandlerAbstract
        {
            public OnlineModeQueryAckHandler(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }

            public override object HandleMessage(string jsonMessage)
            {
                var _object = base.HandleMessage(jsonMessage);
                agvs_entity.CurrentREMOTE_MODE_Downloaded = ((clsOnlineModeQueryResponseMessage)_object).OnlineModeQueryResponse.RemoteMode;
                return _object;
            }

            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsOnlineModeQueryResponseMessage>(json);
            }
        }

        public class OnlineModeChangeAckHandler : MessageHandlerAbstract
        {
            public OnlineModeChangeAckHandler(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }

            public override object HandleMessage(string jsonMessage)
            {
                clsOnlineModeRequestResponseMessage response = (clsOnlineModeRequestResponseMessage)base.HandleMessage(jsonMessage);
                agvs_entity.AGVOnlineReturnCode = response.ReturnCode;
                return response;
            }

            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsOnlineModeRequestResponseMessage>(json);
            }
        }

        public class RunningStateReportAckHander : MessageHandlerAbstract
        {
            public RunningStateReportAckHander(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }

            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsRunningStatusReportResponseMessage>(json);
            }
        }

        public class AGVsOnlineReqHandler : MessageHandlerAbstract
        {
            public AGVsOnlineReqHandler(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }

            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsOnlineModeRequestMessage>(json);
            }
        }


        public class TaskDownloadHander : MessageHandlerAbstract
        {
            public TaskDownloadHander(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }
            public override object HandleMessage(string jsonMessage)
            {
                clsTaskDownloadMessage taskDownloadReq = (clsTaskDownloadMessage)base.HandleMessage(jsonMessage);
                taskDownloadReq.TaskDownload.OriTaskDataJson = jsonMessage;
                TASK_DOWNLOAD_RETURN_CODES return_code = agvs_entity.OnTaskDownload(taskDownloadReq.TaskDownload);
                if (agvs_entity.TryTaskDownloadReqAckAsync(return_code == TASK_DOWNLOAD_RETURN_CODES.OK, taskDownloadReq.SystemBytes))
                {
                    //WaitAckResetEvents[MESSAGE_TYPE.ACK_0304_TASK_FEEDBACK_REPORT_ACK].WaitOne();
                    agvs_entity.OnTaskDownloadFeekbackDone?.Invoke(this, taskDownloadReq.TaskDownload);
                }
                return taskDownloadReq;

            }
            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsTaskDownloadMessage>(json);
            }
        }


        public class TaskFeedbackAckHandler : MessageHandlerAbstract
        {
            public TaskFeedbackAckHandler(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }

            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsSimpleReturnMessage>(json);
            }
        }
        public class TaskCancelReqHandler : MessageHandlerAbstract
        {
            public TaskCancelReqHandler(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }
            public override async Task<object> HandleMessage(string jsonMessage)
            {
                clsTaskResetReqMessage objec = (clsTaskResetReqMessage)base.HandleMessage(jsonMessage);
                bool reset_accept = await agvs_entity.OnTaskResetReq(objec.ResetData.ResetMode, false);
                agvs_entity.TrySimpleReply("0306", reset_accept, objec.SystemBytes);
                return objec;
            }
            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsTaskResetReqMessage>(json);
            }
        }
        public class CarrierRemovedReportAckHandler : MessageHandlerAbstract
        {
            public CarrierRemovedReportAckHandler(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }

            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsSimpleReturnWithTimestampMessage>(json);
            }
        }

        /// <summary>
        /// 0324
        /// </summary>
        public class VirtualIDQueryAckHandler : MessageHandlerAbstract
        {
            public VirtualIDQueryAckHandler(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }
            public override object HandleMessage(string jsonMessage)
            {
                var _object = base.HandleMessage(jsonMessage);
                clsCarrierVirtualIDResponseMessage response = (clsCarrierVirtualIDResponseMessage)_object;
                return _object;
            }
            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsCarrierVirtualIDResponseMessage>(json);
            }
        }



        /// <summary>
        /// 0312
        /// </summary>
        public class ExitRequestAckHandler : MessageHandlerAbstract
        {
            public ExitRequestAckHandler(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }
            public override object HandleMessage(string jsonMessage)
            {
                var _object = base.HandleMessage(jsonMessage);
                clsExitRequestACKMessage response = (clsExitRequestACKMessage)_object;
                return _object;
            }
            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsExitRequestACKMessage>(json);
            }
        }



        /// <summary>
        /// 0314
        /// </summary>
        public class ExitResponseHandler : MessageHandlerAbstract
        {
            public ExitResponseHandler(clsAGVSConnection agvs_entity) : base(agvs_entity)
            {
            }
            public override object HandleMessage(string jsonMessage)
            {
                var _object = base.HandleMessage(jsonMessage);
                clsExitResponse response = (clsExitResponse)_object;
                agvs_entity.TagOfExitResponseFromAGVS = response.exitRequest.ExitPoint;
                agvs_entity.WaitExitResponse.Set();
                agvs_entity.TryExitResponseAck(true, response.SystemBytes);
                return _object;
            }
            protected override object GetMessageFromJson(string json)
            {
                return JsonConvert.DeserializeObject<clsExitResponse>(json);
            }
        }
    }
}
