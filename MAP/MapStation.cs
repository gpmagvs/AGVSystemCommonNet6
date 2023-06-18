using AGVSystemCommonNet6.AGVDispatch.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class MapStation
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string? Name { get; set; }
        public int TagNumber { get; set; }
        public int Direction { get; set; }
        public bool AGV_Alarm { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 是否可停駐
        /// </summary>
        public bool IsStandbyPoint { get; set; }

        /// <summary>
        /// 是否為二次定位點
        /// </summary>
        public bool IsSegment { get; set; }
        public bool IsOverlap { get; set; }
        /// <summary>
        /// 是否可停車
        /// </summary>
        public bool IsParking { get; set; }
        /// <summary>
        /// 是否可避車
        /// </summary>
        public bool IsAvoid { get; set; }
        /// <summary>
        /// 是否為虛擬點
        /// </summary>
        public bool IsVirtualPoint { get; set; }
        /// <summary>
        /// 是否為自動門
        /// </summary>
        public bool IsAutoDoor { get; set; }
        /// <summary>
        /// 是否為消防設備
        /// </summary>
        public bool IsExtinguishing { get; set; }
        /// <summary>
        /// 註冊點(,逗點分隔)
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? InvolvePoint { get; set; }
        public STATION_TYPE StationType { get; set; }
        public int LsrMode { get; set; }
        public double Speed { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Bay { get; set; }
        public Graph Graph { get; set; }
        /// <summary>
        /// Key Point Index, value:權重
        /// </summary>
        public Dictionary<string, int>? Target { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? DodgeMode { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? SpinMode { get; set; }
        public string? SubMap { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public AutoDoor? AutoDoor { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public MotionInfo? MotionInfo { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]

        public string? Region { get; set; }
    }
}
