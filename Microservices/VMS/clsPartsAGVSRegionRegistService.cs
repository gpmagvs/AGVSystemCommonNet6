using Newtonsoft.Json;
using NLog;
using System.Net.Sockets;
using System.Text;

namespace AGVSystemCommonNet6.Microservices.VMS
{
    public class clsPartsAGVSRegionRegistService : IDisposable
    {
        private bool disposedValue;

        public class RegistEventObject
        {
            public enum REGIST_ACTION
            {
                Regist,
                Unregist,
                Query
            }
            public string AGVName { get; set; } = "External";
            public List<string> List_AreaName { get; set; } = new List<string>();
            public string RegistEvent => RegistEventEnum.ToString();

            [NonSerialized]
            public REGIST_ACTION RegistEventEnum = REGIST_ACTION.Regist;
        }
        public string IP { get; set; } = "192.168.0.100";
        public int Port { get; set; } = 5000;
        private Encoding encoding = Encoding.GetEncoding("big5");
        public clsPartsAGVSRegionRegistService() { }
        Logger logger = LogManager.GetCurrentClassLogger();

        public clsPartsAGVSRegionRegistService(string serivceIP, int serivcePort)
        {
            this.IP = serivceIP;
            this.Port = serivcePort;
        }

        public async Task<(bool accept, string message, string responseJson)> Regist(string AGVName, List<string> AreaNames)
        {
            RegistEventObject obj = new RegistEventObject()
            {
                AGVName = AGVName,
                List_AreaName = AreaNames,
                RegistEventEnum = RegistEventObject.REGIST_ACTION.Regist
            };
            return await SendToPartsAGVS(obj);
        }
        public async Task<(bool accept, string message, string responseJson)> Unregist(string AGVName, List<string> AreaNames)
        {
            RegistEventObject obj = new RegistEventObject()
            {
                AGVName = AGVName,
                List_AreaName = AreaNames,
                RegistEventEnum = RegistEventObject.REGIST_ACTION.Unregist
            };
            logger.Warn($"Unregist request send to Parts System : {obj.ToJson()}");
            return await SendToPartsAGVS(obj);
        }
        public async Task<(bool accept, Dictionary<string, string>)> Query()
        {
            RegistEventObject obj = new RegistEventObject()
            {
                AGVName = "",
                RegistEventEnum = RegistEventObject.REGIST_ACTION.Query
            };
            var result = await SendToPartsAGVS(obj);
            if (!result.accept)
                return (false, new Dictionary<string, string>());

            Dictionary<string, string> OutputData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(result.responseJsonMsg);
            logger.Trace($"Regist information from Parts System:{OutputData.ToJson()}");
            return (true, OutputData);
        }
        private Socket _socketClient;
        private async Task<(bool accept, string message, string responseJsonMsg)> SendToPartsAGVS(RegistEventObject data_obj)
        {
            if (string.IsNullOrEmpty(IP))
                return (false, "IP format Illeagle", "");
            if (data_obj.RegistEventEnum != RegistEventObject.REGIST_ACTION.Query && data_obj.List_AreaName.Count == 0)
                return (false, "Regist/Unregist Area Names can't empty", "");
            data_obj.List_AreaName = data_obj.List_AreaName.Select(areaName => areaName.TrimStart()).Select(areaName => areaName.TrimEnd()).ToList();
            Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = 8000,
                ReceiveTimeout = 8000
            };
            _socketClient = ClientSocket;
            ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            try
            {
                ClientSocket.Connect(IP, Port);
            }
            catch (Exception ex)
            {
                ClientSocket.Dispose();
                return (false, ex.Message, "");
            }

            string SendOutMessage = Newtonsoft.Json.JsonConvert.SerializeObject(data_obj);
            ClientSocket.Send(encoding.GetBytes(SendOutMessage));
            CancellationTokenSource cancelwait = new CancellationTokenSource();
            cancelwait.CancelAfter(TimeSpan.FromSeconds(8));
            string ReceiveDataString = "";
            while (true)
            {
                await Task.Delay(1);
                if (cancelwait.IsCancellationRequested)
                {
                    ClientSocket.Dispose();
                    return (false, "Timeout", "");
                }
                if (ClientSocket.Available == 0)
                {
                    continue;
                }
                else
                {
                    byte[] buffer = new byte[ClientSocket.Available];
                    ClientSocket.Receive(buffer);
                    var _revStr = encoding.GetString(buffer);
                    logger.Trace($"[{data_obj.RegistEventEnum}]:{_revStr}");
                    ReceiveDataString += _revStr;
                    if (data_obj.RegistEventEnum != RegistEventObject.REGIST_ACTION.Query)
                    {
                        if (ReceiveDataString == "OK" || ReceiveDataString == "NG")
                            break;
                    }
                    else
                    {
                        try
                        {
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(ReceiveDataString);
                            break;
                        }
                        catch (Exception ex)
                        {
                            logger.Warn(ex.Message);
                            continue;
                        }
                    }
                }
            }

            ClientSocket.Dispose();
            bool isPartsAGVSAccept = data_obj.RegistEventEnum == RegistEventObject.REGIST_ACTION.Query ? true : ReceiveDataString.ToUpper() != "NG";
            string region_names_str = string.Join("", data_obj.List_AreaName);
            return (
                 isPartsAGVSAccept,
                isPartsAGVSAccept ? $"Parts AGVS Accept {data_obj.RegistEvent} [{region_names_str}]" : $"Parts AGVS Reject {data_obj.RegistEvent} [{region_names_str}]",
                ReceiveDataString
                );
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                }
                try
                {
                    _socketClient?.Disconnect(true);
                }
                catch (Exception)
                {
                }
                finally
                {
                    _socketClient?.Dispose();
                }
                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~clsPartsAGVSRegionRegistService()
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
