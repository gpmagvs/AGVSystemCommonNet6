﻿using AGVSystemCommonNet6.AGVDispatch.Messages;
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
using System.Xml.Linq;
using static AGVSystemCommonNet6.clsEnums;
using static System.Collections.Specialized.BitVector32;

namespace AGVSystemCommonNet6.Microservices.AGVS
{
    public static class AGVSSerivces
    {
        public static string AGVSHostUrl => "http://127.0.0.1:5216";
        private static HttpHelper agvs_http => new HttpHelper(AGVSHostUrl);

        public struct TRAFFICS
        {
            public static async Task<clsAGVSBlockedPointsResponse> GetBlockedTagsByEqMaintain()
            {
                using (agvs_http)
                {
                    clsAGVSBlockedPointsResponse response = await agvs_http.GetAsync<clsAGVSBlockedPointsResponse>($"/api/Traffic/GetBlockedTagsByEqMaintain");
                    return response;
                }
            }

            public static async Task<clsGetUsableChargeStationTagResponse> GetUseableChargeStationTags(string agv_name)
            {
                using (agvs_http)
                {
                    clsGetUsableChargeStationTagResponse response = await agvs_http.GetAsync<clsGetUsableChargeStationTagResponse>($"/api/Traffic/GetUseableChargeStationTags?agv_name={agv_name}");
                    return response;
                }
            }
        }

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

            public static async Task<Dictionary<int, int>> GetEQAcceptAGVTypeInfo(IEnumerable<int> tagsCollections)
            {
                using (agvs_http)
                {
                    try
                    {
                        var route = $"/api/Equipment/GetEQOptionsByTags";
                        var response = await agvs_http.PostAsync<List<Dictionary<string, object>>, int[]>(route, tagsCollections.ToArray());

                        return response.ToDictionary(obj => int.Parse(obj["Tag"].ToString()), obj => int.Parse(obj["Accept_AGV_Type"].ToString()));

                        //[
                        //  {
                        //    Tag = option.TagID,
                        //    EqName = option.Name,
                        //    AGVModbusGatewayPort = option.ConnOptions.AGVModbusGatewayPort,
                        //    Accept_AGV_Type = option.Accept_AGV_Type
                        //  },
                        //  {
                        //      ...
                        //  }
                        //]
                    }
                    catch (Exception ex)
                    {
                        LOG.Critical($"LoadUnload Order Start Feedback to AGVS FAIL,{ex.Message}", ex);
                        //return new clsAGVSTaskReportResponse() { confirm = false, message = ex.Message };
                        return new Dictionary<int, int>();
                    }
                }
            }
        }
    }
}
