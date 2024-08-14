﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.ViewModels
{
    public class WebFunctionViewPermissions
    {
        public Menu menu { get; set; } = new Menu();
        public DataQuerySubMenu dataQuerySubMenu { get; set; } = new DataQuerySubMenu();
        public SystemConfigurationSubMenu systemConfigurationSubMenu { get; set; } = new SystemConfigurationSubMenu();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public class Menu
        {
            public int SystemAlarm { get; set; } = 1;
            public int WIPInfo { get; set; } = 1;
            public int VehicleManagnment { get; set; } = 1;
            public int Map { get; set; } = 1;
            public int DataQuery { get; set; } = 1;
            public int HotRun { get; set; } = 1;
            public int SystemConfiguration { get; set; } = 1;
        }

        public class DataQuerySubMenu
        {
            public int TaskHistory { get; set; } = 1;
            public int AlarmHistory { get; set; } = 1;
            public int VehicleTrajectory { get; set; } = 1;
            public int InstrumentsMeasure { get; set; } = 1;
            public int Utilization { get; set; } = 1;
        }
        public class SystemConfigurationSubMenu
        {
            public int BatteryLevelManagnment { get; set; } = 1;
            public int EquipmentlManagnment { get; set; } = 1;
            public int RackManagnment { get; set; } = 1;
            public int UserManagnment { get; set; } = 1;
            public int ChargerManagnment { get; set; } = 1;

        }
    }
}
