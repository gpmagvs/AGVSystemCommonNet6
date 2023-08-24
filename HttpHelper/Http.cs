using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.TASK;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.HttpHelper
{
    public class Http
    {

        public class clsInternalError
        {
            public string ErrorCode { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
        }

        public static async Task<Tin> PostAsync<Tin, Tout>(string url, Tout data)
        {
            string contentDataJson = string.Empty;
            if (data != null)
                contentDataJson = JsonConvert.SerializeObject(data);
            var content = new StringContent(contentDataJson, System.Text.Encoding.UTF8, "application/json");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    var response = await client.PostAsync(url, content);
                    if (sw.ElapsedMilliseconds > 3000)
                        Console.WriteLine($"[HTTP Long Time Notify] Post Request({url}) | Spend:{sw.ElapsedMilliseconds}ms");

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

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }
        public static async Task<Tin> GetAsync<Tin>(string url)
        {
            try
            {
                string jsonContent = "";
                HttpResponseMessage response = null;
                using (HttpClient client = new HttpClient())
                {
                    response = await client.GetAsync(url);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        jsonContent = await response.Content.ReadAsStringAsync();
                        if (sw.ElapsedMilliseconds > 3000)
                            Console.WriteLine($"[HTTP Long Time Notify] Get Request({url}) | Spend:{sw.ElapsedMilliseconds}ms");
                        var result = JsonConvert.DeserializeObject<Tin>(jsonContent);
                        return result;
                    }
                    else
                        throw new HttpRequestException($"Failed to GET to {url}({response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public static async Task<(HttpResponseMessage response, string content)> Get(string host, string request_url)
        {
            string jsonContent = "";
            HttpResponseMessage response = null;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(host);

            var task = Task.Run(async () =>
            {
                response = await client.GetAsync(request_url);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    jsonContent = await response.Content.ReadAsStringAsync();
                    if (sw.ElapsedMilliseconds > 3000)
                        Console.WriteLine($"[HTTP Long Time Notify]Get Request({string.Format("{0}/{1}", host, request_url)}) | Spend:{sw.ElapsedMilliseconds}ms");
                }
                return (response, jsonContent);
            });
            return await task.ContinueWith(t => t.Result);
        }
    }
}
