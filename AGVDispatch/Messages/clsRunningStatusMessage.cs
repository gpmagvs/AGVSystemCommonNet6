﻿using AGVSystemCommonNet6.AGVDispatch.Model;
using Newtonsoft.Json;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
    public class clsRunningStatusReportMessage : MessageBase
    {
        public Dictionary<string, RunningStatus> Header { get; set; } = new Dictionary<string, RunningStatus>();
    }

    public class RunningStatus:Model.clsRunningStatus
    {
        [JsonProperty("Time Stamp")]
        public  override  string Time_Stamp { get; set; } = DateTime.Now.ToAGVSTimeFormat();

        [JsonProperty("Coordination")]
        public override clsCoordination Coordination { get; set; } = new clsCoordination();
        [JsonProperty("Last Visited Node")]
        public override int Last_Visited_Node { get; set; } = 0;
        /// <summary>
        /// 1. IDLE: active but no mission
        /// 2. RUN: executing mission
        /// 3. DOWN: alarm or error
        /// 4. Charging: in charging,
        /// 
        /// </summary>
        [JsonProperty("AGV Status")]
        public override MAIN_STATUS AGV_Status { get; set; } 
        [JsonProperty("Escape Flag")]
        public override bool Escape_Flag { get; set; } = false;
        [JsonProperty("Sensor Status")]

        public override Dictionary<string, int> Sensor_Status { get; set; } = new Dictionary<string, int>
        {
            {"Barcode Reader" ,0 },
            {"Guide Sensor" ,0 },
            {"LiDAR Sensor" ,0 },
            {"Driver" ,0 },
            {"Tag_Reader" ,0 },
            {"G-Sensor" ,0 },
        };

        [JsonProperty("CPU Usage Percent")]
        public override double CPU_Usage_Percent { get; set; } = 0;

        [JsonProperty("RAM Usage Percent")]
        public override double RAM_Usage_Percent { get; set; } = 0;

        [JsonProperty("AGV Reset Flag")]
        public override bool AGV_Reset_Flag { get; set; }
        [JsonProperty("Signal Strength")]
        public override double Signal_Strength { get; set; } = 0;

        [JsonProperty("Cargo Status")]
        public override int Cargo_Status { get; set; } = 0;

        public override string[] CSTID { get; set; } = new string[0];
        public override double Odometry { get; set; } = 0;


        [JsonProperty("Electric Volume")]
        public override double[] Electric_Volume { get; set; } = new double[2] { 0, 0 };

        [JsonProperty("Alarm Code")]
        public new clsAlarmCode[] Alarm_Code { get; set; } = new clsAlarmCode[0];

        [JsonProperty("Fork Height")]
        public override double Fork_Height { get; set; }

   
        public class clsAlarmCode : Model.clsAlarmCode
        {
            [JsonProperty("Alarm ID")]
            public override int Alarm_ID { get; set; }

            /// <summary>
            ///  1: Serious, 0: Light
            /// </summary>
            [JsonProperty("Alarm Level")]
            public override int Alarm_Level { get; set; }

            /// <summary>
            /// 0: Task Recoverable, other: Unrecoverable
            /// </summary>
            [JsonProperty("Alarm Category")]
            public override int Alarm_Category { get; set; }
            [JsonProperty("Alarm Description")]
            public override string Alarm_Description { get; set; } = "";
        }
    }
    public class clsRunningStatusReportResponseMessage : MessageBase
    {
        public Dictionary<string, SimpleRequestResponseWithTimeStamp> Header { get; set; }
        internal SimpleRequestResponseWithTimeStamp RuningStateReportAck => this.Header[Header.Keys.First()];

    }

}
