using AGVSystemCommonNet6.Abstracts;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Log;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSConnection : Connection
    {
        public override string alarm_locate_in_name => "TCP AGVSConnection";

        TcpClient? tcpClient;
        clsSocketState socketState = new clsSocketState();
        ConcurrentDictionary<int, ManualResetEvent> WaitAGVSReplyMREDictionary = new ConcurrentDictionary<int, ManualResetEvent>();
        ConcurrentDictionary<int, MessageBase> AGVSMessageStoreDictionary = new ConcurrentDictionary<int, MessageBase>();
        bool VMS_API_Call_Fail_Flag = true;
        public delegate TASK_DOWNLOAD_RETURN_CODES taskDonwloadExecuteDelage(clsTaskDownloadData taskDownloadData);
        public delegate bool onlineModeChangeDelelage(REMOTE_MODE mode, bool isAGVSRequest);
        public delegate Task<bool> taskResetReqDelegate(RESET_MODE reset_data, bool isNormal);
        public event EventHandler<clsTaskDownloadData> OnTaskDownloadFeekbackDone;
        public taskDonwloadExecuteDelage OnTaskDownload;
        public onlineModeChangeDelelage OnRemoteModeChanged;
        public taskResetReqDelegate OnTaskResetReq;
        public event EventHandler OnDisconnected;
        public bool UseWebAPI = false;
        public enum MESSAGE_TYPE
        {
            REQ_0101_ONLINE_MODE_QUERY = 0101,
            ACK_0102 = 0102,
            REQ_0103_ONLINE_MODE_REQUEST = 0103,
            ACK_0104 = 0104,
            REQ_0105_RUNNING_STATUS_REPORT = 0105,
            ACK_0106 = 0106,
            REQ_0107_AGVS_Online_Req = 0107,
            ACK_0107_AGVS_Online_Req = 0108,
            REQ_0301_TASK_DOWNLOAD = 0301,
            ACK_0302_TASK_DOWNLOADED_ACK = 0302,
            REQ_0303_TASK_FEEDBACK_REPORT = 0303,
            ACK_0304_TASK_FEEDBACK_REPORT_ACK = 0304,
            REQ_0305_TASK_CANCEL = 0305,
            ACK_0306_TASK_CANCEL_ACK = 0306,
            ACK_0322 = 0322,
            REQ_0323_VirtualID_Query = 0323,
            ACK_0324_VirtualID_ACK = 0324,
            UNKNOWN = 9999,
        }

        public string LocalIP { get; }
        public AGV_MODEL AGV_Model { get; }

        public clsAGVSConnection(string IP, int Port) : base(IP, Port)
        {
            this.IP = IP;
            this.Port = Port;
            LocalIP = null;
            WebAPIHttp = new HttpTools.HttpHelper($"http://{IP}:{Port}");
        }
        public clsAGVSConnection(string HostIP, int HostPort, string localIP, AGV_MODEL AGV_Model = AGV_MODEL.FORK_AGV)
        {
            this.IP = HostIP;
            this.Port = HostPort;
            this.LocalIP = localIP;
            this.AGV_Model = AGV_Model;
            WebAPIHttp = new HttpTools.HttpHelper($"http://{IP}:{Port}");
        }


        public override bool Connect()
        {
            try
            {
                if (UseWebAPI)
                    return true;

                if (LocalIP != null)
                {
                    IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Parse(LocalIP), 0);
                    tcpClient = new TcpClient(ipEndpoint);
                    tcpClient.ReceiveBufferSize = 65535;
                    tcpClient.Connect(IP, Port);
                }
                else
                {
                    tcpClient = new TcpClient();
                    tcpClient.Connect(IP, Port);
                }
                socketState.stream = tcpClient.GetStream();
                socketState.Reset();
                socketState.stream.BeginRead(socketState.buffer, socketState.offset, clsSocketState.buffer_size - socketState.offset, ReceieveCallbaak, socketState);
                LOG.INFO($"[AGVS] Connect To AGVS Success !!");
                return true;
            }
            catch (Exception ex)
            {
                LOG.ERROR($"[AGVS] Connect Fail..{ex.Message}. Can't Connect To AGVS ({IP}:{Port})..Will Retry it after 3 secoond...", false);
                tcpClient = null;
                Thread.Sleep(3000);
                return false;
            }
        }

        public async Task<bool> Start()
        {
            _ = Task.Factory.StartNew(async () =>
            {
                int disconnect_cnt = 0;
                while (true)
                {
                    await Task.Delay(300);
                    try
                    {

                        if (!IsConnected())
                        {
                            if (!UseWebAPI)
                            {
                                LOG.WARN($"Try Connect TO AGVS Via TCP/IP(${IP}:{Port})", false);
                                Connect();
                                continue;
                            }
                        }
                        (bool, OnlineModeQueryResponse onlineModeQuAck) result = (false, new OnlineModeQueryResponse());
                        int retryCnt = 0;
                        while (!result.Item1)
                        {
                            Thread.Sleep(1000);
                            retryCnt += 1;
                            if (retryCnt > 5)
                                break;
                            result = TryOnlineModeQueryAsync().Result;
                            if (!result.Item1)
                            {
                                LOG.WARN($"Can't Get OnlineMode From AGVS..Try-{retryCnt}/5", false);
                            }
                        }

                        if (!result.Item1)
                        {
                            LOG.Critical("[AGVS] OnlineMode Query Fail...AGVS No Response");
                            if (!UseWebAPI)
                                Disconnect();
                            Current_Warning_Code = Alarm.VMS_ALARM.AlarmCodes.AGVS_Disconnect;
                            continue;
                        }
                        else
                        {
                            if (UseWebAPI)
                                VMS_API_Call_Fail_Flag = false;
                            Current_Warning_Code = Alarm.VMS_ALARM.AlarmCodes.None;
                            disconnect_cnt = 0;
                        }

                        (bool, SimpleRequestResponseWithTimeStamp runningStateReportAck) runningStateReport_result = TryRnningStateReportAsync().Result;
                        if (!runningStateReport_result.Item1)
                        {
                            LOG.Critical("[AGVS] Running State Report Fail...AGVS No Response");
                        }

                    }
                    catch (Exception ex)
                    {
                    }
                }
            });
            return true;
        }

        void ReceieveCallbaak(IAsyncResult ar)
        {
            clsSocketState _socketState = (clsSocketState)ar.AsyncState;
            try
            {

                int rev_len = _socketState.stream.EndRead(ar);

                string _revStr = Encoding.ASCII.GetString(_socketState.buffer, _socketState.offset, rev_len);
                _socketState.revStr += _revStr;
                _socketState.offset += rev_len;

                if (_revStr.EndsWith("*\r"))
                {
                    string strHandle = _socketState.revStr.Replace("*\r", "$");
                    string[] splited = strHandle.Split('$');//預防粘包，包含多個message包

                    foreach (var str in splited)
                    {
                        if (str == "" | str == null | str == "\r")
                            continue;
                        string _json = str.TrimEnd(new char[] { '*' });
                        HandleAGVSJsonMsg(_json);
                    }
                    _socketState.Reset();
                    _socketState.waitSignal.Set();

                }
                else
                {
                }

                try
                {
                    Task.Factory.StartNew(() => _socketState.stream.BeginRead(_socketState.buffer, _socketState.offset, clsSocketState.buffer_size - _socketState.offset, ReceieveCallbaak, _socketState));
                }
                catch (Exception ex)
                {
                    tcpClient?.Dispose();
                    tcpClient = null;
                }
            }
            catch (Exception)
            {
                tcpClient?.Dispose();
                tcpClient = null;
            }

        }
        private REMOTE_MODE _CurrentREMOTE_MODE_Downloaded = REMOTE_MODE.OFFLINE;
        private REMOTE_MODE CurrentREMOTE_MODE_Downloaded
        {
            get => _CurrentREMOTE_MODE_Downloaded;
            set
            {
                if (_CurrentREMOTE_MODE_Downloaded != value)
                {
                    OnRemoteModeChanged?.Invoke(value, true);
                    _CurrentREMOTE_MODE_Downloaded = value;
                }
            }
        }
        private RETURN_CODE AGVOnlineReturnCode;
        public Task CarrierRemovedRequestAsync(string v, string[] vs)
        {
            throw new NotImplementedException();
        }

        public static MESSAGE_TYPE GetMESSAGE_TYPE(string message_json)
        {

            var _Message = JsonConvert.DeserializeObject<Dictionary<string, object>>(message_json);

            string headerContent = _Message["Header"].ToString();
            var headers = JsonConvert.DeserializeObject<Dictionary<string, object>>(headerContent);

            var firstHeaderKey = headers.Keys.First();

            if (firstHeaderKey.Contains("0101"))
                return MESSAGE_TYPE.REQ_0101_ONLINE_MODE_QUERY;
            if (firstHeaderKey.Contains("0102"))
                return MESSAGE_TYPE.ACK_0102;
            if (firstHeaderKey.Contains("0103"))
                return MESSAGE_TYPE.REQ_0103_ONLINE_MODE_REQUEST;
            if (firstHeaderKey.Contains("0104"))
                return MESSAGE_TYPE.ACK_0104;
            if (firstHeaderKey.Contains("0105"))
                return MESSAGE_TYPE.REQ_0105_RUNNING_STATUS_REPORT;
            if (firstHeaderKey.Contains("0106"))
                return MESSAGE_TYPE.ACK_0106;
            if (firstHeaderKey.Contains("0107"))
                return MESSAGE_TYPE.REQ_0107_AGVS_Online_Req;
            if (firstHeaderKey.Contains("0301"))
                return MESSAGE_TYPE.REQ_0301_TASK_DOWNLOAD;
            if (firstHeaderKey.Contains("0302"))
                return MESSAGE_TYPE.ACK_0302_TASK_DOWNLOADED_ACK;

            if (firstHeaderKey.Contains("0303"))
                return MESSAGE_TYPE.REQ_0303_TASK_FEEDBACK_REPORT;

            if (firstHeaderKey.Contains("0304"))
                return MESSAGE_TYPE.ACK_0304_TASK_FEEDBACK_REPORT_ACK;

            if (firstHeaderKey.Contains("0305"))
                return MESSAGE_TYPE.REQ_0305_TASK_CANCEL;

            if (firstHeaderKey.Contains("0306"))
                return MESSAGE_TYPE.ACK_0306_TASK_CANCEL_ACK;
            if (firstHeaderKey.Contains("0322"))
                return MESSAGE_TYPE.ACK_0322;
            if (firstHeaderKey.Contains("0323"))
                return MESSAGE_TYPE.REQ_0323_VirtualID_Query;
            if (firstHeaderKey.Contains("0324"))
                return MESSAGE_TYPE.ACK_0324_VirtualID_ACK;
            else
                return MESSAGE_TYPE.UNKNOWN;
        }
        public override void Disconnect()
        {
            if (tcpClient != null)
            {
                try
                {

                    tcpClient.Close();
                    tcpClient?.Dispose();
                }
                catch (Exception)
                {

                }
            }
            tcpClient = null;
        }

        public override bool IsConnected()
        {
            if (UseWebAPI)
                return !VMS_API_Call_Fail_Flag;
            if (tcpClient == null)
                return false;
            return tcpClient.Connected;
        }



        public bool WriteDataOut(byte[] dataByte)
        {
            if (!IsConnected())
                return false;
            try
            {
                socketState.stream.Write(dataByte, 0, dataByte.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


        public async Task<bool> SendMsgToAGVSAndWaitReply(byte[] dataByte, int systemBytes)
        {
            if (!IsConnected())
                return false;
            return await Task.Factory.StartNew(() =>
             {
                 try
                 {
                     ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                     socketState.stream.Write(dataByte, 0, dataByte.Length);
                     bool addsucess = WaitAGVSReplyMREDictionary.TryAdd(systemBytes, manualResetEvent);
                     if (addsucess)
                     {
                         manualResetEvent.WaitOne(2000);
                         return true;
                     }
                     else
                     {
                         LOG.Critical($"[WriteDataOut] 將 'ManualResetEvent' 加入 'WaitAGVSReplyMREDictionary' 失敗");
                         return false;
                     }
                 }
                 catch (IOException ioex)
                 {
                     LOG.ERROR($"[AGVS] 發送訊息的過程中發生 IOException : {ioex.Message}", ioex);
                     Disconnect();
                     return false;
                 }
                 catch (Exception ex)
                 {
                     LOG.ERROR($"[AGVS] 發送訊息的過程中發生未知的錯誤 : {ex.Message}", ex);
                     //Disconnect();
                     return false;
                 }

             });

        }

    }
}
