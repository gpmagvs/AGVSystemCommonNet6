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
            SystemConfigs systemConfigs = new SystemConfigs();
            if (File.Exists(configFilePath))
            {
                systemConfigs= JsonConvert.DeserializeObject<SystemConfigs>(File.ReadAllText(configFilePath));
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
            }
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(systemConfigs, Formatting.Indented));
            return systemConfigs;
        }
        public static SystemConfigs SysConfigs { get; set; }
        public static void Init()
        {
            SysConfigs = LoadConfig("C:\\AGVS\\SystemConfigs.json");
        }
    }
}

