using AGVSystemCommonNet6.Log;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.HttpTools
{
    public class HttpHelper
    {

        public class clsInternalError
        {
            public string ErrorCode { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
        }
        public HttpClient http_client { get; private set; }
        public LogBase Logger;
        public readonly string baseUrl;
        public int timeout_sec { get; set; } = 5;
        public HttpHelper(string baseUrl, int timeout_sec = 3)
        {
            this.baseUrl = baseUrl;
            this.timeout_sec = timeout_sec;
            http_client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(timeout_sec),
                BaseAddress = new Uri(baseUrl)
            };
        }
        public async Task<(bool success, string json)> PostAsync(string api_route, object data)
        {
            string contentDataJson = string.Empty;
            string url = this.baseUrl + api_route;
            if (data != null)
                contentDataJson = JsonConvert.SerializeObject(data);
            var content = new StringContent(contentDataJson, System.Text.Encoding.UTF8, "application/json");
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var response = await http_client.PostAsync(api_route, content);
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
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        public async Task<Tin> PostAsync<Tin, Tout>(string api_route, Tout data)
        {
            string contentDataJson = string.Empty;
            string url = this.baseUrl + api_route;
            if (data != null)
                contentDataJson = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(contentDataJson, System.Text.Encoding.UTF8, "application/json");
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var response = await http_client.PostAsync(api_route, content);
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
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        public async Task<Tout> GetAsync<Tout>(string api_route)
        {
            try
            {
                string jsonContent = "";
                string url = this.baseUrl + $"{api_route}";
                HttpResponseMessage response = null;
                response = await http_client.GetAsync(api_route);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    jsonContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Tout>(jsonContent);
                    return result;
                }
                else
                    throw new HttpRequestException($"Failed to GET to {url}({response.StatusCode})");
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

    }
}
