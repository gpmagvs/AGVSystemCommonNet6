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

        public static void Save(SystemConfigs config)
        {
            File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public static async Task<string> GetTrayUnknownFlowID()
        {
            int flowNumber = await GetTrayUnknowFlowNumber();
            string unknowCargoID = $"TUN{SysConfigs.SECSGem.SystemID}{flowNumber.ToString("D5")}";
            return unknowCargoID;
        }
        public static async Task<string> GetRackUnknownFlowID()
        {
            int flowNumber = await GetRackUnknowFlowNumber();
            string unknowCargoID = $"DUN{SysConfigs.SECSGem.SystemID}{flowNumber.ToString("D5")}";
            return unknowCargoID;
        }
        public static async Task<string> GetDoubleTrayUnknownFlowID()
        {
            int flowNumber = await GetTrayDoubleUnknowFlowNumber();
            string unknowCargoID = $"TDU{SysConfigs.SECSGem.SystemID}{flowNumber.ToString("D5")}";
            return unknowCargoID;
        }

        public static async Task<string> GetDoubleRackUnknownFlowID()
        {
            int flowNumber = await GetRackDoubleUnknowFlowNumber();
            string unknowCargoID = $"DDU{SysConfigs.SECSGem.SystemID}{flowNumber.ToString("D5")}";
            return unknowCargoID;
        }
        public static async Task<string> GetTrayMissMatchFlowID()
        {
            int flowNumber = await GetTrayMissMatchFlowNumber();
            string unknowCargoID = $"TMI{SysConfigs.SECSGem.SystemID}{flowNumber.ToString("D5")}";
            return unknowCargoID;
        }

        public static async Task<string> GetRackMissMatchFlowID()
        {
            int flowNumber = await GetRackMissMatchFlowNumber();
            string unknowCargoID = $"DMI{SysConfigs.SECSGem.SystemID}{flowNumber.ToString("D5")}";
            return unknowCargoID;
        }
        private static async Task<int> GetTrayUnknowFlowNumber()
        {
            try
            {
                await SysConfigFIleSemaphoreSlim.WaitAsync();
                SysConfigs.SECSGem.UnknowTrayIDFlowNumberUsed += 1;
                if (SysConfigs.SECSGem.UnknowTrayIDFlowNumberUsed > 99999)
                {
                    SysConfigs.SECSGem.UnknowTrayIDFlowNumberUsed = 1;
                }
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
                if (SysConfigs.SECSGem.UnknowRackIDFlowNumberUsed > 99999)
                {
                    SysConfigs.SECSGem.UnknowRackIDFlowNumberUsed = 1;
                }
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

        private static async Task<int> GetTrayDoubleUnknowFlowNumber()
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


        private static async Task<int> GetRackDoubleUnknowFlowNumber()
        {
            try
            {
                await SysConfigFIleSemaphoreSlim.WaitAsync();
                SysConfigs.SECSGem.DoubleUnknowRackIDFlowNumberUsed += 1;
                Save(SysConfigs);
                return SysConfigs.SECSGem.DoubleUnknowRackIDFlowNumberUsed;
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


        private static async Task<int> GetTrayMissMatchFlowNumber()
        {
            try
            {
                await SysConfigFIleSemaphoreSlim.WaitAsync();
                SysConfigs.SECSGem.MissMatchTrayIDFlowNumberUsed += 1;
                if (SysConfigs.SECSGem.MissMatchTrayIDFlowNumberUsed > 99999)
                {
                    SysConfigs.SECSGem.MissMatchTrayIDFlowNumberUsed = 1;
                }
                Save(SysConfigs);
                return SysConfigs.SECSGem.MissMatchTrayIDFlowNumberUsed;
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


        private static async Task<int> GetRackMissMatchFlowNumber()
        {
            try
            {
                await SysConfigFIleSemaphoreSlim.WaitAsync();
                SysConfigs.SECSGem.MissMatchRackIDFlowNumberUsed += 1;
                if (SysConfigs.SECSGem.MissMatchRackIDFlowNumberUsed > 99999)
                {
                    SysConfigs.SECSGem.MissMatchRackIDFlowNumberUsed = 1;
                }
                Save(SysConfigs);
                return SysConfigs.SECSGem.MissMatchRackIDFlowNumberUsed;
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

