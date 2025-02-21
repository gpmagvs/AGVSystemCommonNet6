using Newtonsoft.Json;
using NLog;
using System.Net.Http.Json;
using System.Text;
using Polly;
using Polly.Retry;
using System.Net;

namespace AGVSystemCommonNet6.HttpTools
{
    public class HttpHelper : IDisposable
    {
        public readonly HttpClient http_client;

        private readonly Logger _logger;
        private bool _disposedValue;
        public string Comment { get; }
        public string? baseUrl => http_client?.BaseAddress?.ToString();

        public HttpHelper(string baseUrl, string comment = "", int maxRetries = 3)
        {
            Comment = comment;
            http_client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(60),
                BaseAddress = new Uri(baseUrl)
            };
            _logger = LogManager.GetLogger("HttpHelperLog");
            _logger.Info($"[{Comment}:{baseUrl}] HttpClient instance created");


        }

        public async Task<(bool success, string json)> PostAsync(string apiRoute, object data, double timeout = 5, int retryCount = 3)
        {
            var response = await SendRequestAsync(HttpMethod.Post, apiRoute, data, timeout, retryCount);
            var responseJson = await response.Content.ReadAsStringAsync();
            return (true, responseJson);
        }

        public async Task<TResponse> PostAsync<TResponse, TRequest>(string apiRoute, TRequest data, double timeout = 5, int retryCount = 3)
        {
            var response = await SendRequestAsync(HttpMethod.Post, apiRoute, data, timeout, retryCount);
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseJson);
        }

        public async Task<T> GetAsync<T>(string apiRoute, double timeout = 5, int retryCount = 3)
        {
            var response = await SendRequestAsync(HttpMethod.Get, apiRoute, null, timeout, retryCount);
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseJson);
        }

        public async Task<string> GetStringAsync(string apiRoute, double timeout = 5, int retryCount = 3)
        {
            var response = await SendRequestAsync(HttpMethod.Get, apiRoute, null, timeout, retryCount);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string apiRoute, object data = null, double timeout = 5, int retryCount = 3)
        {
            _logger.Trace($"{method} Start-{http_client.BaseAddress}, Path: {apiRoute}");

            AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy<HttpResponseMessage>
             .Handle<HttpRequestException>()
             .Or<TaskCanceledException>()
             .OrResult(response => !response.IsSuccessStatusCode)
             .WaitAndRetryAsync(
                 retryCount,
                 retryAttempt => TimeSpan.FromMilliseconds(500),
                 onRetry: (exception, timeSpan, retryCount, context) =>
                 {
                     var error = exception.Exception?.Message ?? $"Status code: {exception.Result?.StatusCode}";
                     _logger.Warn($"Retry {method} to {http_client.BaseAddress}..{apiRoute} {retryCount} after {timeSpan.TotalSeconds} seconds. Error: {error}");
                 }
             );

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));

                try
                {
                    HttpResponseMessage response;
                    if (method == HttpMethod.Get)
                    {
                        response = await http_client.GetAsync(apiRoute, cts.Token);
                    }
                    else
                    {
                        var content = CreateJsonContent(data);
                        response = await http_client.PostAsync(apiRoute, content, cts.Token);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        _logger.Trace($"{method} Success-{http_client.BaseAddress}, Path: {apiRoute}. Response:{responseContent}");
                        return response;
                    }
                    else
                    {
                        var errorMessage = $"{method} Fail:{http_client.BaseAddress}, Path: {apiRoute}:Response status code: {response.StatusCode}";
                        _logger.Error(errorMessage);
                        throw new HttpRequestException(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn($"{method} Fail:{http_client.BaseAddress}, Path: {apiRoute}:{ex.Message}");
                    throw;
                }
            });
        }
        private static StringContent CreateJsonContent(object data)
        {
            var json = data != null ? JsonConvert.SerializeObject(data) : "";
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    http_client?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
