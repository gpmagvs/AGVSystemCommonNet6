using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.AGVS
{
    public static partial class AGVSSerivces
    {
        public static async Task CargoUnloadFromPortDoneReport(string taskName, string agvName, int tag, int slot)
        {
            HttpTools.HttpHelper agvs_http = GetAGVSHttpHelper();
            await agvs_http.PostAsync($"/api/AGVCargoTransfer/UnloadCargoFromPort?taskName={taskName}&agvName={agvName}&tagNumber={tag}&slot={slot}", new { }, 10);
        }

        public static async Task CargoLoadToPortDoneReport(string taskName, string agvName, int tag, int slot, string cargoID)
        {
            HttpTools.HttpHelper agvs_http = GetAGVSHttpHelper();
            await agvs_http.PostAsync($"/api/AGVCargoTransfer/LoadCargoToPort?taskName={taskName}&agvName={agvName}&tagNumber={tag}&slot={slot}&cargoID={cargoID}", new { }, 10);
        }
    }
}
