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

        private static FileSystemWatcher SysConfigFileWatcher;

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
    }
}

