using AGVSystemCommonNet6.AGVDispatch.Messages;
using System.Text;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public class AGVSMessageFactory
    {
        public static string SID { get; set; } = "001:001:001";
        public static string EQName { get; set; } = "AGV_1";

        public delegate RunningStatus GetVCSRunningDataDelegate(bool getPoseWithLastPtOfTrajectory = false);
        public static GetVCSRunningDataDelegate OnVCSRunningDataRequest;

        private static int SystemByteStored = 8790;
        private static int System_Byte_Cyclic
        {
            get
            {
                SystemByteStored = SystemByteStored + 1;
                if (SystemByteStored >= int.MaxValue)
                    SystemByteStored = 1;
                return int.Parse(SystemByteStored.ToString());
            }
        }
        public static void Setup(string _SID, string _EQName)
        {
            SID = _SID;
            EQName = _EQName;
        }

        internal static byte[] CreateSimpleReturnMessageData(string headerKey, bool confirmed, int system_byte, out clsSimpleReturnWithTimestampMessage ackMesg)
        {
            ackMesg = new clsSimpleReturnWithTimestampMessage()
            {
                EQName = EQName,
                SID = SID,
                SystemBytes = system_byte,
                Header = new Dictionary<string, SimpleRequestResponseWithTimeStamp>()
                    {
                        {headerKey, new SimpleRequestResponseWithTimeStamp()
                        {
                             ReturnCode = confirmed? RETURN_CODE.OK: RETURN_CODE.NG,
                              TimeStamp = DateTime.Now.ToAGVSTimeFormat()
                        }
                        }
                    }
            };
            return Encoding.ASCII.GetBytes(FormatSendOutString(ackMesg.ToJson()));

        }
        internal static byte[] CreateTaskDownloadReqAckData(bool accept_task, int system_byte, out clsSimpleReturnMessage ackMesg)
        {
            ackMesg = new clsSimpleReturnMessage()
            {
                EQName = EQName,
                SID = SID,
                SystemBytes = system_byte,
                Header = new Dictionary<string, SimpleRequestResponse>()
                    {
                        {"0302", new SimpleRequestResponse()
                        {
                             ReturnCode = accept_task? RETURN_CODE.OK: RETURN_CODE.NG,
                        }
                        }
                    }
            };
            return Encoding.ASCII.GetBytes(FormatSendOutString(ackMesg.ToJson()));

        }

        //internal static byte[] CreateSimpleReqAckData(bool accept_task, int system_byte, out MessageBase ackMesg)
        //{
        //    ackMesg = new MessageBase()
        //    {
        //        EQName = EQName,
        //        SID = SID,
        //        SystemBytes = system_byte,
        //        Header = new Dictionary<string, MessageHeader>()
        //        {

        //        }
        //    }
        //}

        public static byte[] CreateOnlineModeQueryData(out clsOnlineModeQueryMessage mesg)
        {
            mesg = new clsOnlineModeQueryMessage();
            mesg.SID = SID;
            mesg.EQName = EQName;
            mesg.SystemBytes = System_Byte_Cyclic;
            mesg.Header.Add("0101", new OnlineModeQuery
            {
                TimeStamp = DateTime.Now.ToAGVSTimeFormat(),
            });

            return Encoding.ASCII.GetBytes(FormatSendOutString(mesg.ToJson()));

        }

        public static byte[] CreateOnlineModeQueryData()
        {
            return CreateOnlineModeQueryData(out clsOnlineModeQueryMessage mesg);
        }

        internal static byte[] CreateOnlineModeChangeRequesData(int currentTag, REMOTE_MODE mode, out clsOnlineModeRequestMessage mesg)
        {
            mesg = new clsOnlineModeRequestMessage();
            mesg.SID = SID;
            mesg.EQName = EQName;
            mesg.SystemBytes = System_Byte_Cyclic;
            mesg.Header.Add("0103", new OnlineModeRequest
            {
                TimeStamp = DateTime.Now.ToAGVSTimeFormat(),
                CurrentNode = currentTag,
                ModeRequest = mode

            });

            return Encoding.ASCII.GetBytes(FormatSendOutString(mesg.ToJson()));
        }
        internal static Model.clsRunningStatus CreateRunningStateReportQueryData()
        {
            RunningStatus? runningData = OnVCSRunningDataRequest?.Invoke();
            return new Model.clsRunningStatus
            {
                AGV_Reset_Flag = runningData.AGV_Reset_Flag,
                AGV_Status = runningData.AGV_Status,
                Alarm_Code = runningData.Alarm_Code,
                Cargo_Status = runningData.Cargo_Status,
                Coordination = runningData.Coordination,
                CPU_Usage_Percent = runningData.CPU_Usage_Percent,
                CSTID = runningData.CSTID,
                Electric_Volume = runningData.Electric_Volume,
                Escape_Flag = runningData.Escape_Flag,
                Fork_Height = runningData.Fork_Height,
                Last_Visited_Node = runningData.Last_Visited_Node,
                Odometry = runningData.Odometry,
                RAM_Usage_Percent = runningData.RAM_Usage_Percent,
                Sensor_Status = runningData.Sensor_Status,
                Signal_Strength = runningData.Signal_Strength,
                Time_Stamp = runningData.Time_Stamp
            };
        }
        internal static byte[] CreateRunningStateReportQueryData(out clsRunningStatusReportMessage msg, bool getPoseOfLastPtOfTrajectory = false)
        {
            RunningStatus? runningData = OnVCSRunningDataRequest?.Invoke(getPoseOfLastPtOfTrajectory);
            msg = new clsRunningStatusReportMessage();
            msg.SID = SID;
            msg.EQName = EQName;
            msg.SystemBytes = System_Byte_Cyclic;
            msg.Header.Add("0105", runningData);
            return Encoding.ASCII.GetBytes(FormatSendOutString(msg.ToJson()));
        }

        public static string FormatSendOutString(string json)
        {
            return string.Format("{0}*{1}", json, "\r");
        }

        internal static byte[] CreateTaskFeedbackMessageData(clsTaskDownloadData taskData, int PointIndex, TASK_RUN_STATUS task_status, out clsTaskFeedbackMessage taskFeedbackMessage)
        {
            RunningStatus? runningData = OnVCSRunningDataRequest?.Invoke(false);
            taskFeedbackMessage = new clsTaskFeedbackMessage()
            {
                SID = SID,
                EQName = EQName,
                SystemBytes = System_Byte_Cyclic,
                Header = new Dictionary<string, FeedbackData>
                   {
                       {"0303", new FeedbackData
                        {
                            TaskName = taskData.Task_Name,
                             TaskSimplex = taskData.Task_Simplex,
                              TaskSequence = taskData.Task_Sequence,
                               PointIndex = PointIndex,
                                TaskStatus = task_status,
                                 TimeStamp = DateTime.Now.ToAGVSTimeFormat(),
                        }
                    }
                   }
            };
            return Encoding.ASCII.GetBytes(FormatSendOutString(taskFeedbackMessage.ToJson()));

        }

        internal static byte[] CreateCarrierRemovedData(string[] cstids, string task_name, string opid, out clsCarrierRemovedMessage carrierRemovedMessage)
        {
            carrierRemovedMessage = new clsCarrierRemovedMessage()
            {
                SID = SID,
                EQName = EQName,
                SystemBytes = System_Byte_Cyclic,
                Header = new Dictionary<string, CarrierRemovedData>()
                 {
                     {
                         "0321", new CarrierRemovedData
                         {
                              TimeStamp=DateTime.Now,
                              OPID = opid,
                              TaskName = task_name,
                              CSTID = cstids
                         }
                     }
                 }
            };
            return Encoding.ASCII.GetBytes(FormatSendOutString(carrierRemovedMessage.ToJson()));

        }
    }
}
