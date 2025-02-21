using Microsoft.AspNetCore.Http;
using NLog;
using Polly;
using System.Text;

namespace AGVSystemCommonNet6.HttpTools.ApiMiddlewares
{
    public class ApiLoggingMiddleware
    {
        private readonly Logger _logger;
        private readonly RequestDelegate _next;

        private static readonly HashSet<string> IgnorePaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/api/Map", "/AGVImages", "/api/system", "/api/Equipment", "/api/Alarm",
            "/api/WIP", "/api/system/website", "/api/TaskQuery", "/FrontEndDataHub", "/sw.js"
        };

        private static readonly HashSet<string> ContentTypesToIgnore = new(StringComparer.OrdinalIgnoreCase)
        {
            "image", "text/css", "text/html", "application/javascript",
            "application/zip", "application/x-zip-compressed", "font"
        };

        public ApiLoggingMiddleware(RequestDelegate next)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 只攔截需要記錄的請求，避免影響所有請求效能
            if (ShouldIgnoreRequest(context))
            {
                await _next(context);
                return;
            }

            // 記錄請求
            var requestTask = FormatRequest(context.Request);

            // 攔截回應 Body
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context); // 繼續執行請求

            // 讀取 Response
            var responseTask = FormatResponse(context.Response);

            // 恢復 Response Body
            await responseBody.CopyToAsync(originalBodyStream);

            // 非同步記錄日誌，避免影響回應時間
            _ = Task.Run(async () =>
            {
                string requestLog = await requestTask;
                string responseLog = await responseTask;

                Logger logger = CreateLogger(context);
                logger.Info("Request:\n{Request}", requestLog);
                logger.Info("Response:{Response}", responseLog);
            });
        }

        public virtual Logger CreateLogger(HttpContext context)
        {
            string controllerName = GetControllerNameFromPath(context.Request.Path);
            return LogManager.GetLogger($"{GetType().Name}/{controllerName}");
        }

        /// <summary>
        /// 判斷是否忽略該請求
        /// </summary>
        private bool ShouldIgnoreRequest(HttpContext context)
        {
            string? path = context.Request.Path.Value;

            if (string.IsNullOrEmpty(path) || path == "/")
                return true;
            // 檢查忽略路徑
            if (IgnorePaths.Contains(path))
                return true;

            // 檢查忽略的 Content-Type
            if (context.Response.Headers.TryGetValue("Content-Type", out var contentType))
                return ContentTypesToIgnore.Any(type => contentType.ToString().Contains(type, StringComparison.OrdinalIgnoreCase));
            return false;
        }

        private string GetControllerNameFromPath(PathString path)
        {
            string pathStr = path.ToString().Replace("api/", "api_").TrimStart('/');
            return pathStr.Split('/').FirstOrDefault() ?? "api";
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var headers = FormatHeaders(request.Headers);
            var ip = request.HttpContext.Connection.RemoteIpAddress?.ToString();

            string body = await ReadBody(request.Body);
            request.Body.Position = 0;

            const int ONE_MB = 1 * 1024 * 1024;
            if (request.ContentLength.HasValue && request.ContentLength > ONE_MB)
                return $"Method: {request.Method}\nURL: {request.Scheme}://{request.Host}{request.Path}{request.QueryString}\nIP: {ip}\nBody: Content size too large.Size:{request.ContentLength} bytes";
            return $"Method: {request.Method}\nURL: {request.Scheme}://{request.Host}{request.Path}{request.QueryString}\nIP: {ip}\nBody: {body}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var headers = FormatHeaders(response.Headers);
            string body = await ReadBody(response.Body);
            response.Body.Seek(0, SeekOrigin.Begin);

            // 檢查 Content-Type
            var contentType = response.ContentType?.ToLower() ?? "";
            if (contentType.Contains("javascript") ||
                contentType.Contains("css") ||
                contentType.Contains("image"))
            {
                return $"Status code: {response.StatusCode},{contentType}";
            }
            const int ONE_MB = 1 * 1024 * 1024;
            if (response.ContentLength.HasValue && response.ContentLength > ONE_MB)
                return $"Status code: {response.StatusCode},Body:Content size too large.Size:{response.ContentLength} bytes";
            return $"Status code: {response.StatusCode},Body: {body}";
        }

        private async Task<string> ReadBody(Stream bodyStream, int maxLength = 5000)
        {
            using var reader = new StreamReader(bodyStream, Encoding.UTF8, leaveOpen: true);
            char[] buffer = new char[maxLength];
            int readLength = await reader.ReadBlockAsync(buffer, 0, maxLength);
            return readLength == maxLength ? new string(buffer) + "..." : new string(buffer, 0, readLength);
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            var formattedHeaders = new StringBuilder();
            foreach (var (key, value) in headers)
            {
                formattedHeaders.AppendLine($"\t{key}: {string.Join(",", value)}");
            }
            return formattedHeaders.ToString();
        }
    }
}