using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.RunMode;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.HttpTools;
using AGVSystemCommonNet6.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.Microservices.AGVS
{
    public static class AGVSSerivces
    {
        public static string AGVSHostUrl => "http://127.0.0.1:5216";
        private static HttpHelper agvs_http => new HttpHelper(AGVSHostUrl);
        public struct TRANSFER_TASK
        {
            /// <summary>
            /// 向中控回報開始將貨物轉移至目的地
            /// </summary>
            /// <param name="AGVName"></param>
            /// <param name="SourceTag"></param>
            /// <param name="DestineTag"></param>
            /// <returns></returns>
            public static async Task StartTransferCargoReport(string AGVName, int SourceTag, int DestineTag)
            {
                using (agvs_http)
                {
                    string response = await agvs_http.GetStringAsync($"/api/Task/StartTransferCargoReport?AGVName={AGVName}&SourceTag={SourceTag}&DestineTag={DestineTag}");
                    LOG.INFO($"Cargo start Transfer to destine({DestineTag}) from source({SourceTag}) Report to AGVS, AGVS Response = {response}");
                }
            }


            public static async Task LoadUnloadActionFinishReport(int tagNumber, ACTION_TYPE action)
            {
                using (agvs_http)
                {
                    var route = $"/api/Task/LoadUnloadTaskFinish?tag={tagNumber}&action={action}";
                    string response = await agvs_http.GetStringAsync(route);
                    LOG.INFO($"LoadUnload Task Finish Feedback to AGVS, AGVS Response = {response}");

                }
            }


            public static async Task LoadUnloadActionStartReport(int tagNumber, ACTION_TYPE action)
            {
                using (agvs_http)
                {
                    var route = $"/api/Task/LoadUnloadTaskStart?tag={tagNumber}&action={action}";
                    string response = await agvs_http.GetStringAsync(route);
                    LOG.INFO($"LoadUnload Task Start Feedback to AGVS, AGVS Response = {response}");

                }
            }

        }
    }
}
