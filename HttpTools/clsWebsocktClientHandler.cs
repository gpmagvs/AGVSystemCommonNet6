using AGVSystemCommonNet6.Log;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;

namespace AGVSystemCommonNet6.HttpTools
{
    public class clsWebsocktClientHandler
    {
        public event EventHandler<clsWebsocktClientHandler> OnClientDisconnect;
        public string UserID = "";
        public clsWebsocktClientHandler(WebSocket webSocket, string path, string UserID = "")
        {
            WebSocket = webSocket;
            Path = path;
            this.UserID = UserID;
        }

        public WebSocket WebSocket { get; }
        public string Path { get; }

        internal async Task ListenConnection()
        {
            var buff = new ArraySegment<byte>(new byte[4]);
            while (WebSocket.State == WebSocketState.Open)
            {
                await Task.Delay(100);
                try
                {
                    var result = await WebSocket.ReceiveAsync(buff, CancellationToken.None).ConfigureAwait(false);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Close();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
            Console.WriteLine(WebSocket.State);
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                WebSocket.Dispose();
                OnClientDisconnect?.Invoke(this, this);
            }
        }
        internal async void Close()
        {
            try
            {
                await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "backend close", CancellationToken.None);
            }
            catch (Exception)
            {
            }
            finally
            {
                WebSocket.Dispose();
            }
        }

        internal void InvokeOnClientDisconnect()
        {
            OnClientDisconnect(this, this);
        }
    }
}
