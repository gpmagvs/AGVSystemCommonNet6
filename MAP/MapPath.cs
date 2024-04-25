using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class MapPath
    {
        public int StartPtIndex { get; set; }
        public int EndPtIndex { get; set; }

        public double[] StartCoordination { get; set; }
        public double[] EndCoordination { get; set; }
        public string PathID => $"{StartPtIndex}_{EndPtIndex}";

        public bool IsEQLink { get; set; }

        /// <summary>
        /// 是否僅允許一部車子通行
        /// </summary>
        public bool IsSingleCar { get; set; } = false;

        /// <summary>
        /// 是否可旋轉
        /// </summary>
        public bool IsRotationable { get; set; } = true;
        public bool IsPassable { get; set; } = true;

        /// <summary>
        /// 是否為禁止停車之消防路徑
        /// </summary>
        public bool IsExtinguishingPath { get; set; } = false;

        /// <summary>
        /// 限速(為速度比例, ex. Speed=1 =>全速)
        /// </summary>
        public double Speed { get; set; } = 1;
        public int LsrMode { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? DodgeMode { get; set; } = 0;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? SpinMode { get; set; } = 0;
        public double Weight { get; set; } = 0.0;
    }
}
