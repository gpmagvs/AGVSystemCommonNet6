using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.RunMode;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.HttpTools;
using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.Microservices.ResponseModel;
using Newtonsoft.Json;
using RosSharp.RosBridgeClient;
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
            public static async Task<clsAGVSTaskReportResponse> StartTransferCargoReport(string AGVName, int SourceTag, int DestineTag)
            {
                using (agvs_http)
                {
                    clsAGVSTaskReportResponse response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>($"/api/Task/StartTransferCargoReport?AGVName={AGVName}&SourceTag={SourceTag}&DestineTag={DestineTag}");
                    LOG.INFO($"Cargo start Transfer to destine({DestineTag}) from source({SourceTag}) Report to AGVS, AGVS Response = {response.ToJson()}");
                    return response;
                }
            }


            public static async Task<clsAGVSTaskReportResponse> LoadUnloadActionFinishReport(int tagNumber, ACTION_TYPE action)
            {
                using (agvs_http)
                {
                    var route = $"/api/Task/LoadUnloadTaskFinish?tag={tagNumber}&action={action}";
                    clsAGVSTaskReportResponse response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>(route);
                    LOG.INFO($"LoadUnload Task Finish Feedback to AGVS, AGVS Response = {response.ToJson()}");
                    return response;

                }
            }


            public static async Task<clsAGVSTaskReportResponse> LoadUnloadActionStartReport(int tagNumber, ACTION_TYPE action)
            {
                using (agvs_http)
                {
                    try
                    {
                        var route = $"/api/Task/LoadUnloadTaskStart?tag={tagNumber}&action={action}";
                        clsAGVSTaskReportResponse response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>(route);
                        LOG.INFO($"LoadUnload Task Start Feedback to AGVS, AGVS Response = {response.ToJson()}");
                        return response;
                    }
                    catch (Exception ex)
                    {
                        LOG.Critical($"LoadUnload Task Start Feedback to AGVS FAIL,{ex.Message}", ex);
                        return new clsAGVSTaskReportResponse() { confirm = false, message = ex.Message };
                    }
                }
            }

            public static async Task<clsAGVSTaskReportResponse> StartLDULDOrderReport(int destineTag, ACTION_TYPE action)
            {
                return await StartLDULDOrderReport(-1, destineTag, action);
            }

            public static async Task<clsAGVSTaskReportResponse> StartLDULDOrderReport(int from_Station_Tag, int to_Station_Tag, ACTION_TYPE action)
            {
                using (agvs_http)
                {
                    try
                    {
                        var route = $"/api/Task/LDULDOrderStart?from={from_Station_Tag}&to={to_Station_Tag}&action={action}";
                        clsAGVSTaskReportResponse response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>(route);
                        LOG.INFO($"LoadUnload Order Start Feedback to AGVS, AGVS Response = {response.ToJson()}");
                        return response;

                    }
                    catch (Exception ex)
                    {
                        LOG.Critical($"LoadUnload Order Start Feedback to AGVS FAIL,{ex.Message}", ex);
                        return new clsAGVSTaskReportResponse() { confirm = false, message = ex.Message };

                    }
                }
            }
        }
    }
}
