using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AGVSystemCommonNet6.Configuration
{
    public class AGVSConfigulator
    {
        public static SystemConfigs LoadConfig(string configFilePath)
        {
            if (File.Exists(configFilePath))
            {
                return JsonConvert.DeserializeObject<SystemConfigs>(File.ReadAllText(configFilePath));
            }
            else
            {
                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(new SystemConfigs(), Formatting.Indented));
                return new SystemConfigs();
            }
        }
        public static SystemConfigs SysConfigs { get; set; }
        public static void Init()
        {
            SysConfigs = LoadConfig("C:\\AGVS\\SystemConfigs.json");
        }
    }
}

