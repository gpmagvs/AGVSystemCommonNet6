﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.AGVDispatch.Model
{
    public class clsCoordination
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Theta { get; set; }
    }
    public class clsAlarmCode
    {
        public virtual int Alarm_ID { get; set; }

        /// <summary>
        ///  1: Serious, 0: Light
        /// </summary>
        public virtual int Alarm_Level { get; set; }

        /// <summary>
        /// 0: Task Recoverable, other: Unrecoverable
        /// </summary>
        public virtual int Alarm_Category { get; set; }
        public virtual string Alarm_Description { get; set; } = "";
    }

    public class clsRunningStatus
    {
        public virtual string Time_Stamp { get; set; } = DateTime.Now.ToAGVSTimeFormat();

        public virtual clsCoordination Coordination { get; set; } = new clsCoordination();
        public virtual int Last_Visited_Node { get; set; } = 0;
        /// <summary>
        /// 1. IDLE: active but no mission
        /// 2. RUN: executing mission
        /// 3. DOWN: alarm or error
        /// 4. Charging: in charging,
        /// 
        /// </summary>
        public virtual MAIN_STATUS AGV_Status { get; set; }
        public virtual bool Escape_Flag { get; set; } = false;

        public virtual Dictionary<string, int> Sensor_Status { get; set; } = new Dictionary<string, int>
        {
            {"Barcode Reader" ,0 },
            {"Guide Sensor" ,0 },
            {"LiDAR Sensor" ,0 },
            {"Driver" ,0 },
            {"Tag_Reader" ,0 },
            {"G-Sensor" ,0 },
        };

        public virtual double CPU_Usage_Percent { get; set; } = 0;

        public virtual double RAM_Usage_Percent { get; set; } = 0;

        public virtual bool AGV_Reset_Flag { get; set; }
        public virtual double Signal_Strength { get; set; } = 0;

        public virtual int Cargo_Status { get; set; } = 0;

        public virtual string[] CSTID { get; set; } = new string[0];
        public virtual double Odometry { get; set; } = 0;
        public virtual double[] Electric_Volume { get; set; } = new double[2] { 0, 0 };

        public virtual clsAlarmCode[] Alarm_Code { get; set; } = new clsAlarmCode[0];
        public virtual double Fork_Height { get; set; }
    }
}