using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System.Diagnostics;
using static SQLite.SQLite3;

namespace AGVSystemCommonNet6.HttpTools
{
    public class HttpHelper : IDisposable
    {

        public class clsInternalError
        {
            public string ErrorCode { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
        }
        public HttpClient http_client { get; private set; }
        public readonly string baseUrl;
        private bool disposedValue;
        public string Comment { get; set; } = "";
        private NLog.Logger logger = LogManager.GetLogger("HttpHelperLog");

        public HttpHelper(string baseUrl, string comment = "")
        {
            this.baseUrl = baseUrl;
            Comment = comment;
            http_client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(60),
                BaseAddress = new Uri(baseUrl)
            };
            logger.Info($"[{Comment}:{baseUrl}] HttpClinet instance created");

        }
        public async Task<(bool success, string json)> PostAsync(string api_route, object data, int timeout = 1)
        {
            string contentDataJson = string.Empty;
            string url = this.baseUrl + api_route;
            if (data != null)
                contentDataJson = JsonConvert.SerializeObject(data);
            var content = new StringContent(contentDataJson, System.Text.Encoding.UTF8, "application/json");
            try
            {
                using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));// 使用 CancellationTokenSource 設置特定請求的超時
                using HttpResponseMessage response = await http_client.PostAsync(api_route, content, cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return (true, responseJson);
                }
                else
                {
                    var errmsg = $"Failed to POST to {url}. Response status code: {response.StatusCode}";
                    Console.WriteLine(errmsg);
                    throw new HttpRequestException(errmsg);
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex.Message);
                logger.Error(ex);
                return (false, "{}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.Error(ex);
                throw;
            }

        }
        public async Task<Tin> PostAsync<Tin, Tout>(string api_route, Tout data, int timeout = 1)
        {
            string contentDataJson = string.Empty;
            string url = this.baseUrl + (this.baseUrl.Last() == '/' ? "" : "/") + api_route;
            if (data != null)
                contentDataJson = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(contentDataJson, System.Text.Encoding.UTF8, "application/json");
            try
            {
                using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));// 使用 CancellationTokenSource 設置特定請求的超時
                using HttpResponseMessage response = await http_client.PostAsync(api_route, content, cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Tin>(responseJson);
                    return result;
                }
                else
                {
                    var errmsg = $"Failed to POST to {url}. Response status code: {response.StatusCode}";
                    Console.WriteLine(errmsg);
                    throw new HttpRequestException(errmsg);
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex.Message);
                logger.Error(ex);
                return JsonConvert.DeserializeObject<Tin>("{}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.Error(ex);
                throw;
            }

        }
        public async Task<Tin> GetAsync<Tin>(string api_route, int timeout = 1)
        {
            string jsonContent = "";
            try
            {
                string url = this.baseUrl + $"{api_route}";
                using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));// 使用 CancellationTokenSource 設置特定請求的超時
                using HttpResponseMessage response = await http_client.GetAsync(api_route, cts.Token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    jsonContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Tin>(jsonContent);
                    return result;
                }
                else
                    throw new HttpRequestException($"Failed to GET to {url}({response.StatusCode})");
            }
            catch (TaskCanceledException ex)
            {
                logger.Error(ex);
                return JsonConvert.DeserializeObject<Tin>("{}");

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
        public async Task<string> GetStringAsync(string api_route, int timeout = 1)
        {
            string str_result = "";
            try
            {
                string url = this.baseUrl + $"{api_route}";
                using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));// 使用 CancellationTokenSource 設置特定請求的超時
                using HttpResponseMessage response = await http_client.GetAsync(api_route, cts.Token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    str_result = await response.Content.ReadAsStringAsync();
                    return str_result;
                }
                else
                    throw new HttpRequestException($"Failed to GET to {url}({response.StatusCode})");
            }
            catch (TaskCanceledException ex)
            {
                logger.Error(ex);
                return "{}";
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
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
                http_client?.Dispose();
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~HttpHelper()
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
