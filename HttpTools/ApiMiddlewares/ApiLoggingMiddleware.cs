using Microsoft.AspNetCore.Http;
using NLog;
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
            if (ShouldIgnoreRequest(context))
            {
                await _next(context);
                return;
            }

            var requestTask = FormatRequest(context.Request);

            await _next(context);

            var responseTask = FormatResponse(context.Response);

            FireAndForget(LogRequestAndResponseAsync(requestTask, responseTask, context));
        }

        private async Task LogRequestAndResponseAsync(Task<string> requestTask, Task<string> responseTask, HttpContext context)
        {
            string requestLog = await requestTask;
            string responseLog = await responseTask;
            Logger logger = CreateLogger(context);
            logger.Info("Request:\n{Request}", requestLog);
            logger.Info("Response:\n{Response}", responseLog);
        }

        protected virtual Logger CreateLogger(HttpContext context)
        {
            string controllerName = GetControllerNameFromPath(context.Request.Path);
            return LogManager.GetLogger($"{GetType().Name}/{controllerName}");
        }
        private string GetControllerNameFromPath(PathString path)
        {
            string pathStr = path.ToString().Replace("api/", "api_").TrimStart('/');
            return pathStr.Split('/').FirstOrDefault() ?? "api";
        }
        private void FireAndForget(Task task)
        {
            task.ContinueWith(t =>
            {
                _logger.Error(t.Exception, "ApiLoggingMiddleware encountered an error.");
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private bool ShouldIgnoreRequest(HttpContext context)
        {
            string? path = context.Request.Path.Value;

            if (string.IsNullOrEmpty(path) || path == "/" || IgnorePaths.Contains(path))
                return true;

            if (context.Response.Headers.TryGetValue("Content-Type", out var contentType))
                return ContentTypesToIgnore.Any(type => contentType.ToString().Contains(type, StringComparison.OrdinalIgnoreCase));
            return false;
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering(); // 確保請求內容可多次讀取
            var headers = FormatHeaders(request.Headers);
            var ip = request.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (request.ContentLength.HasValue && request.ContentLength > 1_000_000)
            {
                return $"Method: {request.Method}\nURL: {request.Scheme}://{request.Host}{request.Path}{request.QueryString}\nIP: {ip}\nBody: Content too large ({request.ContentLength} bytes)";
            }

            // 讀取 Body
            string body;
            var memoryStream = new MemoryStream(); // **移除 using**

            await request.Body.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(memoryStream, leaveOpen: true)) // **確保 MemoryStream 不會被關閉**
            {
                body = await reader.ReadToEndAsync();
            }

            // 重置 Body 流，確保其他 Middleware 可正常讀取
            memoryStream.Seek(0, SeekOrigin.Begin);
            request.Body = memoryStream; // **確保未關閉的 Stream 被保留**

            return $"Method: {request.Method}\nURL: {request.Scheme}://{request.Host}{request.Path}{request.QueryString}\nIP: {ip}\nBody: {body}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            const int ONE_MB = 1 * 1024 * 1024;

            // 提早判斷 Content-Length，避免不必要的 Body 讀取
            if (response.ContentLength.HasValue && response.ContentLength > ONE_MB)
            {
                return $"Status code: {response.StatusCode}, Body: Content size too large. Size: {response.ContentLength} bytes";
            }

            // 檢查 Content-Type（避免頻繁調用 ToLower）
            var contentType = response.ContentType?.ToLowerInvariant() ?? "";
            if (contentType.Contains("javascript") ||
                contentType.Contains("css") ||
                contentType.Contains("image"))
            {
                return $"Status code: {response.StatusCode}, {contentType}";
            }

            // **替換 Response Body 為 MemoryStream**
            var originalBodyStream = response.Body;
            var memoryStream = new MemoryStream();
            response.Body = memoryStream;  // **讓 ASP.NET Core 把 Response 寫入 MemoryStream**

            try
            {
                // 等待 Response 完成
                await memoryStream.FlushAsync();
                memoryStream.Seek(0, SeekOrigin.Begin);

                // 讀取 Body
                string body;
                using (var reader = new StreamReader(memoryStream, leaveOpen: true)) // **確保 MemoryStream 不會被關閉**
                {
                    body = await reader.ReadToEndAsync();
                }

                // **將 MemoryStream 內容寫回原始 Response Body**
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return $"Status code: {response.StatusCode}, Body: {body}";
            }
            finally
            {
                response.Body = originalBodyStream; // **確保還原 Response Body**
            }
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
            return string.Join("; ", headers
                .Where(h => h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) ||
                            h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                .Select(h => $"{h.Key}: {string.Join(",", h.Value)}"));
        }
    }
}
