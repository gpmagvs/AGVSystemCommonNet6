using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.HttpTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices
{
    public class MapSync
    {
        public static async Task<(bool, string)> SendReloadRequest(string map_file_path)
        {
            try
            {
                using HttpHelper http = new HttpHelper(AGVSConfigulator.SysConfigs.VMSHost);
                bool alive = await http.GetAsync<bool>($"/api/Map/Reload?map_file={map_file_path}");
                return (alive, "");
            }
            catch (TaskCanceledException)
            {
                return (false, "Timeout");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
