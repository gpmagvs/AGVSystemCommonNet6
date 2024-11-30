using AGVSystemCommonNet6.HttpTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.MCS
{
    public partial class MCSCIMService
    {
        public class TransportCommandDto
        {
            public string CommandID { get; set; } = "";
            public string CarrierID { get; set; } = "";
            public string CarrierLoc { get; set; } = "";
            public string CarrierZoneName { get; set; } = "";
            public string Dest { get; set; } = "";
            /// <summary>
            /// 搬運結果: 
            /// 0= Successful | 1 = Other errors | 2 = Zone is full | 3 = Duplicate ID | 4 = ID mismatch | 5 = ID Read Failed | 64 = Interlock Error
            /// </summary>
            public int ResultCode { get; set; } = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CommandID">任務名稱</param>
        /// <param name="CarrierID">貨物ID</param>
        /// <param name="CarrierLoc">來源Rack Port ID</param>
        /// <param name="CarrierZoneName">來源料架ID</param>
        /// <param name="Dest">目的地</param>
        /// <returns></returns>
        public static async Task TransferInitiatedReport(TransportCommandDto commandDto)
        {
            await _http.PostAsync($"/api/TransportEventReport/TransferInitiated?" +
                $"CommandID={commandDto.CommandID}&" +
                $"CarrierID={commandDto.CarrierID}&" +
                $"CarrierLoc={commandDto.CarrierLoc}&" +
                $"CarrierZoneName={commandDto.CarrierZoneName}&" +
                $"Dest={commandDto.Dest}", null);
        }
        public static async Task TransferringReport(TransportCommandDto commandDto)
        {
            await _http.PostAsync($"/api/TransportEventReport/Transferring?CommandID={commandDto.CommandID}&CarrierID={commandDto.CarrierID}&CarrierLoc={commandDto.CarrierLoc}&CarrierZoneName={commandDto.CarrierZoneName}&Dest={commandDto.Dest}", null);
        }
        public static async Task TransferCompletedReport(TransportCommandDto commandDto)
        {
            await _http.PostAsync($"/api/TransportEventReport/TransferCompleted?CommandID={commandDto.CommandID}&CarrierID={commandDto.CarrierID}&CarrierLoc={commandDto.CarrierLoc}&CarrierZoneName={commandDto.CarrierZoneName}&Dest={commandDto.Dest}&ResultCode={commandDto.ResultCode}", null);
        }
        public static async Task TransferAbortInitiatedReport(TransportCommandDto commandDto)
        {
            await _http.PostAsync($"/api/TransportEventReport/TransferAbortInitiated?CommandID={commandDto.CommandID}&CarrierID={commandDto.CarrierID}&CarrierLoc={commandDto.CarrierLoc} &CarrierZoneName={commandDto.CarrierZoneName}&Dest={commandDto.Dest}", null);
        }
        public static async Task TransferCancelInitiatedReport(TransportCommandDto commandDto)
        {
            await _http.PostAsync($"/api/TransportEventReport/TransferCancelInitiated?CommandID={commandDto.CommandID}&CarrierID={commandDto.CarrierID}&CarrierLoc={commandDto.CarrierLoc} &CarrierZoneName={commandDto.CarrierZoneName}&Dest={commandDto.Dest}", null);
        }
        public static async Task TransferAbortFailedReport(TransportCommandDto commandDto)
        {
            await _http.PostAsync($"/api/TransportEventReport/TransferAbortFailed?CommandID={commandDto.CommandID}&CarrierID={commandDto.CarrierID}&CarrierLoc={commandDto.CarrierLoc}&CarrierZoneName={commandDto.CarrierZoneName}&Dest={commandDto.Dest}", null);
        }
        public static async Task TransferCancelFailedReport(TransportCommandDto commandDto)
        {
            await _http.PostAsync($"/api/TransportEventReport/TransferCancelFailed?CommandID={commandDto.CommandID}&CarrierID={commandDto.CarrierID}&CarrierLoc={commandDto.CarrierLoc}&CarrierZoneName=  {commandDto.CarrierZoneName}  &Dest=  {commandDto.Dest}", null);
        }


    }
}
