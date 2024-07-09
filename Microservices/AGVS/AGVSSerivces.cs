using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.RunMode;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.HttpTools;
using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.MAP;
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
        private static HttpHelper _agvs_http;
        private static HttpHelper GetAGVSHttpHelper()
        {
            if (_agvs_http == null)
                _agvs_http = new HttpHelper(AGVSHostUrl);
            return _agvs_http;
        }


        public struct TRAFFICS
        {
            public static async Task<clsAGVSBlockedPointsResponse> GetBlockedTagsByEqMaintain()
            {
                var agvs_http = GetAGVSHttpHelper();
                clsAGVSBlockedPointsResponse response = await agvs_http.GetAsync<clsAGVSBlockedPointsResponse>($"/api/Traffic/GetBlockedTagsByEqMaintain");
                return response;
            }

            public static async Task<clsGetUsableChargeStationTagResponse> GetUseableChargeStationTags(string agv_name)
            {
                var agvs_http = GetAGVSHttpHelper();
                clsGetUsableChargeStationTagResponse response = await agvs_http.GetAsync<clsGetUsableChargeStationTagResponse>($"/api/Traffic/GetUseableChargeStationTags?agv_name={agv_name}");
                return response;
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
                var agvs_http = GetAGVSHttpHelper();
                clsAGVSTaskReportResponse response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>($"/api/Task/StartTransferCargoReport?AGVName={AGVName}&SourceTag={SourceTag}&DestineTag={DestineTag}");
                LOG.INFO($"Cargo start Transfer to destine({DestineTag}) from source({SourceTag}) Report to AGVS, AGVS Response = {response.ToJson()}");
                return response;
            }
            public static async Task<clsAGVSTaskReportResponse> LoadUnloadActionFinishReport(int tagNumber, ACTION_TYPE action)
            {
                var agvs_http = GetAGVSHttpHelper();
                try
                {
                    var route = $"/api/Task/LoadUnloadTaskFinish?tag={tagNumber}&action={action}";
                    LOG.INFO($"LoadUnloadActionFinishReport start");
                    clsAGVSTaskReportResponse response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>(route);
                    LOG.INFO($"LoadUnload Task Finish Feedback to AGVS, AGVS Response = {response.ToJson()}");
                    return response;
                }
                catch (Exception ex)
                {
                    LOG.Critical($"LoadUnload Task Finish Feedback to AGVS FAIL,{ex.Message}", ex);
                    return new clsAGVSTaskReportResponse() { confirm = false, message = ex.Message };
                }

            }
            public static async Task<clsAGVSTaskReportResponse> LoadUnloadActionStartReport(int tagNumber, int slot, ACTION_TYPE action)
            {
                var agvs_http = GetAGVSHttpHelper();
                try
                {
                    var route = $"/api/Task/LoadUnloadTaskStart?tag={tagNumber}&slot={slot}&action={action}";
                    LOG.INFO($"LoadUnloadActionStartReport start");
                    clsAGVSTaskReportResponse response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>(route);
                    //LOG.INFO($"LoadUnload Task Start Feedback to AGVS, AGVS Response = {response.ToJson()}");
                    return response;
                }
                catch (Exception ex)
                {
                    LOG.Critical($"LoadUnload Task Start Feedback to AGVS FAIL,{ex.Message}", ex);
                    return new clsAGVSTaskReportResponse() { confirm = false, message = ex.Message };
                }
            }
            public static async Task<clsAGVSTaskReportResponse> StartLDULDOrderReport(int from_Station_Tag, int from_station_slot, int to_Station_Tag, int to_Station_Slot, ACTION_TYPE action, bool isSourceAGV = false)
            {
                clsAGVSTaskReportResponse response = new clsAGVSTaskReportResponse() { confirm = false };
                int intRetry = 0;
                bool IsReportOK = false;
                while (intRetry < 3 && IsReportOK == false)
                {
                    try
                    {
                        var route = $"/api/Task/LDULDOrderStart?from={from_Station_Tag}&FromSlot={from_station_slot}&to={to_Station_Tag}&ToSlot={to_Station_Slot}&action={action}&isSourceAGV={isSourceAGV}";
                        LOG.INFO($"StartLDULDOrderReport start");

                        var agvs_http = GetAGVSHttpHelper();
                        response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>(route);

                        LOG.INFO($"LoadUnload Order Start Feedback to AGVS, AGVS Response = {response.ToJson()}");
                        return response;
                    }
                    catch (Exception ex)
                    {
                        LOG.Critical($"LoadUnload Order Start Feedback to AGVS FAIL (try:{intRetry + 1}times),{ex.Message}", ex);
                        response = new clsAGVSTaskReportResponse() { confirm = false, message = ex.Message };
                    }
                    intRetry++;
                    await Task.Delay(1000);
                }
                return response;
            }
            public static async Task<Dictionary<int, int>> GetEQAcceptAGVTypeInfo(IEnumerable<int> tagsCollections)
            {

                var agvs_http = GetAGVSHttpHelper();
                try
                {
                    var route = $"/api/Equipment/GetEQOptionsByTags";
                    //LOG.INFO($"GetEQAcceptAGVTypeInfo start");
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
                    LOG.Critical($"GetEQAcceptAGVTypeInfo from AGVS FAIL,{ex.Message}", ex);
                    //return new clsAGVSTaskReportResponse() { confirm = false, message = ex.Message };
                    return new Dictionary<int, int>();
                }
            }
            public static async Task<List<int>> GetEQWIPAcceptTransferTagInfoByTag(int tag)
            {
                HttpHelper agvs_http = GetAGVSHttpHelper();
                try
                {
                    var route = $"/api/Equipment/GetEQWIPInfoByTag?Tag={tag}";
                    //LOG.INFO($"GetEQAcceptAGVTypeInfo start");
                    var response = await agvs_http.GetAsync<Dictionary<string, object>>(route);
                    object vv = response["AcceptTransferTag"];
                    List<int> ll = JsonConvert.DeserializeObject<List<int>>(vv.ToString());
                    return ll;
                }
                catch (Exception ex)
                {
                    LOG.Critical($"GetEQAcceptAGVTypeInfo from AGVS FAIL,{ex.Message}", ex);
                    return new List<int>();
                }
            }
        }
        public static async Task<(bool confirm, string message)> TaskReporter(object data)
        {
            (bool confirm, string message) response = new(false, "[AGVSSerivces.TaskReporter] System Error");
            GetAGVSHttpHelper();
            try
            {
                var route = $"/api/MCSCIM/TaskReporter";
                string strJson = data.ToJson();
                (bool success, string json) v = await _agvs_http.PostAsync(route, data, timeout: 7);
                clsAGVSTaskReportResponse responsedata = JsonConvert.DeserializeObject<clsAGVSTaskReportResponse>(v.json);
                if (v.success == true && responsedata.confirm == true)
                    response.confirm = true;
                else
                    response.confirm = false;
                response.message = responsedata.message;
            }
            catch (Exception ex)
            {
                response.message = $"[AGVSSerivces.TaskReporter] {ex.Message}";
            }
            return response;
        }
    }
}
