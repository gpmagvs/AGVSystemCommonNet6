﻿using AGVSystemCommonNet6.Log;
using Newtonsoft.Json;
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
            var buff = new ArraySegment<byte>(new byte[10]);
            while (true)
            {
                try
                {
                    await Task.Delay(1).ConfigureAwait(false);
                    WebSocketReceiveResult result = await WebSocket.ReceiveAsync(buff, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    break;
                }
            }
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
                OnClientDisconnect?.Invoke(this, this);
            }
        }
        internal async void Close()
        {
            try
            {
                await WebSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, "", CancellationToken.None);
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
