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
        public string FieldName { get; set; } = "UMTC-AOI-2F";
        public string DBConnection { get; set; } = "Server=127.0.0.1;Database=GPMAGVs;User Id=sa;Password=12345678;Encrypt=False;MultipleActiveResultSets=True;Connection Lifetime=1;Min Pool Size=5;Max Pool Size=50;MultipleActiveResultSets=True;";
        public string PartsAGVSDBConnection { get; set; } = "Server=127.0.0.1;Database=AGVS_Info;User Id=sa;Password=Tsmc12345678;Encrypt=False;MultipleActiveResultSets=True;Connection Lifetime=1;Min Pool Size=5;Max Pool Size=50;MultipleActiveResultSets=True;";
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
        public clsAutoModeConfigs AutoModeConfigs { get; set; } = new clsAutoModeConfigs();
        public clsAutoSendDailyData AutoSendDailyData { get; set; } = new clsAutoSendDailyData();

        public string AGVUpdateFileFolder { get; set; } = "C:\\AGVS\\AGV_Update";

        public string LogFolder { get; set; } = "C:\\AGVSLog";

        public string TrobleShootingFolder { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Resources\\");

        public clsMaterialBufferLevelMonitor MaterialBufferLevelMonitor { get; set; } = new clsMaterialBufferLevelMonitor();

        public bool LinkPartsAGVSystem { get; set; } = false;

        public clsOrderState OrderState { get; set; } = new clsOrderState();

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
        public string EquipmentManagementConfigFolder { get; set; } = "C://AGVS//EquipmentManagement";
        /// <summary>
        /// 當設備維修時封閉進入點
        /// </summary>
        public bool DisableEntryPointWhenEQMaintaining { get; set; } = false;
        /// <summary>
        /// 當設備零件更換時封閉進入點
        /// </summary>
        public bool DisableEntryPointWhenEQPartsReplacing { get; set; } = false;

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

        public bool UnLockEntryPointWhenParkAtEquipment { get; set; } = true;
        public bool MultiRegionNavigation { get; set; } = true;

        public int SegmentTrajectoryPointNum { get; set; } = 3;

    }
    public class clsAutoSendDailyData
    {
        public string SavePath { get; set; } = @"d:\DailyData\";
        public int SaveTime { get; set; } = 1;
    }

    public class clsMaterialBufferLevelMonitor
    {
        public bool MonitorSwitch { get; set; } = false;
        public int LevelThreshold { get; set; } = 0;
    }

    public class clsOrderState
    {
        /// <summary>
        /// 任務沒有被執行的超時時間(單位:分鐘)
        /// </summary>
        public double TaskNoExecutedTimeout { get; set; } = 20;
    }
}