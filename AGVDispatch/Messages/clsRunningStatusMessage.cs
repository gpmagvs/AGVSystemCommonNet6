using AGVSystemCommonNet6.AGVDispatch.Model;
using Newtonsoft.Json;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
    public class clsRunningStatusReportMessage : MessageBase
    {
        public Dictionary<string, RunningStatus> Header { get; set; } = new Dictionary<string, RunningStatus>();
    }

    public class RunningStatus
    {
        [JsonProperty("Time Stamp")]
        public string Time_Stamp { get; set; } = DateTime.Now.ToAGVSTimeFormat();

        [JsonProperty("Coordination")]
        public clsCoordination Coordination { get; set; } = new clsCoordination();
        [JsonProperty("Last Visited Node")]
        public int Last_Visited_Node { get; set; } = 0;
        /// <summary>
        /// 1. IDLE: active but no mission
        /// 2. RUN: executing mission
        /// 3. DOWN: alarm or error
        /// 4. Charging: in charging,
        /// 
        /// </summary>
        [JsonProperty("AGV Status")]
        public MAIN_STATUS AGV_Status { get; set; }

        [JsonProperty("Alarm Code")]
        public new clsAlarmCode[] Alarm_Code { get; set; }

        [JsonProperty("Escape Flag")]
        public bool Escape_Flag { get; set; } = false;
        [JsonProperty("Sensor Status")]
        public Dictionary<string, int> Sensor_Status { get; set; } = new Dictionary<string, int>
        {
            {"Barcode Reader" ,0 },
            {"Guide Sensor" ,0 },
            {"LiDAR Sensor" ,0 },
            {"Driver" ,0 },
            {"Tag_Reader" ,0 },
            {"G-Sensor" ,0 },
        };

        [JsonProperty("CPU Usage Percent")]
        public double CPU_Usage_Percent { get; set; } = 0;

        [JsonProperty("RAM Usage Percent")]
        public double RAM_Usage_Percent { get; set; } = 0;

        [JsonProperty("AGV Reset Flag")]
        public bool AGV_Reset_Flag { get; set; }
        [JsonProperty("Signal Strength")]
        public double Signal_Strength { get; set; } = 0;

        [JsonProperty("Cargo Status")]
        public int Cargo_Status { get; set; } = 0;

        public  string[] CSTID { get; set; } = new string[0];

        /// <summary>
        ///  0:None,200:Tray
        /// </summary>
        [JsonProperty("Cargo Type")]
        public int CargoType { get; set; } = 0;

        public  double Odometry { get; set; } = 0;



        [JsonProperty("Electric Volume")]
        public double[] Electric_Volume { get; set; } = new double[2] { 0, 0 };


        [JsonProperty("Fork Height")]
        public  double Fork_Height { get; set; }

        public bool IsCharging { get; set; } = false;

    }
    public class clsAlarmCode
    {
        [JsonProperty("Alarm ID")]
        public virtual int Alarm_ID { get; set; }

        /// <summary>
        ///  1: Serious, 0: Light
        /// </summary>
        [JsonProperty("Alarm Level")]
        public virtual int Alarm_Level { get; set; }

        /// <summary>
        /// 0: Task Recoverable, other: Unrecoverable
        /// </summary>
        [JsonProperty("Alarm Category")]
        public virtual int Alarm_Category { get; set; }
        [JsonProperty("Alarm Description")]
        public virtual string Alarm_Description { get; set; } = "";

        [JsonProperty("Alarm Description EN")]
        public virtual string Alarm_Description_EN { get; set; } = "";

    }

    public class clsRunningStatusReportResponseMessage : MessageBase
    {
        public Dictionary<string, SimpleRequestResponseWithTimeStamp> Header { get; set; }
        internal SimpleRequestResponseWithTimeStamp RuningStateReportAck => this.Header[Header.Keys.First()];

    }

}
