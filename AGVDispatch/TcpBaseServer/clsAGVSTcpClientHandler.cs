using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Log;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSTcpServer
    {
        public override string alarm_locate_in_name => "AGVS_TCP_Serverb";

        public class clsAGVSTcpClientHandler : IDisposable
        {
            public clsAGVSTcpClientHandler()
            {

            }
            public class clsMsgSendEventArg : EventArgs
            {
                public string Json { get; internal set; }
                public clsAGVSConnection.MESSAGE_TYPE MsgType { get; internal set; }
            }
            private int SystemBytes = 4000;
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
                        return Encoding.UTF8.GetString(buffer, 0, revedDataLen);
                    }
                }

                public bool TryFindEndChar(out int lastIndexOfCR)
                {
                    lastIndexOfCR = revedString.LastIndexOf('*');
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
                    SendMsgWorker();
                    try
                    {
                        Task.Factory.StartNew(() =>
                        {
                            _SocketClient.BeginReceive(ClientSocketState.buffer, 0, 32768, SocketFlags.None, new AsyncCallback(ClientMsgRevCallback), ClientSocketState);
                        });
                    }
                    catch (Exception ex)
                    {
                        OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                        AlarmManagerCenter.AddAlarmAsync(ALARMS.AGV_TCPIP_DISCONNECT);
                        LOG.ERROR($"{ClientIP} {ex.Message}", ex);
                        _SocketClient?.Dispose();
                    }
                }
            }

            private void SendMsgWorker()
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1);
                        if (send_out_queue.TryDequeue(out string json))
                        {
                            try
                            {
                                SocketClient.Send(Encoding.UTF8.GetBytes(json + "*\r"));
                            }
                            catch (Exception ex)
                            {
                                OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                                AlarmManagerCenter.AddAlarmAsync(ALARMS.AGV_TCPIP_DISCONNECT);
                                LOG.ERROR($"{ClientIP} {ex.Message}", ex);
                            }
                        }
                    }
                });
            }

            protected void PingServerCheckProcess()
            {
                Task.Factory.StartNew(async () =>
                {

                    while (_SocketClient != null)
                    {
                        Thread.Sleep(1000);
                        _ping_success = await PingServer();
                        if (!_ping_success)
                        {
                            OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                            _SocketClient?.Dispose();
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

            private async void ClientMsgRevCallback(IAsyncResult ar)
            {
                try
                {
                    clsSocketState ClientSocketState = ar.AsyncState as clsSocketState;
                    int revDataLen = ClientSocketState.socket.EndReceive(ar);
                    ClientSocketState.revedDataLen += revDataLen;
                    string str = ClientSocketState.revedString;

                    if (str.Contains("0105"))
                    {

                    }
                    if (ClientSocketState.TryFindEndChar(out int index))
                    {
                        int remaindData = ClientSocketState.revedDataLen - (index + 1);
                        await HandleClientMsg(ClientSocketState.revedString);
                        ClientSocketState.buffer = new byte[clsSocketState.BufferSize];
                        ClientSocketState.revedDataLen = 0;
                        //}
                    }
                    int offset = ClientSocketState.revedDataLen; //2
                    int toRevLen = clsSocketState.BufferSize - offset;
                    await Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            ClientSocketState.socket.BeginReceive(ClientSocketState.buffer, offset, toRevLen, SocketFlags.None, new AsyncCallback(ClientMsgRevCallback), ClientSocketState);
                        }
                        catch (Exception ex)
                        {
                            OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                            AlarmManagerCenter.AddAlarmAsync(ALARMS.AGV_TCPIP_DISCONNECT);
                            LOG.ERROR($"{ClientIP} {ex.Message}", ex);
                            _SocketClient?.Dispose();
                        }
                    });
                }
                catch (Exception ex)
                {
                    OnTcpSocketDisconnect?.Invoke(this, EventArgs.Empty);
                    AlarmManagerCenter.AddAlarmAsync(ALARMS.AGV_TCPIP_DISCONNECT);
                    LOG.ERROR($"{ClientIP} {ex.Message}", ex);
                    _SocketClient?.Dispose();
                }

            }


            private async Task HandleClientMsg(string clientMsg)
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
                        Console.WriteLine(json);
                        clsTaskFeedbackMessage taskFeedback = JsonConvert.DeserializeObject<clsTaskFeedbackMessage>(json);
                        OnClientTaskFeedback?.Invoke(this, taskFeedback);
                    }

                    if (msgType == clsAGVSConnection.MESSAGE_TYPE.ACK_0302_TASK_DOWNLOADED_ACK)
                    {
                        clsTaskDownloadAckMessage taskDownloadFeedback = JsonConvert.DeserializeObject<clsTaskDownloadAckMessage>(json);
                        if (TaskDownloadMsg.SystemBytes == taskDownloadFeedback.SystemBytes)
                        {
                            var response = taskDownloadFeedback.Header.First().Value;
                            LOG.INFO($"Task Download To {AGV_Name}, AGV Response={response.ToJson()}");
                            taskDownload_AGV_ReturnCode = response.ReturnCode;
                            TaskDownloadWaitMRE.Set();
                        }
                    }


                    if (msgType == clsAGVSConnection.MESSAGE_TYPE.ACK_0306_TASK_CANCEL_ACK)
                    {
                        clsSimpleReturnWithTimestampMessage taskDownloadFeedback = JsonConvert.DeserializeObject<clsSimpleReturnWithTimestampMessage>(json);
                        if (TaskCancelMsg.SystemBytes == taskDownloadFeedback.SystemBytes)
                        {
                            var response = taskDownloadFeedback.Header.First().Value;
                            LOG.INFO($"Task Cancel To {AGV_Name}, AGV Response={response.ToJson()}");
                            taskCancel_AGV_ReturnCode = response.ReturnCode;
                            TaskCancelWaitMRE.Set();
                        }
                    }
                }
            }

            ConcurrentQueue<string> send_out_queue = new ConcurrentQueue<string>();
            public void SendJsonReply(string json)
            {
                send_out_queue.Enqueue(json);
            }
            private RETURN_CODE taskDownload_AGV_ReturnCode;
            private RETURN_CODE taskCancel_AGV_ReturnCode;

            private ManualResetEvent TaskDownloadWaitMRE = new ManualResetEvent(false);
            private ManualResetEvent TaskCancelWaitMRE = new ManualResetEvent(false);
            private clsTaskDownloadMessage TaskDownloadMsg;
            private clsTaskResetReqMessage TaskCancelMsg;

            private bool disposedValue;

            public SimpleRequestResponse SendTaskCancelMessage(Model.clsCancelTaskCmd reset_cmd)
            {
                SystemBytes += 1;
                TaskCancelMsg = new clsTaskResetReqMessage()
                {
                    SystemBytes = SystemBytes,
                    EQName = AGV_Name,
                    Header = new Dictionary<string, TaskResetDto> {
                         {"0305", new TaskResetDto
                         {
                              ResetMode = reset_cmd.ResetMode,
                               Time_Stamp = DateTime.Now.ToAGVSTimeFormat()
                         } }
                     }
                };
                TaskCancelWaitMRE.Reset();
                SendJsonReply(JsonConvert.SerializeObject(TaskCancelMsg));
                bool timeout = !TaskCancelWaitMRE.WaitOne(TimeSpan.FromSeconds(3));
                if (timeout)
                {
                    LOG.WARN($"Task Cancel To {AGV_Name}, Timeout!");
                }
                else
                {
                }
                return new SimpleRequestResponse { ReturnCode = taskCancel_AGV_ReturnCode };
            }

            public TaskDownloadRequestResponse SendTaskMessage(clsTaskDownloadData downloadData)
            {
                SystemBytes += 1;
                TaskDownloadMsg = new clsTaskDownloadMessage()
                {
                    SystemBytes = SystemBytes,
                    EQName = AGV_Name,
                    Header = new Dictionary<string, clsTaskDownloadData>
                             {
                                 { "0301",downloadData }
                             },
                };
                TaskDownloadWaitMRE.Reset();
                SendJsonReply(JsonConvert.SerializeObject(TaskDownloadMsg));
                LOG.INFO($"Task Download To {AGV_Name}, Wait Response...");
                bool timeout = !TaskDownloadWaitMRE.WaitOne(TimeSpan.FromSeconds(3));
                if (timeout)
                {
                    LOG.WARN($"Task Download To {AGV_Name}, Timeout!");
                    return new TaskDownloadRequestResponse
                    {
                        ReturnCode = TASK_DOWNLOAD_RETURN_CODES.TASK_DOWN_LOAD_TIMEOUT
                    };
                }
                else
                {
                    if (taskDownload_AGV_ReturnCode == RETURN_CODE.OK)
                    {
                        LOG.INFO($"Task Download To {AGV_Name}, AGV Accept!");
                        return new TaskDownloadRequestResponse { ReturnCode = TASK_DOWNLOAD_RETURN_CODES.OK };
                    }
                    else
                    {
                        LOG.WARN($"Task Download To {AGV_Name}, AGV Return Code == {taskDownload_AGV_ReturnCode}");
                        return new TaskDownloadRequestResponse
                        {
                            ReturnCode = TASK_DOWNLOAD_RETURN_CODES.TASK_CANCEL
                        };
                    }
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: 處置受控狀態 (受控物件)
                    }

                    // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                    // TODO: 將大型欄位設為 Null
                    disposedValue = true;
                }
            }

            // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
            // ~clsAGVSTcpClientHandler()
            // {
            //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
