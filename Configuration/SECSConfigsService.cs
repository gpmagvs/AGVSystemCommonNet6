using AGVSystemCommonNet6.Microservices.MCSCIM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Configuration
{
    public class SECSConfigsService
    {
        public SECSConfiguration baseConfiguration => AGVSConfigulator.SysConfigs.SECSGem;
        public SECSAlarmConfiguration alarmConfiguration { get; private set; } = new SECSAlarmConfiguration();

        public readonly string configsSaveFolder = @"C:\AGVS\SECSConfigs";

        public string alarmConfigFilePath => Path.Combine(configsSaveFolder, "SECS_Alarm_Settings.json");

        public SECSConfigsService(string configsSaveFolder)
        {
            this.configsSaveFolder = configsSaveFolder;
            Initialize();
        }
        public SECSConfigsService()
        {
            Initialize();
        }

        private void Initialize()
        {
            CreateDirectory();
            alarmConfiguration = LoadAlarmConfig();
        }

        private SECSAlarmConfiguration LoadAlarmConfig()
        {
            try
            {
                return LoadConfig<SECSAlarmConfiguration>(alarmConfigFilePath);
            }
            catch
            {
                SECSAlarmConfiguration defaultConfig = new SECSAlarmConfiguration();
                CreateNewConfig(defaultConfig, alarmConfigFilePath);
                return defaultConfig;
            }
        }

        private T LoadConfig<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(alarmConfigFilePath);
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new FileNotFoundException($"{filePath} not exist.");
            }
        }

        private void CreateNewConfig(object defaultObj, string filePath)
        {
            CreateDirectory();
            File.WriteAllText(filePath, JsonConvert.SerializeObject(defaultObj, Formatting.Indented));
        }

        private void CreateDirectory()
        {
            Directory.CreateDirectory(configsSaveFolder);
        }
    }
}
