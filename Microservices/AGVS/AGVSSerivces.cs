﻿using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.HttpTools;
using AGVSystemCommonNet6.Material;
using AGVSystemCommonNet6.Microservices.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AGVSystemCommonNet6.Microservices.AGVS
{
    public static partial class AGVSSerivces
    {
        public static string AGVSHostUrl => "http://127.0.0.1:5216";
        private static HttpHelper _agvs_http;
        private static HttpHelper GetAGVSHttpHelper()
        {
            if (_agvs_http == null)
                _agvs_http = new HttpHelper(AGVSHostUrl);
            return _agvs_http;
        }

        public struct DATABASE
        {
            public static async Task<DBDataService.OperationResult> AddTaskDto(clsTaskDto taskDto)
            {
                var agvs_http = GetAGVSHttpHelper();
                DBDataService.OperationResult response = await agvs_http.PostAsync<DBDataService.OperationResult, clsTaskDto>($"/api/Database/AddTaskDto", taskDto);
                return response;
            }
            public static async Task<DBDataService.OperationResult> ModifyTaskDto(clsTaskDto taskDto)
            {
                var agvs_http = GetAGVSHttpHelper();
                DBDataService.OperationResult response = await agvs_http.PostAsync<DBDataService.OperationResult, clsTaskDto>($"/api/Database/ModifyTaskDto", taskDto);
                return response;
            }

            public static async Task<DBDataService.OperationResult> DeleteTaskDto(clsTaskDto taskDto)
            {
                var agvs_http = GetAGVSHttpHelper();
                DBDataService.OperationResult response = await agvs_http.PostAsync<DBDataService.OperationResult, clsTaskDto>($"/api/Database/DeleteTaskDto", taskDto);
                return response;
            }


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

            public static async Task<List<int>> GetTagsOfEQPartsReplacing()
            {
                var agvs_http = GetAGVSHttpHelper();
                List<int> response = await agvs_http.GetAsync<List<int>>($"/api/Traffic/GetTagsOfEQPartsReplacing");
                return response;
            }

            /// <summary>
            /// 暫時關閉指定的 rack colum ports
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public static async Task DisableRackColumnTemptory(int tag, int timeout = 1)
            {
                var agvs_http = GetAGVSHttpHelper();
                await agvs_http.PostAsync($"api/WIP/DisablePortsColumnTempotary?tag={tag}", null, timeout);
            }
            /// <summary>
            /// 恢復被暫時關閉指定的 rack colum ports
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public static async Task RestoreRackColumnByDisabledTemptory(int tag, int timeout = 1)
            {
                var agvs_http = GetAGVSHttpHelper();
                await agvs_http.PostAsync($"api/WIP/EnablePortsColumnByDisableTempotary?tag={tag}", null, timeout);
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
            public static async Task<clsAGVSTaskReportResponse> StartTransferCargoReport(string AGVName, int SourceTag, int DestineTag, string SoureSlot, string DestineSlot, bool IsSourceAGV)
            {
                var agvs_http = GetAGVSHttpHelper();
                clsAGVSTaskReportResponse response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>($"/api/Task/StartTransferCargoReport?AGVName={AGVName}&SourceTag={SourceTag}&DestineTag={DestineTag}&SourceSlot={SoureSlot}&DestineSlot={DestineSlot}&IsSourceAGV={IsSourceAGV}");
                return response;
            }
            public static async Task<clsAGVSTaskReportResponse> LoadUnloadActionFinishReport(string taskID, int tagNumber, ACTION_TYPE action, bool normalDone, string agvName = "")
            {
                clsAGVSTaskReportResponse response = new clsAGVSTaskReportResponse(false, ALARMS.SYSTEM_ERROR, "", "");
                var agvs_http = GetAGVSHttpHelper();
                try
                {
                    var route = $"/api/Task/LoadUnloadTaskFinish?taskID={taskID}&tag={tagNumber}&action={action}&normalDone={normalDone}";
                    response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>(route);
                }
                catch (HttpRequestException ex)
                {
                    return new clsAGVSTaskReportResponse() { confirm = false, AlarmCode = ALARMS.VMSOrderActionStatusReportToAGVSFail, message = ex.Message };

                }
                catch (TaskCanceledException ex)
                {
                    return new clsAGVSTaskReportResponse() { confirm = false, AlarmCode = ALARMS.VMSOrderActionStatusReportToAGVSTimeout, message = ex.Message };
                }
                catch (Exception ex)
                {
                    response = new clsAGVSTaskReportResponse() { confirm = false, AlarmCode = ALARMS.VMSOrderActionStatusReportToAGVSFail, message = ex.Message };
                }
                //NotifyServiceHelper.INFO($"{agvName} {action} Action Finish Report To AGVS.Alarm Code Response={response.AlarmCode}");
                return response;
            }
            public static async Task<clsAGVSTaskReportResponse> LoadUnloadActionStartReport(string taskID, int tagNumber, int slot, ACTION_TYPE action)
            {
                var agvs_http = GetAGVSHttpHelper();
                try
                {
                    var route = $"/api/Task/LoadUnloadTaskStart?taskID={taskID}&tag={tagNumber}&slot={slot}&action={action}";
                    clsAGVSTaskReportResponse response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>(route);
                    //LOG.INFO($"LoadUnload Task Start Feedback to AGVS, AGVS Response = {response.ToJson()}");
                    return response;
                }
                catch (HttpRequestException ex)
                {
                    return new clsAGVSTaskReportResponse() { confirm = false, AlarmCode = ALARMS.VMSOrderActionStatusReportToAGVSFail, message = ex.Message };

                }
                catch (TaskCanceledException ex)
                {
                    return new clsAGVSTaskReportResponse() { confirm = false, AlarmCode = ALARMS.VMSOrderActionStatusReportToAGVSTimeout, message = ex.Message };
                }
                catch (Exception ex)
                {
                    return new clsAGVSTaskReportResponse() { confirm = false, AlarmCode = ALARMS.VMSOrderActionStatusReportToAGVSFail, message = ex.Message };
                }
            }
            public static async Task<clsAGVSTaskReportResponse> StartLDULDOrderReport(string taskID, int from_Station_Tag, int from_station_slot, int to_Station_Tag, int to_Station_Slot, ACTION_TYPE action, bool isSourceAGV = false)
            {
                clsAGVSTaskReportResponse response = new clsAGVSTaskReportResponse() { confirm = false };
                int intRetry = 0;
                bool IsReportOK = false;
                while (intRetry < 3 && IsReportOK == false)
                {
                    try
                    {
                        var route = $"/api/Task/LDULDOrderStart?taskID={taskID}&from={from_Station_Tag}&FromSlot={from_station_slot}&to={to_Station_Tag}&ToSlot={to_Station_Slot}&action={action}&isSourceAGV={isSourceAGV}";

                        var agvs_http = GetAGVSHttpHelper();
                        response = await agvs_http.GetAsync<clsAGVSTaskReportResponse>(route, timeout: 10);

                        return response;
                    }
                    catch (Exception ex)
                    {
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
                    return new List<int>();
                }
            }

            public static async Task<bool> call_AGVs_carry_api(string strToken, string strUsername, object data)
            {
                HttpHelper agvs_http = GetAGVSHttpHelper();
                agvs_http.http_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $"{strToken}");
                try
                {
                    var route = $"/api/Task/carry?user={strUsername}";
                    var response = await agvs_http.PostAsync(route, data, timeout: 15);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public static async Task<(bool confirm, string message, object obj)> GetNGPort()
        {
            (bool confirm, string message, object obj) response = new(false, "", null);
            GetAGVSHttpHelper();
            try
            {
                var route = $"/api/Equipment/GetNgPort";
                clsResponseBase v = await _agvs_http.GetAsync<clsResponseBase>(route, timeout: 15);
                response.confirm = v.confirm;
                response.message = v.message;
                response.obj = v.ReturnObj;

            }
            catch (Exception ex)
            {
                response.message = ex.Message;
            }
            return response;
        }

        [HttpPost("UpdateStationInfo")]
        public static async Task<(bool confirm, string message)> UpdateStationInfo(clsStationStatus stationStatus)
        {
            (bool confirm, string message) response = new(false, "[AGVSSerivces.UpdateStationInfo] System Error");
            GetAGVSHttpHelper();

            try
            {
                var route = $"/api/Equipment/UpdateStationInfo";
                (bool success, string message) v = await _agvs_http.PostAsync(route, stationStatus, timeout: 7);

                response = v;
            }
            catch (Exception ex)
            {
                response.message = $"[AGVSSerivces.AlarmReporterSwitch] {ex.Message}";
            }

            return response;
        }

        public static async Task<(bool confirm, string token, string strUsername)> Login()
        {
            (bool confirm, string token, string strUsername) response = new(false, "", "");
            GetAGVSHttpHelper();
            try
            {
                var route = $"/api/Auth/login?Username=dev&Password=12345678";
                (bool success, string json) v = await _agvs_http.PostAsync(route, new AGVSystemCommonNet6.User.UserLoginRequest() { Username = "dev", Password = "12345678" }, timeout: 15);
                response.confirm = v.success;
                Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(v.json);
                string strToken = obj["token"];
                string strUsername = obj["UserName"];
                response.token = strToken;
                response.strUsername = strUsername;
            }
            catch (Exception ex)
            {
                response.token = ex.Message;
            }
            return response;

        }
    }
}
