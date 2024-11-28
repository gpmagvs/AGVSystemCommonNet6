using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.Microservices.MCS.MCSCIMService;

namespace AGVSystemCommonNet6.Microservices.MCS
{
    public partial class MCSCIMService
    {
        public static async Task VehicleActiveReport(string commandId, string vehicleId, string carrierId)
        {
            await _http.PostAsync($"/api/VehicleStateReport/active?commandId={commandId}&vehicleId={vehicleId}&carrierId={carrierId}", null);
        }

        public static async Task VehicleIdleReport(string commandId, string vehicleId, string carrierId)
        {
            await _http.PostAsync($"/api/VehicleStateReport/idle?commandId={commandId}&vehicleId={vehicleId}&carrierId={carrierId}", null);
        }

        public static async Task VehicleOutOfServiceReport(string vehicleId)
        {
            await _http.PostAsync($"/api/VehicleStateReport/outOfService?vehicleId={vehicleId}", null);
        }

        public static async Task VehicleInServiceReport(string vehicleId)
        {
            await _http.PostAsync($"/api/VehicleStateReport/inService?vehicleId={vehicleId}", null);
        }

        public static async Task VehicleAcquireStartedReport(string vehicleId, string carrierId, string transferPort)
        {
            await _http.PostAsync($"/api/VehicleStateReport/acquireStarted?vehicleId={vehicleId}&carrierId={carrierId}&transferPort={transferPort}", null);
        }

        public static async Task VehicleAcquireCompletedReport(string vehicleId, string carrierId, string transferPort)
        {
            await _http.PostAsync($"/api/VehicleStateReport/acquireCompleted?vehicleId={vehicleId}&carrierId={carrierId}&transferPort={transferPort}", null);
        }

        public static async Task VehicleDepositStartedReport(string vehicleId, string carrierId, string transferPort)
        {
            await _http.PostAsync($"/api/VehicleStateReport/depositStarted?vehicleId={vehicleId}&carrierId={carrierId}&transferPort={transferPort}", null);
        }

        public static async Task VehicleDepositCompletedReport(string vehicleId, string carrierId, string transferPort)
        {
            await _http.PostAsync($"/api/VehicleStateReport/depositCompleted?vehicleId={vehicleId}&carrierId={carrierId}&transferPort={transferPort}", null);
        }

        public static async Task VehicleAssignedReport(string vehicleId, string commandId)
        {
            await _http.PostAsync($"/api/VehicleStateReport/assigned?vehicleId={vehicleId}&commandId={commandId}", null);
        }

        public static async Task VehicleUnassignedReport(string vehicleId, string commandId)
        {
            await _http.PostAsync($"/api/VehicleStateReport/unassigned?vehicleId={vehicleId}&commandId={commandId}", null);
        }

        public static async Task VehicleDepartedReport(string vehicleId, string transferPort)
        {
            await _http.PostAsync($"/api/VehicleStateReport/departed?vehicleId={vehicleId}&transferPort={transferPort}", null);
        }

        public static async Task VehicleArrivedReport(string vehicleId, string transferPort)
        {
            await _http.PostAsync($"/api/VehicleStateReport/arrived?vehicleId={vehicleId}&transferPort={transferPort}", null);
        }

        public static async Task VehicleChargeStartedReport(string vehicleId, string chargerId)
        {
            await _http.PostAsync($"/api/VehicleStateReport/chargeStarted?vehicleId={vehicleId}&chargerId={chargerId}", null);
        }

        public static async Task VehicleChargeEndReport(string vehicleId, string chargerId)
        {
            await _http.PostAsync($"/api/VehicleStateReport/chargeEnd?vehicleId={vehicleId}&chargerId={chargerId}", null);
        }

        public static async Task VehicleCoordinateChangedReport(string vehicleId, string x, string y)
        {
            await _http.PostAsync($"/api/VehicleStateReport/coordinateChanged?vehicleId={vehicleId}&x={x}&y={y}", null);
        }
    }
}
