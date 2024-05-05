using AGVSystemCommonNet6.Log;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SQLitePCL;
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

        protected Dictionary<string, List<clsWebsocktClientHandler>> ClientsOfAllChannel = new Dictionary<string, List<clsWebsocktClientHandler>>();
        protected Dictionary<string, object> CurrentViewModelDataOfAllChannel = new Dictionary<string, object>();

        public int OnlineClientNumber => ClientsOfAllChannel.First().Value.Count;

        protected bool Initializd = false;

        public readonly int publish_duration = 0;

        public WebsocketServerMiddleware(int publish_duration = 100)
        {
            this.publish_duration = publish_duration;
        }

        public virtual void Initialize()
        {
            ClientsOfAllChannel = channelMaps.ToDictionary(str => str, str => new List<clsWebsocktClientHandler>());
            CurrentViewModelDataOfAllChannel = channelMaps.ToDictionary(str => str, str => new object());
            StartCollectViewModelDataAndPublishOutAsync();
        }
        private SemaphoreSlim _ClientConnectionChanging = new SemaphoreSlim(1, 2);
        public async Task HandleWebsocketClientConnectIn(HttpContext _context, string user_id = "")
        {
            try
            {
                string path = _context.Request.Path.Value;
                if (path == null || !_context.WebSockets.IsWebSocketRequest)
                {
                    _context.Response.StatusCode = 400;
                    return;
                }

                LOG.TRACE($"User-{path} ");
                if (!this.Initializd)
                {
                    _context.Response.StatusCode = 400;
                    return;
                }
                WebSocket client = await _context.WebSockets.AcceptWebSocketAsync();
                clsWebsocktClientHandler clientHander = new clsWebsocktClientHandler(client, path, user_id);
                LOG.TRACE($"User Broswer AGVS Website {ClientsOfAllChannel.Keys.ToJson()}");

                if (ClientsOfAllChannel.TryGetValue(path, out var clientCollection))
                {
                    try
                    {
                        clientCollection.Add(clientHander);
                        clientHander.OnClientDisconnect += ClientHander_OnClientDisconnect;
                        if (user_id != "")
                        {
                            LOG.TRACE($"User-{user_id} Broswer AGVS Website  | Online-Client={OnlineClientNumber}");
                        }
                        await clientHander.ListenConnection();
                    }
                    catch (Exception ex)
                    {
                        LOG.WARN(ex.Message);
                    }
                    finally
                    {
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
            }
           
        }

        private async void ClientHander_OnClientDisconnect(object? sender, clsWebsocktClientHandler e)
        {
            try
            {
                var group = ClientsOfAllChannel.FirstOrDefault(kp => kp.Value.Contains(e));
                if (group.Value != null)
                {
                    group.Value.Remove(e);
                    if (e.UserID != "")
                        LOG.TRACE($"User-{e.UserID} Leave AGVS Website. | Online-Client={OnlineClientNumber}");
                    e.Close();
                }
            }
            catch (Exception ex)
            {
                LOG.WARN(ex.Message);
            }
            finally
            {

            }
        }

        protected async Task StartCollectViewModelDataAndPublishOutAsync()
        {
            await Task.Run(async () =>
            {
                LOG.WARN($"Start Websocket data publish");
                while (true)
                {
                    //await _ClientConnectionChanging.WaitAsync();
                    await Task.Delay(publish_duration);
                    //await _ClientConnectionChanging.WaitAsync();
                    Initializd = true;
                    try
                    {
                        await CollectViewModelData();

                        List<Task> channelTasks = new List<Task>();

                        foreach (KeyValuePair<string, object> item in CurrentViewModelDataOfAllChannel)
                        {
                            channelTasks.Add(ProcessChannelAsync(item));
                            //ProcessChannelAsync(item);
                        }

                        await Task.WhenAll(channelTasks);
                        async Task ProcessChannelAsync(KeyValuePair<string, object> item)
                        {
                            try
                            {
                                var ChannelName = item.Key;
                                var clientsInThisChannel = ClientsOfAllChannel[ChannelName];

                                if (clientsInThisChannel.Count == 0)
                                    return;
                                var clients = clientsInThisChannel.ToArray();
                                object Data = item.Value;
                                if (Data == null)
                                    return;

                                byte[] datPublishOut = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Data));

                                List<Task> clientTasks = new List<Task>();

                                foreach (clsWebsocktClientHandler client in clients.Where(c=>c.WebSocket.State== System.Net.WebSockets.WebSocketState.Open))
                                {
                                    clientTasks.Add(SendMessageAsync(client, datPublishOut));
                                    //SendMessageAsync(client, datPublishOut);
                                }

                                await Task.WhenAll(clientTasks);

                                async Task SendMessageAsync(clsWebsocktClientHandler client, byte[] data)
                                {
                                    int offset = 0;
                                    int chunkSize = 512;

                                  
                                    try
                                    {
                                        while (offset < data.Length)
                                        {
                                            int remainingBytes = data.Length - offset;
                                            int bytesToSend = Math.Min(remainingBytes, chunkSize);

                                            byte[] chunk = new byte[bytesToSend];
                                            Array.Copy(data, offset, chunk, 0, bytesToSend);

                                            await client.WebSocket.SendAsync(new ArraySegment<byte>(chunk), WebSocketMessageType.Text, offset + bytesToSend >= data.Length, CancellationToken.None);

                                            offset += bytesToSend;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        client.InvokeOnClientDisconnect();
                                        Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            
                        }

                    }
                    catch (Exception ex)
                    {
                        LOG.WARN($"Websocket data publish fail.= {ex.Message}, {ex.StackTrace}");
                    }
                    finally
                    {
                        //_ClientConnectionChanging.Release();
                        //try
                        //{
                        //  
                        //}
                        //catch (Exception ex)
                        //{
                        //    LOG.WARN($"Websocket data publish fail.= {ex.Message}, {ex.StackTrace}");
                        //    _ClientConnectionChanging = new SemaphoreSlim(1, 1);
                        //}
                    }
                }
            });
        }


        protected abstract Task CollectViewModelData();
    }
}
