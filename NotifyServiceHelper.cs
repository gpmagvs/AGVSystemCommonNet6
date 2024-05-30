using AGVSystemCommonNet6.Log;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace AGVSystemCommonNet6.Notify
{
    public class NotifyServiceHelper
    {
        public static async Task WebsocketNotification(HttpContext context)
        {
            var tcs = new TaskCompletionSource<object>();
            System.Net.WebSockets.WebSocket? client = null;
            try
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    client = await context.WebSockets.AcceptWebSocketAsync();
                    NotifyServiceHelper.OnMessage += OnMessageRecieved;
                    await tcs.Task;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }


            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Notify register is disconnect.");
                client?.Dispose();

            }
            async void OnMessageRecieved(object sender, NotifyMessage notify)
            {
                try
                {
                    client?.ReceiveAsync(new ArraySegment<byte>(new byte[20]), CancellationToken.None);
                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(notify));
                    if (client.State != System.Net.WebSockets.WebSocketState.Open)
                    {
                        NotifyServiceHelper.OnMessage -= OnMessageRecieved;
                        client.Dispose();
                        tcs.SetCanceled();
                    }
                    await client.SendAsync(new ArraySegment<byte>(data), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    client?.Dispose();
                }
            }
        }
        public class NotifyMessage
        {
            public enum NOTIFY_TYPE
            {
                info, warning, error, success
            }
            public NOTIFY_TYPE type { get; set; } = NOTIFY_TYPE.info;
            public string message { get; set; } = "";

            public bool show { get; set; } = true;

        }
        public static event EventHandler<NotifyMessage> OnMessage;

        public static async Task INFO(string message, bool show = true)
        {
            await NotifyAsync(NotifyMessage.NOTIFY_TYPE.info, message, show);
        }
        public static async Task WARNING(string message, bool show = true)
        {
            await NotifyAsync(NotifyMessage.NOTIFY_TYPE.warning, message, show);
        }
        public static async Task ERROR(string message, bool show = true)
        {
            await NotifyAsync(NotifyMessage.NOTIFY_TYPE.error, message, show);
        }
        public static async Task SUCCESS(string message, bool show = true)
        {
            await NotifyAsync(NotifyMessage.NOTIFY_TYPE.success, message, show);
        }

        public static async Task NotifyAsync(NotifyMessage.NOTIFY_TYPE type, string message, bool show)
        {
            LOG.TRACE($"[Notify]-[{type}] {message}");
            var handler = OnMessage;
            if (handler != null)
            {
                try
                {
                    OnMessage?.Invoke("", new NotifyMessage
                    {
                        type = type,
                        message = message,
                        show = show
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
