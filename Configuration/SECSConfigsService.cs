using AGVSystemCommonNet6.Microservices.MCSCIM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
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

        public SECSConfiguration SECSConfigs { get; set; } = new SECSConfiguration();
        public readonly string SECSConfigsSaveFolder = @"C:\AGVS";

        public readonly string configsSaveFolder = @"C:\AGVS\SECSConfigs";
        public string SECSConfigsFilePath => Path.Combine(SECSConfigsSaveFolder, "SystemConfigs.json");

        public string alarmConfigFilePath => Path.Combine(configsSaveFolder, "SECS_Alarm_Settings.json");
        public string transferReportConfigFilePath => Path.Combine(configsSaveFolder, "SECS_Transfer_Report.json");

        private static SemaphoreSlim _initializeSemaphoreSlim = new SemaphoreSlim(1, 1);

        public static Logger logger = LogManager.GetCurrentClassLogger();

        public SECSConfigsService(string configsSaveFolder) : this()
        {
            this.configsSaveFolder = configsSaveFolder;
        }
        public SECSConfigsService()
        {
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _initializeSemaphoreSlim.WaitAsync();
                CreateDirectory();
                alarmConfiguration = LoadAlarmConfig();
                transferReportConfiguration = LoadTransferReportConfig();
                UpdateCofigurationFile(alarmConfiguration, alarmConfigFilePath);
                UpdateCofigurationFile(transferReportConfiguration, transferReportConfigFilePath);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "SECSConfigsService InitializeAsync Error");
            }
            finally
            {
                _initializeSemaphoreSlim.Release();
            }
        }

        private TransferReportConfiguration LoadTransferReportConfig()
        {
            try
            {
                return LoadConfig<TransferReportConfiguration>(transferReportConfigFilePath);
            }
            catch (FileNotFoundException ex)
            {
                TransferReportConfiguration defaultConfig = new();
                UpdateCofigurationFile(defaultConfig, transferReportConfigFilePath);
                return defaultConfig;
            }
            catch
            {
                return transferReportConfiguration;
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
        private void CheckSECSCofigurationFile(object defaultObj, string filePath)
        {
            // 確保目錄存在
            // CreateDirectory(filePath);

            // 如果文件存在，讀取內容
            JObject config = new JObject();
            if (File.Exists(filePath))
            {
                string existingContent = File.ReadAllText(filePath);
                config = JObject.Parse(existingContent);
            }

            // 檢查 SECSGem 節點是否存在，並更新內容
            if (config["SECSGem"] == null || !JToken.DeepEquals(config["SECSGem"], JToken.FromObject(defaultObj)))
            {
                config["SECSGem"] = JToken.FromObject(defaultObj);
                string newContent = JsonConvert.SerializeObject(config, Formatting.Indented);

                // 寫入更新後的內容
                File.WriteAllText(filePath, newContent);
                Console.WriteLine("SECSGem 配置已更新！");
            }
            else
            {
                Console.WriteLine("SECSGem 配置相同，無需更新。");
            }
        }
        private void CreateDirectory()
        {
            Directory.CreateDirectory(configsSaveFolder);
        }

        public void UpdateReturnCodes(TransferReportConfiguration.clsResultCodes transferCompletedResultCodes)
        {
            this.transferReportConfiguration.ResultCodes = transferCompletedResultCodes;
            UpdateCofigurationFile(transferReportConfiguration, transferReportConfigFilePath);
            //CheckCofigurationFile(transferReportConfiguration, transferReportConfigFilePath);
        }
        public void UpdateSECSGemConfigs(SECSConfiguration SECSConfig)
        {
            //this.SECSConfigs = SECSGemConfigSetting;
            this.SECSConfigs = SECSConfig;

            CheckSECSCofigurationFile(SECSConfig, SECSConfigsFilePath);
            //this.baseConfiguration.SECSGemConfigSetting = SECSGemConfigSetting;
            //UpdateCofigurationFile(transferReportConfiguration, transferReportConfigFilePath);
            //CheckCofigurationFile(transferReportConfiguration, transferReportConfigFilePath);
        }

    }
}
