using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.Microservices.MCS.MCSCIMService;

namespace AGVSystemCommonNet6.Microservices.MCS
{
    public partial class MCSCIMService
    {
        /// <summary>
        /// [CEID=201] 
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="carrierId"></param>
        /// <returns></returns>
        public static async Task VehicleActiveReport(string commandId, string vehicleId, string carrierId)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/active?commandId={commandId}&vehicleId={vehicleId}&carrierId={carrierId}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=202] 
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="carrierId"></param>
        /// <returns></returns>
        public static async Task VehicleIdleReport(string commandId, string vehicleId, string carrierId)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/idle?commandId={commandId}&vehicleId={vehicleId}&carrierId={carrierId}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        /// <summary>
        /// [CEID=205] AGV當機時上報
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public static async Task VehicleOutOfServiceReport(string vehicleId)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/outOfService?vehicleId={vehicleId}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        /// <summary>
        /// [CEID=206] 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public static async Task VehicleInServiceReport(string vehicleId)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/inService?vehicleId={vehicleId}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=210] AGV開始進行Unload(車子從設備取料)動作時上報
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="carrierId"></param>
        /// <param name="transferPort"></param>
        /// <returns></returns>
        public static async Task VehicleAcquireStartedReport(string vehicleId, string carrierId, string transferPort)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/acquireStarted?vehicleId={vehicleId}&carrierId={carrierId}&transferPort={transferPort}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=211] 完成Unload(車子從設備取料)
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="carrierId"></param>
        /// <param name="transferPort"></param>
        /// <returns></returns>
        public static async Task VehicleAcquireCompletedReport(string vehicleId, string carrierId, string transferPort)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/acquireCompleted?vehicleId={vehicleId}&carrierId={carrierId}&transferPort={transferPort}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=212]  車輛開始進行Load(車子放料到設備)動作時上報
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="carrierId"></param>
        /// <param name="transferPort"></param>
        /// <returns></returns>
        public static async Task VehicleDepositStartedReport(string vehicleId, string carrierId, string transferPort)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/depositStarted?vehicleId={vehicleId}&carrierId={carrierId}&transferPort={transferPort}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        /// <summary>
        /// [CEID=213] 完成Load(車子放料到設備)
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="carrierId"></param>
        /// <param name="transferPort"></param>
        /// <returns></returns>
        public static async Task VehicleDepositCompletedReport(string vehicleId, string carrierId, string transferPort)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/depositCompleted?vehicleId={vehicleId}&carrierId={carrierId}&transferPort={transferPort}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=220] 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public static async Task VehicleAssignedReport(string vehicleId, string commandId)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/assigned?vehicleId={vehicleId}&commandId={commandId}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        /// <summary>
        /// [CEID=221] 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public static async Task VehicleUnassignedReport(string vehicleId, string commandId)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/unassigned?vehicleId={vehicleId}&commandId={commandId}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=222] 車子離開工作站時上報(比如從來源設備取到貨，開始移動去目的地時?)
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="transferPort"></param>
        /// <returns></returns>
        public static async Task VehicleDepartedReport(string vehicleId, string transferPort)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/departed?vehicleId={vehicleId}&transferPort={transferPort}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=223] 搬運過程中-當車子抵達要取放或的位置時上報事件(工作站進入點)
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="transferPort"></param>
        /// <returns></returns>
        public static async Task VehicleArrivedReport(string vehicleId, string transferPort)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/arrived?vehicleId={vehicleId}&transferPort={transferPort}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=2230] 充電開始事件 (從KGS的LOG沒有看到有上報此事件)
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="chargerId"></param>
        /// <returns></returns>
        public static async Task VehicleChargeStartedReport(string vehicleId, string chargerId)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/chargeStarted?vehicleId={vehicleId}&chargerId={chargerId}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=231] 充電結束事件 (從KGS的LOG沒有看到有上報此事件)
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="chargerId"></param>
        /// <returns></returns>
        public static async Task VehicleChargeEndReport(string vehicleId, string chargerId)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/chargeEnd?vehicleId={vehicleId}&chargerId={chargerId}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// [CEID=241] 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static async Task VehicleCoordinateChangedReport(string vehicleId, string x, string y)
        {
            try
            {
                await _http.PostAsync($"/api/VehicleStateReport/coordinateChanged?vehicleId={vehicleId}&x={x}&y={y}", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
