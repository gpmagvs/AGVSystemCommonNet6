using AGVSystemCommonNet6.Log;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocket = System.Net.WebSockets.WebSocket;

namespace AGVSystemCommonNet6.HttpTools
{
    public abstract class WebsocketServerMiddleware
    {

        public virtual List<string> channelMaps { get; set; } = new List<string>();

        private Dictionary<string, List<clsWebsocktClientHandler>> ClientsOfAllChannel = new Dictionary<string, List<clsWebsocktClientHandler>>();
        protected Dictionary<string, object> CurrentViewModelDataOfAllChannel = new Dictionary<string, object>();

        public int OnlineClientNumber => ClientsOfAllChannel.First().Value.Count;

        public virtual void Initialize()
        {
            ClientsOfAllChannel = channelMaps.ToDictionary(str => str, str => new List<clsWebsocktClientHandler>());
            CurrentViewModelDataOfAllChannel = channelMaps.ToDictionary(str => str, str => new object());
            StartCollectViewModelDataAndPublishOutAsync();
        }
        private SemaphoreSlim _ClientConnectionChanging = new SemaphoreSlim(1, 1);
        public async Task HandleWebsocketClientConnectIn(HttpContext _context, string user_id = "")
        {
            await _ClientConnectionChanging.WaitAsync();
            string path = _context.Request.Path.Value;
            if (path == null || !_context.WebSockets.IsWebSocketRequest)
            {
                _context.Response.StatusCode = 400;
                return;
            }

            WebSocket client = await _context.WebSockets.AcceptWebSocketAsync();
            clsWebsocktClientHandler clientHander = new clsWebsocktClientHandler(client, path, user_id);

            if (ClientsOfAllChannel.TryGetValue(path, out var clientCollection))
            {
                clientCollection.Add(clientHander);
                clientHander.OnClientDisconnect += ClientHander_OnClientDisconnect;
                if (user_id != "")
                {
                    LOG.TRACE($"User-{user_id} Broswer AGVS Website  | Online-Client={OnlineClientNumber}");
                }

                _ClientConnectionChanging.Release();
                await clientHander.ListenConnection();
            }
        }

        private async void ClientHander_OnClientDisconnect(object? sender, clsWebsocktClientHandler e)
        {
            await _ClientConnectionChanging.WaitAsync();
            var group = ClientsOfAllChannel.FirstOrDefault(kp => kp.Value.Contains(e));
            if (group.Value != null)
            {
                group.Value.Remove(e);
                if (e.UserID != "")
                    LOG.TRACE($"User-{e.UserID} Leave AGVS Website. | Online-Client={OnlineClientNumber}");
                e.Close();
                GC.Collect();
            }
            _ClientConnectionChanging.Release();
        }

        private async void StartCollectViewModelDataAndPublishOutAsync()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(100);
                    await _ClientConnectionChanging.WaitAsync();
                    try
                    {
                        await CollectViewModelData();

                        List<Task> channelTasks = new List<Task>();

                        foreach (KeyValuePair<string, object> item in CurrentViewModelDataOfAllChannel)
                        {
                            channelTasks.Add(ProcessChannelAsync(item));
                        }

                        await Task.WhenAll(channelTasks);

                        async Task ProcessChannelAsync(KeyValuePair<string, object> item)
                        {
                            var ChannelName = item.Key;
                            var clientsInThisChannel = ClientsOfAllChannel[ChannelName];

                            if (clientsInThisChannel.Count == 0)
                                return;

                            var Data = item.Value;
                            var datPublishOut = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Data));

                            List<Task> clientTasks = new List<Task>();

                            foreach (clsWebsocktClientHandler client in clientsInThisChannel)
                            {
                                clientTasks.Add(SendMessageAsync(client, datPublishOut));
                            }

                            await Task.WhenAll(clientTasks);

                            async Task SendMessageAsync(clsWebsocktClientHandler client, byte[] data)
                            {
                                try
                                {
                                    await client.WebSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
                                }
                                catch (Exception ex)
                                {
                                    client.InvokeOnClientDisconnect();
                                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        try
                        {
                            _ClientConnectionChanging.Release();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            });
        }


        protected abstract Task CollectViewModelData();
    }
}
