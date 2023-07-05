using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP.YunTech
{
    public static class YunTechMapManager
    {
        public static Dictionary<string, Dictionary<string, clsYuntechSubMap>> LoadMapFromFile(string mapFile)
        {
            string json = File.ReadAllText(mapFile);
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, clsYuntechSubMap>>>(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
