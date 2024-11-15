using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.AGVS
{
    public static partial class AGVSSerivces
    {
        public static async Task CargoUnloadFromPortDoneReport(int tag, int slot)
        {
            HttpTools.HttpHelper agvs_http = GetAGVSHttpHelper();
            await agvs_http.PostAsync($"/api/AGVCargoTransfer/UnloadCargoFromPort?tagNumber={tag}&slot={slot}", new { }, 10);
        }

        public static async Task CargoLoadToPortDoneReport(int tag, int slot, string cargoID)
        {
            HttpTools.HttpHelper agvs_http = GetAGVSHttpHelper();
            await agvs_http.PostAsync($"/api/AGVCargoTransfer/LoadCargoToPort?tagNumber={tag}&slot={slot}&cargoID={cargoID}", new { }, 10);
        }
    }
}
