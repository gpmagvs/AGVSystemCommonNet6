using AGVSystemCommonNet6.HttpTools;
using AGVSystemCommonNet6.Material;
using EquipmentManagment.WIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.MCS
{
    /// <summary>
    /// 這個類別存放載具、Zone相關的事件上報API方法
    /// </summary>
    public partial class MCSCIMService
    {
        public static async Task<(bool confirm, string message)> ShelfStatusChange(object wipData)
        {
            (bool confirm, string message) response = new(false, "[MCSCIMService.AlarmReporter] System Error.");
            try
            {
                var route = $"/api/HostMode/ShelfStatusChange";
                (bool success, string json) v = await _http.PostAsync(route, wipData);
                response.confirm = v.success;
                response.message = v.json;
            }
            catch (Exception ex)
            {
                response.message = $"[MCSCIMService.AlarmReporter] Report to: {_http.http_client.BaseAddress} with exmessage: {ex.Message}";
            }
            return response;
        }
        public static async Task<(bool confirm, string message)> ZoneCapacityChange(object wipData)
        {
            (bool confirm, string message) response = new(false, "[MCSCIMService.AlarmReporter] System Error.");
            try
            {
                var route = $"/api/HostMode/ZoneCapacityChange";
                (bool success, string json) v = await _http.PostAsync(route, wipData);
                response.confirm = v.success;
                response.message = v.json;
            }
            catch (Exception ex)
            {
                response.message = $"[MCSCIMService.AlarmReporter] Report to: {_http.http_client.BaseAddress} with exmessage: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// [CEID=151]
        /// </summary>
        /// <param name="CarrierID"></param>
        /// <param name="CarrierLoc"></param>
        /// <param name="CarrierZoneName"></param>
        /// <param name="HandoffType"></param>
        /// <returns></returns>
        public static async Task CarrierInstallCompletedReport(string CarrierID, string CarrierLoc, string CarrierZoneName, ushort HandoffType)
        {
            await _http.PostAsync<object, object>($"/api/Carrier/CarrierInstallCompleted?CarrierID=${CarrierID}&CarrierLoc=${CarrierLoc}&CarrierZoneName=${CarrierZoneName}&HandoffType=${HandoffType}",null);
        }
        /// <summary>
        /// [CEID=152]
        /// </summary>
        /// <param name="CarrierID"></param>
        /// <param name="CarrierLoc"></param>
        /// <param name="CarrierZoneName"></param>
        /// <param name="HandoffType"></param>
        /// <returns></returns>
        public static async Task CarrierRemoveCompletedReport(string CarrierID, string CarrierLoc, string CarrierZoneName, ushort HandoffType)
        {
            await _http.PostAsync<object, object>($"/api/Carrier/CarrierRemoveCompleted?CarrierID=${CarrierID}&CarrierLoc=${CarrierLoc}&CarrierZoneName=${CarrierZoneName}&HandoffType=${HandoffType}",null);
        }
    }
}
