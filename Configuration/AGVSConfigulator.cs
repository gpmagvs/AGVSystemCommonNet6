using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace AGVSystemCommonNet6.Configuration
{
    public class AGVSConfigulator
    {
        public static string ConfigsFilesFolder { get; set; } = @"C:\AGVS";
        public static string _configFilePath { get; private set; } = "";
        public static SystemConfigs SysConfigs { get; set; }

        private static FileSystemWatcher SysConfigFileWatcher;

        private static SemaphoreSlim SysConfigFIleSemaphoreSlim = new SemaphoreSlim(1, 1);

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void LoadConfig()
        {
            SysConfigs = LoadConfig(_configFilePath);
        }
        public static SystemConfigs LoadConfig(string configFilePath)
        {
            SystemConfigs systemConfigs = new SystemConfigs();
            if (File.Exists(configFilePath))
            {
                systemConfigs = JsonConvert.DeserializeObject<SystemConfigs>(File.ReadAllText(configFilePath));
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
            }
            systemConfigs.CONFIGS_ROOT_FOLDER = Path.GetDirectoryName(configFilePath);
            string json = JsonConvert.SerializeObject(systemConfigs, Formatting.Indented);
            File.WriteAllText(configFilePath, json);
            Console.WriteLine(json);
            return systemConfigs;
        }
        public static void Init(string configsFolder)
        {

            ConfigsFilesFolder = string.IsNullOrEmpty(configsFolder) || !Directory.Exists(configsFolder) ? ConfigsFilesFolder : configsFolder;
            _configFilePath = Path.Combine(ConfigsFilesFolder, "SystemConfigs.json");
            SysConfigs = LoadConfig(_configFilePath);
            //WatchSystemConfigs(_configFilePath);
        }

        private static void WatchSystemConfigs(string _configFilePath)
        {
            SysConfigFileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_configFilePath), Path.GetFileName(_configFilePath));
            SysConfigFileWatcher.Changed += Watcher_Changed;
            SysConfigFileWatcher.EnableRaisingEvents = true;
        }

        private static async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            SysConfigFileWatcher.EnableRaisingEvents = false;
            //copy the file as temp file and load it
            string _tempConfigFilePath = _configFilePath + $"{DateTime.Now.Ticks}.tmp";
            File.Copy(_configFilePath, _tempConfigFilePath, true);
            SysConfigs = JsonConvert.DeserializeObject<SystemConfigs>(File.ReadAllText(_tempConfigFilePath));
            File.Delete(_tempConfigFilePath);
            await Task.Delay(100);
            SysConfigFileWatcher.EnableRaisingEvents = true;
        }

        internal static void Save(SystemConfigs config)
        {
            File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public static async Task<string> GetTrayUnknownFlowID()
        {
            int flowNumber = await AGVSConfigulator.GetTrayUnknowFlowNumber();
            string unknowCargoID = $"TUN{AGVSConfigulator.SysConfigs.SECSGem.SystemID}{flowNumber.ToString("D5")}";
            return unknowCargoID;
        }
        public static async Task<string> GetRackUnknownFlowID()
        {
            int flowNumber = await AGVSConfigulator.GetRackUnknowFlowNumber();
            string unknowCargoID = $"UN{AGVSConfigulator.SysConfigs.SECSGem.SystemID}{flowNumber.ToString("D5")}";
            return unknowCargoID;
        }
        public static async Task<string> GetDoubleUnknownFlowID()
        {
            int flowNumber = await AGVSConfigulator.GetRackDoubleUnknowFlowNumber();
            string unknowCargoID = $"DU{AGVSConfigulator.SysConfigs.SECSGem.SystemID}{flowNumber.ToString("D5")}";
            return unknowCargoID;
        }

        private static async Task<int> GetTrayUnknowFlowNumber()
        {
            try
            {
                await SysConfigFIleSemaphoreSlim.WaitAsync();
                SysConfigs.SECSGem.UnknowTrayIDFlowNumberUsed += 1;
                Save(SysConfigs);
                return SysConfigs.SECSGem.UnknowTrayIDFlowNumberUsed;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return 0;
            }
            finally
            {
                SysConfigFIleSemaphoreSlim.Release();
            }
        }

        private static async Task<int> GetRackUnknowFlowNumber()
        {
            try
            {
                await SysConfigFIleSemaphoreSlim.WaitAsync();
                SysConfigs.SECSGem.UnknowRackIDFlowNumberUsed += 1;
                Save(SysConfigs);
                return SysConfigs.SECSGem.UnknowRackIDFlowNumberUsed;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return 0;
            }
            finally
            {
                SysConfigFIleSemaphoreSlim.Release();
            }
        }

        private static async Task<int> GetRackDoubleUnknowFlowNumber()
        {
            try
            {
                await SysConfigFIleSemaphoreSlim.WaitAsync();
                SysConfigs.SECSGem.DoubleUnknowDFlowNumberUsed += 1;
                Save(SysConfigs);
                return SysConfigs.SECSGem.DoubleUnknowDFlowNumberUsed;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return 0;
            }
            finally
            {
                SysConfigFIleSemaphoreSlim.Release();
            }
        }

    }
}

