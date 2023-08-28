using AGVSystemCommonNet6.AGVDispatch.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
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
            public clsSocketState ClientSocketState { get; set; } = new clsSocketState();
            public event EventHandler<clsMsgSendEventArg> OnClientMsgSendIn;
            public event EventHandler<clsOnlineModeQueryMessage> OnClientOnlineModeQuery;
            public event EventHandler<clsOnlineModeRequestMessage> OnClientOnlineRequesting;
            public event EventHandler<clsRunningStatusReportMessage> OnClientRunningStatusReport;
            public event EventHandler<clsTaskFeedbackMessage> OnClientTaskFeedback;
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
                    ClientSocketState = new clsSocketState { socket = value };
                    _SocketClient.BeginReceive(ClientSocketState.buffer, 0, 32768, SocketFlags.None, new AsyncCallback(ClientMsgRevCallback), ClientSocketState);
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
                        ClientSocketState.socket.BeginReceive(ClientSocketState.buffer, offset, toRevLen, SocketFlags.None, new AsyncCallback(ClientMsgRevCallback), ClientSocketState);
                    });
                }
                catch (Exception ex)
                {
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
                    throw ex;
                }
                //*\r
            }

            public string ClientIP
            {
                get
                {
                    var endpoint = SocketClient?.RemoteEndPoint.ToString();
                    return endpoint.Split(':')[0];
                }
            }
        }
    }
}
