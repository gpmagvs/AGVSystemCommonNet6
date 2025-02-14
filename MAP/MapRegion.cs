using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    /// <summary>
    /// 地圖上的禁制區
    /// </summary>
    public class MapRegion
    {
        public enum MAP_REGION_TYPE
        {
            FORBID, PASSIBLE,
            UNKNOWN
        }

        public string Name { get; set; } = "";
        public MAP_REGION_TYPE RegionType { get; set; } = MAP_REGION_TYPE.FORBID;
        public List<double[]> PolygonCoordinations { get; set; } = new List<double[]>();
        public bool IsOpend { get; set; } = false;
        public bool IsNarrowPath { get; set; } = true;

        public bool IsPathCanUseWhenAgvNoPathToUse { get; set; } = false;

        /// <summary>
        /// 最多可容納車輛數
        /// </summary>
        public int MaxVehicleCapacity { get; set; } = 2;
        /// <summary>
        /// 限速
        /// </summary>
        public double SpeedLimit { get; set; } = 1;
        /// <summary>
        /// 入口Tag
        /// </summary>
        public List<int> EnteryTags { get; set; } = new List<int>();
        /// <summary>
        /// 出口Tag
        /// </summary>
        public List<int> LeavingTags { get; set; } = new List<int>();
        /// <summary>
        /// 車輛閒置時應該朝向的角度
        /// </summary>
        public double ThetaLimitWhenAGVIdling { get; set; } = 0;

        /// <summary>
        /// 區域內的路徑僅可供那些Tag為終點時使用,若車輛已在該區域內則不限
        /// </summary>
        public List<int> PathOnlyUseForTagsWhenVehicleFromOutsideRegion { get; set; } = new List<int>();

        [JsonIgnore]
        public List<string> InRegionVehicles = new List<string>();


        [JsonIgnore]
        public List<string> ReserveRegionVehicles = new List<string>();
    }
}
