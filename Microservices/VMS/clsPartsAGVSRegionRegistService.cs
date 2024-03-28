using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.VMS
{
    public class clsPartsAGVSRegionRegistService
    {
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

        public clsPartsAGVSRegionRegistService() { }
        public clsPartsAGVSRegionRegistService(string serivceIP, int serivcePort)
        {
            this.IP = serivceIP;
            this.Port = serivcePort;
        }

        public async Task<(bool accept, string message)> Regist(string AGVName, List<string> AreaNames)
        {
            RegistEventObject obj = new RegistEventObject()
            {
                AGVName = AGVName,
                List_AreaName = AreaNames,
                RegistEventEnum = RegistEventObject.REGIST_ACTION.Regist
            };
            return await SendToPartsAGVS(obj);
        }
        public async Task<(bool accept, string message)> Unregist(string AGVName, List<string> AreaNames)
        {
            RegistEventObject obj = new RegistEventObject()
            {
                AGVName = AGVName,
                List_AreaName = AreaNames,
                RegistEventEnum = RegistEventObject.REGIST_ACTION.Unregist
            };
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

            Dictionary<string, string> OutputData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(result.message);
            return (true, OutputData);
        }
        private async Task<(bool accept, string message)> SendToPartsAGVS(RegistEventObject data_obj)
        {
            if (string.IsNullOrEmpty(IP))
                return (false, "IP format Illeagle");
            if (data_obj.RegistEventEnum != RegistEventObject.REGIST_ACTION.Query && data_obj.List_AreaName.Count == 0)
                return (false, "Regist/Unregist Area Names can't empty");

            var ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            try
            {
                ClientSocket.Connect(IP, Port);
            }
            catch (Exception ex)
            {
                ClientSocket.Dispose();
                return (false, ex.Message);
            }

            string SendOutMessage = Newtonsoft.Json.JsonConvert.SerializeObject(data_obj);
            ClientSocket.Send(Encoding.ASCII.GetBytes(SendOutMessage));
            CancellationTokenSource cancelwait = new CancellationTokenSource();
            cancelwait.CancelAfter(TimeSpan.FromSeconds(5));
            string ReceiveDataString = "";
            while (true)
            {
                await Task.Delay(1);
                if (cancelwait.IsCancellationRequested)
                {
                    ClientSocket.Dispose();
                    return (false, "Timeout");
                }
                if (ClientSocket.Available == 0)
                {
                    continue;
                }
                else
                {
                    byte[] buffer = new byte[ClientSocket.Available];
                    ClientSocket.Receive(buffer);
                    ReceiveDataString += Encoding.ASCII.GetString(buffer);
                    if (ReceiveDataString == "OK" || ReceiveDataString == "NG")
                    {
                        break;
                    }
                }
            }
            bool isPartsAGVSAccept = ReceiveDataString.ToUpper() != "NG";
            string region_names_str = string.Join("", data_obj.List_AreaName);
            return (isPartsAGVSAccept, isPartsAGVSAccept ? $"Parts AGVS Accept {data_obj.RegistEvent} [{region_names_str}]" : $"Parts AGVS Reject {data_obj.RegistEvent} [{region_names_str}]");
        }
    }
}
