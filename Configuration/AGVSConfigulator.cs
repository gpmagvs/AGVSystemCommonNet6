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
        public static string ConfigsFilesFolder { get; set; } = @"C:\AGVS";
        public static string _configFilePath { get; private set; } = "";
        public static SystemConfigs SysConfigs { get; set; }
        public static void LoadConfig()
        {
            SysConfigs = LoadConfig(_configFilePath);
        }
        public static SystemConfigs LoadConfig(string configFilePath)
        {
            _configFilePath = configFilePath;
            SystemConfigs systemConfigs = new SystemConfigs();
            if (File.Exists(configFilePath))
            {
                systemConfigs = JsonConvert.DeserializeObject<SystemConfigs>(File.ReadAllText(configFilePath));
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
            }
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(systemConfigs, Formatting.Indented));
            return systemConfigs;
        }
        public static void Init()
        {
            SysConfigs = LoadConfig(Path.Combine(ConfigsFilesFolder, "SystemConfigs.json"));
        }

        internal static void Save(SystemConfigs config)
        {
            File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }
}

