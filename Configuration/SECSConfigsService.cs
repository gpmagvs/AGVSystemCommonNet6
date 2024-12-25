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

        public TransferReportConfiguration transferReportConfiguration { get; private set; } = new TransferReportConfiguration();

        public readonly string configsSaveFolder = @"C:\AGVS\SECSConfigs";

        public string alarmConfigFilePath => Path.Combine(configsSaveFolder, "SECS_Alarm_Settings.json");
        public string transferReportConfigFilePath => Path.Combine(configsSaveFolder, "SECS_Transfer_Report.json");

        public SECSConfigsService(string configsSaveFolder)
        {
            this.configsSaveFolder = configsSaveFolder;
            Initialize();
        }
        public SECSConfigsService()
        {
        }
        public void Reload()
        {
            Initialize();
        }
        private void Initialize()
        {
            CreateDirectory();
            alarmConfiguration = LoadAlarmConfig();
            transferReportConfiguration = LoadTransferReportConfig();
            UpdateCofigurationFile(alarmConfiguration, alarmConfigFilePath);
            UpdateCofigurationFile(transferReportConfiguration, transferReportConfigFilePath);
        }

        private TransferReportConfiguration LoadTransferReportConfig()
        {
            try
            {
                return LoadConfig<TransferReportConfiguration>(transferReportConfigFilePath);
            }
            catch
            {
                TransferReportConfiguration defaultConfig = new();
                UpdateCofigurationFile(defaultConfig, transferReportConfigFilePath);
                return defaultConfig;
            }
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
                UpdateCofigurationFile(defaultConfig, alarmConfigFilePath);
                return defaultConfig;
            }
        }

        private T LoadConfig<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(filePath);
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

        private void UpdateCofigurationFile(object defaultObj, string filePath)
        {
            CreateDirectory();
            File.WriteAllText(filePath, JsonConvert.SerializeObject(defaultObj, Formatting.Indented));
        }

        private void CreateDirectory()
        {
            Directory.CreateDirectory(configsSaveFolder);
        }

        public void UpdateReturnCodes(TransferReportConfiguration.clsResultCodes transferCompletedResultCodes)
        {
            this.transferReportConfiguration.ResultCodes = transferCompletedResultCodes;
            UpdateCofigurationFile(transferReportConfiguration, transferReportConfigFilePath);
        }
    }
}
