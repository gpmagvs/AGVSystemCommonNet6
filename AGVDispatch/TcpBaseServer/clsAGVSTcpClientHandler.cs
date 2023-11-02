using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Log;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSTcpServer
    {
        public override string alarm_locate_in_name => "AGVS_TCP_Serverb";

        public class clsAGVSTcpClientHandler
        {
            public class clsMsgSendEventArg : EventArgs
            {
                public string Json { get; internal set; }
                public clsAGVSConnection.MESSAGE_TYPE MsgType { get; internal set; }
            }

            private Socket _SocketClient = null;
            private bool _ping_success = false;
            public string ClientIP { get; private set; } = "";
            public string AGV_Name { get; private set; } = "";
            public clsSocketState ClientSocketState { get; set; } = new clsSocketState();
            public event EventHandler<clsMsgSendEventArg> OnClientMsgSendIn;
            public event EventHandler<clsOnlineModeQueryMessage> OnClientOnlineModeQuery;
            public event EventHandler<clsOnlineModeRequestMessage> OnClientOnlineRequesting;
            public event EventHandler<clsRunningStatusReportMessage> OnClientRunningStatusReport;
            public event EventHandler<clsTaskFeedbackMessage> OnClientTaskFeedback;
            public event EventHandler OnTcpSocketDisconnect;
            public class clsSocketState
            {
                public Socket socket;
                public byte[] buffer = new byte[BufferSize];
                public const int BufferSize = 32768;
                public int revedDataLen = 0;
                public string revedString
                {
                    get
                    {
                        return Encoding.ASCII.GetString(buffer, 0, revedDataLen);
                    }
                }

                public bool TryFindEndChar(out int lastIndexOfCR)
                {
                    lastIndexOfCR = revedString.LastIndexOf('\r');
                    if (lastIndexOfCR < 0)
                        return false;
                    return true;
                }

            }
            public Socket SocketClient
            {
                get => _SocketClient;
                set
                {
                    _SocketClient = value;
                    var endpoint = _SocketClient?.RemoteEndPoint.ToString();
                    ClientIP = endpoint.Split(':')[0];
                    ClientSocketState = new clsSocketState { socket = value };
                    PingServerCheckProcess();
                    try
                    {
                        _SocketClient.BeginReceive(ClientSocketState.buffer, 0, 32768, SocketFlags.None, new AsyncCallback(ClientMsgRevCallback), ClientSocketState);
                    }
                    catch (Exception ex)
                    {
                        OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                        AlarmManagerCenter.AddAlarm(ALARMS.AGV_TCPIP_DISCONNECT);
                        LOG.ERROR($"{ClientIP} {ex.Message}", ex);
                        _SocketClient.Dispose();
                    }
                }
            }
            protected void PingServerCheckProcess()
            {
                Task.Run(async () =>
                {

                    while (_SocketClient != null)
                    {
                        await Task.Delay(1000);
                        _ping_success = await PingServer();
                        if (!_ping_success)
                        {
                            OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                            _SocketClient.Dispose();
                            _SocketClient = null;
                        }
                    }
                });
            }

            public async Task<bool> PingServer()
            {
                Ping pingSender = new Ping();
                // 設定要 ping 的主機或 IP
                string address = ClientIP;
                try
                {
                    PingReply reply = pingSender.Send(address);
                    if (reply.Status != IPStatus.Success)
                    {
                        Console.WriteLine(reply.Status);
                    }
                    return reply.Status == IPStatus.Success;
                }
                catch (PingException ex)
                {
                    Console.WriteLine($"Ping Error: {ex.Message}");
                    return false;
                }
            }

            private void ClientMsgRevCallback(IAsyncResult ar)
            {
                try
                {
                    clsSocketState ClientSocketState = ar.AsyncState as clsSocketState;
                    int revDataLen = ClientSocketState.socket.EndReceive(ar);
                    ClientSocketState.revedDataLen += revDataLen;
                    string str = ClientSocketState.revedString;
                    if (ClientSocketState.TryFindEndChar(out int index))
                    {
                        int remaindData = ClientSocketState.revedDataLen - (index + 1);
                        //0 =>整包都是完整資料
                        if (remaindData != 0)
                        {
                            var newBuffer = new byte[clsSocketState.BufferSize];
                            Array.Copy(ClientSocketState.buffer, index + 1, newBuffer, 0, remaindData);
                            ClientSocketState.buffer = newBuffer;
                            ClientSocketState.revedDataLen = remaindData;
                        }
                        else
                        {
                            HandleClientMsg(ClientSocketState.revedString);
                            ClientSocketState.buffer = new byte[clsSocketState.BufferSize];
                            ClientSocketState.revedDataLen = 0;
                        }
                    }
                    int offset = ClientSocketState.revedDataLen; //2
                    int toRevLen = clsSocketState.BufferSize - offset;
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            ClientSocketState.socket.BeginReceive(ClientSocketState.buffer, offset, toRevLen, SocketFlags.None, new AsyncCallback(ClientMsgRevCallback), ClientSocketState);
                        }
                        catch (Exception ex)
                        {
                            OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                            AlarmManagerCenter.AddAlarm(ALARMS.AGV_TCPIP_DISCONNECT);
                            LOG.ERROR($"{ClientIP} {ex.Message}", ex);
                            _SocketClient.Dispose();
                        }
                    });
                }
                catch (Exception ex)
                {
                    OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                    AlarmManagerCenter.AddAlarm(ALARMS.AGV_TCPIP_DISCONNECT);
                    LOG.ERROR($"{ClientIP} {ex.Message}", ex);
                    _SocketClient.Dispose();
                }

            }

            private async Task HandleClientMsg(string clientMsg)
            {
                //"55688abc*\r"
                _ = Task.Factory.StartNew(() =>
                {
                    string[] splited = clientMsg.Replace("*\r", "$").Split('$');
                    List<string> jsonMsgLs = splited.ToList().FindAll(x => x != "");

                    foreach (var json in jsonMsgLs)
                    {
                        clsAGVSConnection.MESSAGE_TYPE msgType = clsAGVSConnection.GetMESSAGE_TYPE(json);

                        if (msgType == clsAGVSConnection.MESSAGE_TYPE.REQ_0101_ONLINE_MODE_QUERY) //詢問上線狀態
                        {
                            clsOnlineModeQueryMessage? onlineModeQu = JsonConvert.DeserializeObject<clsOnlineModeQueryMessage>(json);
                            AGV_Name = onlineModeQu.EQName;
                            OnClientOnlineModeQuery?.Invoke(this, onlineModeQu);
                        }
                        if (msgType == clsAGVSConnection.MESSAGE_TYPE.REQ_0103_ONLINE_MODE_REQUEST) //要求上/下線
                        {
                            clsOnlineModeRequestMessage onlineRequest = JsonConvert.DeserializeObject<clsOnlineModeRequestMessage>(json);
                            OnClientOnlineRequesting?.Invoke(this, onlineRequest);
                        }

                        if (msgType == clsAGVSConnection.MESSAGE_TYPE.REQ_0105_RUNNING_STATUS_REPORT) //狀態上報
                        {
                            clsRunningStatusReportMessage runningStatus = JsonConvert.DeserializeObject<clsRunningStatusReportMessage>(json);
                            OnClientRunningStatusReport?.Invoke(this, runningStatus);
                        }

                        if (msgType == clsAGVSConnection.MESSAGE_TYPE.REQ_0303_TASK_FEEDBACK_REPORT) //任務狀態上報
                        {
                            clsTaskFeedbackMessage taskFeedback = JsonConvert.DeserializeObject<clsTaskFeedbackMessage>(json);
                            OnClientTaskFeedback?.Invoke(this, taskFeedback);
                        }
                    }

                });
            }

            public void SendJsonReply(string json)
            {
                try
                {
                    SocketClient.Send(Encoding.ASCII.GetBytes(json + "*\r"));
                }
                catch (Exception ex)
                {
                    OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                    AlarmManagerCenter.AddAlarm(ALARMS.AGV_TCPIP_DISCONNECT);
                    LOG.ERROR($"{ClientIP} {ex.Message}", ex);
                }
            }


        }
    }
}
