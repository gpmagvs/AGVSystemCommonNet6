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

        public class ZoneData
        {
            public string ZoneName { get; set; } = string.Empty;
            public int ZoneCapacity => LocationStatusList.Count(p => p.DisabledStatus == 0 && !p.IsCargoExist);
            public int ZoneTotalSize => LocationStatusList.Count;

            /// <summary>
            /// 0: Shelf,  1: Port,  2: Other,  9: HandOff
            /// </summary>
            public int ZoneType { get; set; } = 0;

            public List<LocationStatus> LocationStatusList { get; set; } = new List<LocationStatus>();


            public class LocationStatus
            {
                public string ShelfId { get; set; } = "ShelfID";
                public string CarrierID { get; set; } = string.Empty;
                public bool IsCargoExist { get; set; } = false;
                /// <summary>
                /// 0:enable,1:disabled;
                /// </summary>
                public int DisabledStatus { get; set; } = 0;
                /// <summary>
                /// 0= None/Dynamic(無限制) ;1 = Normal(DCS可派送，MCS異常處理不可派送);2 = Abnormal(DCS不可派送，MCS異常處理可派送)
                /// </summary>
                public int ProcessState { get; set; } = 0;
            }

        }

        public static async Task<(bool confirm, string message)> ShelfStatusChange(ZoneData zoneData)
        {
            (bool confirm, string message) response = new(false, "[MCSCIMService.AlarmReporter] System Error.");
            try
            {
                var route = $"/api/Carrier/ShelfStatusChange";
                (bool success, string json) v = await _http.PostAsync(route, zoneData);
                response.confirm = v.success;
                response.message = v.json;
            }
            catch (Exception ex)
            {
                response.message = $"[MCSCIMService.AlarmReporter] Report to: {_http.http_client.BaseAddress} with exmessage: {ex.Message}";
            }
            return response;
        }
        public static async Task<(bool confirm, string message)> ZoneCapacityChange(ZoneData zoneData)
        {
            (bool confirm, string message) response = new(false, "[MCSCIMService.AlarmReporter] System Error.");
            try
            {
                var route = $"/api/Carrier/ZoneCapacityChange";
                (bool success, string json) v = await _http.PostAsync(route, zoneData);
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
            await _http.PostAsync<object, object>($"/api/Carrier/CarrierInstallCompleted?CarrierID={CarrierID}&CarrierLoc={CarrierLoc}&CarrierZoneName={CarrierZoneName}&HandoffType={HandoffType}", null, 8);
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
            await _http.PostAsync<object, object>($"/api/Carrier/CarrierRemoveCompleted?CarrierID={CarrierID}&CarrierLoc={CarrierLoc}&CarrierZoneName={CarrierZoneName}&HandoffType={HandoffType}", null, 8);
        }
    }
}
