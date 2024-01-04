using AGVSystemCommonNet6.Log;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace AGVSystemCommonNet6.HttpTools
{
    public class clsWebsocktClientHandler
    {
        public delegate object DataFetchDelegate(string ws_path);
        public DataFetchDelegate OnDataFetching;
        public event EventHandler<string> OnClientLeve;
        public string UserID = "";
        public clsWebsocktClientHandler(WebSocket webSocket, string path, string UserID = "")
        {
            WebSocket = webSocket;
            Path = path;
            this.UserID = UserID;
        }

        public WebSocket WebSocket { get; }
        public string Path { get; }

        private byte[] datBytes = new byte[0];
        public async Task StartBrocast()
        {
            if (Path == null)
                return;

            var buff = new ArraySegment<byte>(new byte[10]);
            bool closeFlag = false;
            _ = Task.Factory.StartNew(async () =>
            {
                while (!closeFlag)
                {
                    await Task.Delay(10);
                    var data = OnDataFetching(Path);
                    if (data == null)
                        continue;
                    if (data != null)
                    {
                        try
                        {
                            byte[] _datBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
                            if (!_datBytes.SequenceEqual(datBytes))
                                await WebSocket.SendAsync(new ArraySegment<byte>(_datBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            else
                                await Task.Delay(200);
                            data = null;
                            datBytes = _datBytes;
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
            });

            while (true)
            {
                try
                {
                    await Task.Delay(100);
                    WebSocketReceiveResult result = await WebSocket.ReceiveAsync(buff, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            if (UserID != "")
            {
                OnClientLeve?.Invoke(this, UserID);
            }
            closeFlag = true;
            WebSocket.Dispose();
            GC.Collect();
        }
    }
}
