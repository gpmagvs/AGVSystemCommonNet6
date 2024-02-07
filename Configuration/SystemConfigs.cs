using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Configuration
{
    public class SystemConfigs
    {
        public string DBConnection { get; set; } = "Server=127.0.0.1;Database=GPMAGVs;User Id=sa;Password=12345678;Encrypt=False";
        public string VMSHost { get; set; } = "http://localhost:5036";
        public string AGVSHost { get; set; } = "http://localhost:5216";

        public string VMSTcpServerIP { get; set; } = "127.0.0.1";
        public int VMSTcpServerPort { get; set; } = 5500;

        /// <summary>
        /// 前端用戶閒置超過此秒數後自動登出。
        /// 單位:秒
        /// </summary>
        public int WebUserLogoutExipreTime { get; set; } = 300;
        public clsMapConfigs MapConfigs { get; set; } = new clsMapConfigs();
        public clsEquipmentManagementConfigs EQManagementConfigs { get; set; } = new clsEquipmentManagementConfigs();
        public clsAGVTaskControlConfigs TaskControlConfigs { get; set; } = new clsAGVTaskControlConfigs();
        public clsAutoModeConfigs AutoModeConfigs { get; set; } = new clsAutoModeConfigs();
    }
    public class clsMapConfigs
    {
        public string MapFolder { get; set; } = "C://AGVS//Map";
        public string CurrentMapFileName { get; set; } = "Map_UMTC_3F_MEC.json";
        public string MapRegionConfigFile { get; set; } = "C://AGVS//Map//MapRegions.json";

        [JsonIgnore]
        public string MapFileFullName => Path.Combine(MapFolder, CurrentMapFileName);

    }
    public class clsEquipmentManagementConfigs
    {
        public bool UseEQEmu { get; set; } = false;
        public string EquipmentManagementConfigFolder { get; set; } = "C://AGVS//EquipmentManagement_MEC";

    }

    public class clsAutoModeConfigs
    {
        /// <summary>
        /// AGV閒置多久後自動產生充電任務
        /// </summary>
        public int AGVIdleTimeUplimitToExecuteChargeTask { get; set; } = 3;
    }

    public class clsAGVTaskControlConfigs
    {
        public bool CheckAGVCargoStatusWhenLDULDAction { get; set; } = true;
    }
}
