using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System.Diagnostics;

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
        private int _timeout_sec = 5;
        private bool disposedValue;
        public string Comment { get; set; } = "";
        private NLog.Logger logger = LogManager.GetLogger("HttpHelperLog");
        public int timeout_sec
        {
            get => _timeout_sec;
            set
            {
                if (_timeout_sec != value)
                {
                    logger.Info($"[{Comment}:{baseUrl}] Change timeout_sec from {_timeout_sec} to {value},Dispose Old HttpClinet instance  and New HttpClinet instance created");
                    _timeout_sec = value;
                    http_client?.Dispose();
                    http_client = new HttpClient()
                    {
                        Timeout = TimeSpan.FromSeconds(value),
                        BaseAddress = new Uri(baseUrl)
                    };
                }
            }
        }
        public HttpHelper(string baseUrl, int timeout_sec = 3, string comment = "")
        {
            this.baseUrl = baseUrl;
            this.timeout_sec = timeout_sec;
            Comment = comment;
            http_client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(timeout_sec),
                BaseAddress = new Uri(baseUrl)
            };
            logger.Info($"[{Comment}:{baseUrl}] HttpClinet instance created");

        }
        public async Task<(bool success, string json)> PostAsync(string api_route, object data, int timeout = 3)
        {
            string contentDataJson = string.Empty;
            string url = this.baseUrl + api_route;
            if (data != null)
                contentDataJson = JsonConvert.SerializeObject(data);
            var content = new StringContent(contentDataJson, System.Text.Encoding.UTF8, "application/json");
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                timeout_sec = timeout;
                using HttpResponseMessage response = await http_client.PostAsync(api_route, content);
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
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.Error(ex);
                throw;
            }

        }
        public async Task<Tin> PostAsync<Tin, Tout>(string api_route, Tout data, int timeout = 3)
        {
            string contentDataJson = string.Empty;
            string url = this.baseUrl + (this.baseUrl.Last() == '/' ? "" : "/") + api_route;
            if (data != null)
                contentDataJson = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(contentDataJson, System.Text.Encoding.UTF8, "application/json");
            try
            {
                timeout_sec = timeout;
                using HttpResponseMessage response = await http_client.PostAsync(api_route, content);
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
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.Error(ex);
                throw;
            }

        }
        public async Task<Tin> GetAsync<Tin>(string api_route, int timeout = 3)
        {
            string jsonContent = "";
            try
            {
                string url = this.baseUrl + $"{api_route}";
                timeout_sec = timeout;
                using HttpResponseMessage response = await http_client.GetAsync(api_route);
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
                throw ex;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
        public async Task<string> GetStringAsync(string api_route, int timeout = 3)
        {
            string str_result = "";
            try
            {
                string url = this.baseUrl + $"{api_route}";
                timeout_sec = timeout;
                using HttpResponseMessage response = await http_client.GetAsync(api_route);
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
                throw ex;
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
