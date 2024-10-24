using AGVSystemCommonNet6.Abstracts;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using Microsoft.Extensions.Logging;
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
        public ConcurrentDictionary<int, ManualResetEvent> WaitAGVSReplyMREDictionary = new ConcurrentDictionary<int, ManualResetEvent>();
        ConcurrentDictionary<int, MessageBase> AGVSMessageStoreDictionary = new ConcurrentDictionary<int, MessageBase>();
        bool VMS_API_Call_Fail_Flag = true;

        public delegate clsRunningStatus GetRunningDataUseWebAPIProtocolDelegate();
        public delegate RunningStatus GetRunningDataUseTCPIPProtocolDelegate();
        public delegate TASK_DOWNLOAD_RETURN_CODES taskDonwloadExecuteDelage(clsTaskDownloadData taskDownloadData);
        public delegate bool onlineModeChangeDelelage(REMOTE_MODE mode, bool isAGVSRequest);
        public delegate Task<bool> taskResetReqDelegate(RESET_MODE reset_data, bool isNormal);

        public GetRunningDataUseWebAPIProtocolDelegate OnWebAPIProtocolGetRunningStatus;
        public GetRunningDataUseTCPIPProtocolDelegate OnTcpIPProtocolGetRunningStatus;

        public event EventHandler<clsTaskDownloadData> OnTaskDownloadFeekbackDone;
        public event EventHandler OnConnectionRestored;

        public event EventHandler OnOnlineModeQuery_T1Timeout;
        public event EventHandler OnOnlineModeQuery_Recovery;
        public event EventHandler OnRunningStatusReport_T1Timeout;
        public event EventHandler<FeedbackData> OnTaskFeedBack_T1Timeout;

        public taskDonwloadExecuteDelage OnTaskDownload;
        public onlineModeChangeDelelage OnRemoteModeChanged;
        public taskResetReqDelegate OnTaskResetReq;
        public event EventHandler OnDisconnected;
        public bool IsGetOnlineModeTrying = false;
        public bool UseWebAPI = false;
        private bool _Connected = false;
        public string SID { get; set; } = "001:001:001";
        public string EQName { get; set; } = "AGV_1";

        public bool Connected
        {
            get => _Connected;
            private set
            {
                if (_Connected != value)
                {
                    if (value)
                        OnConnectionRestored?.Invoke(this, EventArgs.Empty);
                    else if (!IsGetOnlineModeTrying)
                    {
                        OnDisconnected?.Invoke(this, EventArgs.Empty);
                    }
                    _Connected = value;
                }
            }
        }

        private bool _IsOnlineModeQueryTimeout = false;
        public bool IsOnlineModeQueryTimeout
        {
            get => _IsOnlineModeQueryTimeout;
            set
            {
                if (_IsOnlineModeQueryTimeout != value)
                {
                    _IsOnlineModeQueryTimeout = value;
                    if (value)
                        OnOnlineModeQuery_T1Timeout?.Invoke(this, EventArgs.Empty);
                    else
                        OnOnlineModeQuery_Recovery?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public ILogger<clsAGVSConnection> logger;

        public enum MESSAGE_TYPE
        {
            REQ_0101_ONLINE_MODE_QUERY = 0101,
            OnlineMode_Query_ACK_0102 = 0102,
            REQ_0103_ONLINE_MODE_REQUEST = 0103,
            OnlineMode_Change_ACK_0104 = 0104,
            REQ_0105_RUNNING_STATUS_REPORT = 0105,
            RunningStateReport_ACK_0106 = 0106,
            REQ_0107_AGVS_Online_Req = 0107,
            ACK_0107_AGVS_Online_Req = 0108,
            REQ_0301_TASK_DOWNLOAD = 0301,
            ACK_0302_TASK_DOWNLOADED_ACK = 0302,
            REQ_0303_TASK_FEEDBACK_REPORT = 0303,
            ACK_0304_TASK_FEEDBACK_REPORT_ACK = 0304,
            REQ_0305_TASK_CANCEL = 0305,
            ACK_0306_TASK_CANCEL_ACK = 0306,
            REQ_0311_EXIT_REQUEST = 0311,
            ACK_0312_EXIT_REQUEST_ACK = 0312,
            REQ_0313_EXIT_RESPONSE = 0313,
            ACK_0314_EXIT_RESPONSE = 0314,
            Carrier_Remove_Request_0321 = 0321,
            Carrier_Remove_Request_ACK_0322 = 0322,
            REQ_0323_VirtualID_Query = 0323,
            ACK_0324_VirtualID_ACK = 0324,
            UNKNOWN = 9999,
        }
        public string LocalIP { get; set; }
        public AGV_TYPE AGV_Type { get; }

        public int msgHsDuration { get; private set; } = 200;

        public clsAGVSConnection(string IP, int Port, bool AutoPingServerCheck = true) : base(IP, Port, AutoPingServerCheck)
        {
            this.IP = IP;
            this.VMSPort = Port;
            LocalIP = null;
            AGVsWebAPIHttp = new HttpTools.HttpHelper($"http://{IP}:{AGVsPort}");
        }
        public clsAGVSConnection(string HostIP, int HostPort, string localIP, AGV_TYPE AGV_TYPE = AGV_TYPE.FORK, ILogger<clsAGVSConnection> logger = null, int msgHsDuration = 200)
        {
            this.logger = logger;
            this.IP = HostIP;
            this.VMSPort = HostPort;
            this.LocalIP = localIP;
            this.AGV_Type = AGV_TYPE;
            this.msgHsDuration = msgHsDuration;
            InitVMSWebAPIHttpChannels($"http://{IP}:{VMSPort}");
            AGVsWebAPIHttp = new HttpTools.HttpHelper($"http://{IP}:{AGVsPort}");
            AutoPingServerCheck = true;
            pingTimeoutInMillSecond = 20000;
            PingServerCheckProcess();
            AGVSMessageFactory.OnCylicSystemByteCreate += (_crteated_systemByte) =>
            {
                bool _is_systembyte_used = WaitAGVSReplyMREDictionary.ContainsKey(_crteated_systemByte);
                return !_is_systembyte_used;
            };
        }


        public void Setup(string _SID, string _EQName)
        {
            SID = _SID;
            EQName = _EQName;
        }

        public override async Task<bool> Connect()
        {
            try
            {
                if (UseWebAPI)
                    return true;

                if (LocalIP != null)
                {
                    IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Parse(LocalIP), 0);
                    try
                    {
                        tcpClient = new TcpClient(ipEndpoint);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"[AGVS] Connect Fail..本地網卡IP設定錯誤-{ipEndpoint.Address.ToString()} 不可用", true);
                        tcpClient = null;
                        await Task.Delay(3000);
                        return false;
                    }
                    tcpClient.ReceiveBufferSize = 65535;
                    tcpClient.Connect(IP, VMSPort);
                }
                else
                {
                    tcpClient = new TcpClient();
                    tcpClient.Connect(IP, VMSPort);
                }
                socketState.stream = tcpClient.GetStream();
                socketState.Reset();
                socketState.stream.BeginRead(socketState.buffer, socketState.offset, clsSocketState.buffer_size - socketState.offset, ReceieveCallbaak, socketState);
                logger.LogInformation($"[AGVS] Connect To AGVS Success !!");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[AGVS] Connect Fail..{ex.Message}. Can't Connect To AGVS ({IP}:{VMSPort})..Will Retry it after 3 secoond...");
                tcpClient = null;
                await Task.Delay(3000);
                return false;
            }
        }

        public async Task Start()
        {
            await Task.Delay(1);
            while (true)
            {
                try
                {
                    if (!UseWebAPI)
                        CheckAndClearOlderMessageStored();
                    if (!IsConnected())
                    {
                        if (!UseWebAPI)
                        {
                            await Task.Delay(100);
                            logger.LogWarning($"Try Connect TO AGVS Via TCP/IP({IP}:{VMSPort})");
                            bool Reconnected = await Connect();
                            Connected = Reconnected;
                            continue;
                        }
                    }
                    IsGetOnlineModeTrying = false;

                    if (await OnlineModeQueryOut())
                    {
                        if (UseWebAPI)
                            VMS_API_Call_Fail_Flag = false;
                        await RunningStatusReport();
                    }
                    else
                    {
                        if (!UseWebAPI)
                            Disconnect();
                        await Task.Delay(1000);
                    }

                }
                catch (Exception ex)
                {
                    OnOnlineModeQuery_T1Timeout?.Invoke(this, EventArgs.Empty);
                    await Task.Delay(1000);
                }
                finally
                {
                    await Task.Delay(msgHsDuration);
                }
            }
        }

        private async Task<bool> OnlineModeQueryOut()
        {
            (bool, OnlineModeQueryResponse onlineModeQuAck) _OnlineModeQueryResult = await TryOnlineModeQueryAsync(20);
            if (!_OnlineModeQueryResult.Item1)
            {
                IsOnlineModeQueryTimeout = true;
                Current_Warning_Code = AlarmCodes.OnlineModeQuery_T1_Timeout;
                logger.LogWarning("[AGVS] OnlineMode Query Fail...AGVS No Response");
                return false;
            }
            else
            {
                IsOnlineModeQueryTimeout = false;
                IsGetOnlineModeTrying = false;
                Connected = true;
                Current_Warning_Code = AlarmCodes.None;
                return true;
            }
        }

        private async Task RunningStatusReport()
        {
            (bool, SimpleRequestResponseWithTimeStamp runningStateReportAck) _runningStateReport_result = await TryRnningStateReportAsync(8);
            if (!_runningStateReport_result.Item1)
            {
                Current_Warning_Code = AlarmCodes.RunningStatusReport_T1_Timeout;
                logger.LogWarning("[AGVS] Running State Report Fail...AGVS No Response");
                OnRunningStatusReport_T1Timeout?.Invoke(this, EventArgs.Empty);
            }
            else
                Current_Warning_Code = AlarmCodes.None;
        }
        private void CheckAndClearOlderMessageStored()
        {
            try
            {
                IEnumerable<KeyValuePair<int, MessageBase>> allOldderMessage = AGVSMessageStoreDictionary.Where(kp => (DateTime.Now - kp.Value.createdTime).TotalSeconds >= 5);
                if (allOldderMessage.Any())
                {
                    int count = allOldderMessage.Count();
                    foreach (KeyValuePair<int, MessageBase> item in allOldderMessage)
                    {
                        AGVSMessageStoreDictionary.TryRemove(item.Key, out _);
                        item.Value.Dispose();
                    }
                    logger.LogTrace($"Find {count} old message, remove from AGVSMessageStoreDictionary ");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
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
                        if (str == "" || str == null || str == "\r")
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
                    Task.Factory.StartNew(() => { OnRemoteModeChanged?.Invoke(value, true); });
                    _CurrentREMOTE_MODE_Downloaded = value;
                }
            }
        }
        private RETURN_CODE AGVOnlineReturnCode;

        public static MESSAGE_TYPE GetMESSAGE_TYPE(string message_json)
        {
            Dictionary<string, object>? _Message = new Dictionary<string, object>();
            try
            {
                _Message = JsonConvert.DeserializeObject<Dictionary<string, object>>(message_json);
            }
            catch (Exception ex)
            {
                return MESSAGE_TYPE.UNKNOWN;
            }

            string headerContent = _Message["Header"].ToString();
            var headers = JsonConvert.DeserializeObject<Dictionary<string, object>>(headerContent);

            var firstHeaderKey = headers.Keys.First();

            if (firstHeaderKey.Contains("0101"))
                return MESSAGE_TYPE.REQ_0101_ONLINE_MODE_QUERY;
            if (firstHeaderKey.Contains("0102"))
                return MESSAGE_TYPE.OnlineMode_Query_ACK_0102;
            if (firstHeaderKey.Contains("0103"))
                return MESSAGE_TYPE.REQ_0103_ONLINE_MODE_REQUEST;
            if (firstHeaderKey.Contains("0104"))
                return MESSAGE_TYPE.OnlineMode_Change_ACK_0104;
            if (firstHeaderKey.Contains("0105"))
                return MESSAGE_TYPE.REQ_0105_RUNNING_STATUS_REPORT;
            if (firstHeaderKey.Contains("0106"))
                return MESSAGE_TYPE.RunningStateReport_ACK_0106;
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

            if (firstHeaderKey.Contains("0311"))
                return MESSAGE_TYPE.REQ_0311_EXIT_REQUEST;
            if (firstHeaderKey.Contains("0312"))
                return MESSAGE_TYPE.ACK_0312_EXIT_REQUEST_ACK;

            if (firstHeaderKey.Contains("0313"))
                return MESSAGE_TYPE.REQ_0313_EXIT_RESPONSE;
            if (firstHeaderKey.Contains("0314"))
                return MESSAGE_TYPE.ACK_0314_EXIT_RESPONSE;

            if (firstHeaderKey.Contains("0322"))
                return MESSAGE_TYPE.Carrier_Remove_Request_ACK_0322;
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
            Connected = false;
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
                logger.LogTrace($"(TCP/IP) {dataByte.GetString(Encoding.ASCII)}");
                socketState.stream.Write(dataByte, 0, dataByte.Length);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> SendMsgToAGVSAndWaitReply(byte[] dataByte, int systemBytes, MESSAGE_TYPE ack_msg_type, int timeout_sec = 8)
        {
            if (!IsConnected())
                return false;

            try
            {
                if (ack_msg_type == MESSAGE_TYPE.ACK_0312_EXIT_REQUEST_ACK)
                {

                }
                ManualResetEvent manualResetEvent = WaitAckResetEvents[ack_msg_type];
                manualResetEvent.Reset();
                WriteDataOut(dataByte);
                bool _recieve_signal = manualResetEvent.WaitOne(TimeSpan.FromSeconds(timeout_sec));
                return _recieve_signal;
            }
            catch (IOException ioex)
            {
                logger.LogError(ioex, $"[AGVS] 發送訊息的過程中發生 IOException : {ioex.Message}");
                Disconnect();
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[AGVS] 發送訊息的過程中發生未知的錯誤 : {ex.Message}");
                return false;
            }
        }
    }
}
